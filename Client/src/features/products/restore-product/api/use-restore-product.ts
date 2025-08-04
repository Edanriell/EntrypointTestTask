import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

import type {
	GetProductByIdResponse,
	GetProductsResponse,
	RestoreProductCommand
} from "@entities/products";
import { restoreProduct } from "@entities/products";
import { ProductStatus } from "@entities/products/model";

import { ErrorHandler } from "@shared/lib/handlers/error";

type MutationContext = {
	previousProductData?: GetProductByIdResponse;
	previousProductsListData?: GetProductsResponse;
};

export const useRestoreProduct = () => {
	const queryClient = useQueryClient();

	return useMutation<void, Error, RestoreProductCommand, MutationContext>({
		mutationFn: ({ productId }) => restoreProduct({ productId }),

		onMutate: async ({ productId }) => {
			// Cancel any outgoing fetches so they don't overwrite our optimistic update
			await queryClient.cancelQueries({
				queryKey: ["products", "productDetail", productId]
			});

			// Also cancel products list queries
			await queryClient.cancelQueries({
				queryKey: ["products", "list"]
			});

			// Snapshot the current values with proper typing
			const previousProductData = queryClient.getQueryData<GetProductByIdResponse>([
				"products",
				"productDetail",
				productId
			]);

			const previousProductsListData = queryClient.getQueryData<GetProductsResponse>([
				"products",
				"list"
			]);

			// Optimistically update the product detail cache
			queryClient.setQueryData(
				["products", "productDetail", productId],
				(oldData: GetProductByIdResponse | undefined) =>
					oldData
						? {
								...oldData,
								status: ProductStatus.Available,
								lastUpdatedAt: new Date().toISOString()
							}
						: oldData
			);

			// Optimistically update the products list cache
			queryClient.setQueryData<GetProductsResponse>(["products", "list"], (oldData) => {
				if (!oldData?.products) return oldData;

				return {
					...oldData,
					products: oldData.products.map((product) =>
						product.id === productId
							? {
									...product,
									status: ProductStatus.Available,
									lastUpdatedAt: new Date().toISOString()
								}
							: product
					)
				};
			});

			// Return context so it's available in onError/onSettled
			return { previousProductData, previousProductsListData };
		},

		onError: (error, { productId }, context) => {
			// Roll back product detail if we have a snapshot
			if (context?.previousProductData) {
				queryClient.setQueryData(
					["products", "productDetail", productId],
					context.previousProductData
				);
			}

			// Roll back products list if we have a snapshot
			if (context?.previousProductsListData) {
				queryClient.setQueryData(["products", "list"], context.previousProductsListData);
			}

			ErrorHandler.createMutationErrorHandler(
				() => undefined, // No form errors for this simple action
				undefined, // No field mapping needed
				{
					action: "restore_product",
					resource: "product"
				}
			)(error);
		},

		onSuccess: (_, { productId }) => {
			toast.success("Product restored successfully", {
				description: `Product ID: ${productId}`
			});
		},

		onSettled: (_, __, { productId }) => {
			// Always re-validate after the mutation has either succeeded or failed
			queryClient.invalidateQueries({ queryKey: ["products", "productDetail", productId] });
			queryClient.invalidateQueries({ queryKey: ["products", "list"] });
		}
	});
};
