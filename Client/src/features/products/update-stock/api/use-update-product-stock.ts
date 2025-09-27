import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import { UseFormSetError } from "react-hook-form";

import type {
	GetProductByIdResponse,
	GetProductsResponse,
	UpdateProductStockCommand
} from "@entities/products";
import { updateProductStock } from "@entities/products";

import { ErrorHandler } from "@shared/lib/handlers/error";

import { UPDATE_PRODUCT_STOCK_FORM_FIELD_MAPPING, UpdateProductStockFormData } from "../model";

type MutationContext = {
	previousProductData?: GetProductByIdResponse;
	previousProductsListData?: GetProductsResponse;
};

export const useUpdateProductStock = (setError: UseFormSetError<UpdateProductStockFormData>) => {
	const queryClient = useQueryClient();

	return useMutation<void, Error, UpdateProductStockCommand, MutationContext>({
		mutationFn: ({ productId, updatedProductStockData }) =>
			updateProductStock({ productId, updatedProductStockData }),

		onMutate: async ({ productId, updatedProductStockData }) => {
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
								...updatedProductStockData,
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
									...updatedProductStockData,
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
				setError,
				UPDATE_PRODUCT_STOCK_FORM_FIELD_MAPPING,
				{
					action: "update_product_stock",
					resource: "product"
				}
			)(error);
		},

		onSuccess: (_, { productId, updatedProductStockData }, context) => {
			toast.success("Product stock updated successfully", {
				description: `New stock: ${context.previousProductData!.totalStock + updatedProductStockData.totalStock!}`
			});
		},

		onSettled: (_, __, { productId }) => {
			// Always re-validate after the mutation has either succeeded or failed
			queryClient.invalidateQueries({ queryKey: ["products", "productDetail", productId] });
			queryClient.invalidateQueries({ queryKey: ["products", "list"] });
		}
	});
};
