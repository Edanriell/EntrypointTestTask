import { useMutation, useQueryClient } from "@tanstack/react-query";
import { UseFormSetError } from "react-hook-form";
import { toast } from "sonner";

import type { GetProductByIdResponse, GetProductsResponse } from "@entities/products";
import { discountProduct, DiscountProductCommand } from "@entities/products";

import { ErrorHandler } from "@shared/lib/handlers/error";

import { DISCOUNT_PRODUCT_FORM_FIELDS_MAPPING, DiscountProductFormData } from "../model";

type MutationContext = {
	previousProductData?: GetProductByIdResponse;
	previousProductsListData?: GetProductsResponse;
};

export const useDiscountProduct = (setError: UseFormSetError<DiscountProductFormData>) => {
	const queryClient = useQueryClient();

	return useMutation<unknown, Error, DiscountProductCommand, MutationContext>({
		mutationFn: ({ productId, updatedProductPriceData }) =>
			discountProduct({ productId, updatedProductPriceData }),

		onMutate: async ({ productId, updatedProductPriceData }) => {
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
					oldData && updatedProductPriceData.newPrice !== undefined
						? {
								...oldData,
								...updatedProductPriceData
							}
						: oldData
			);

			// Optimistically update the products list cache
			queryClient.setQueryData<GetProductsResponse>(["products", "list"], (oldData) => {
				if (!oldData?.products) return oldData;

				return {
					...oldData,
					products: oldData.products.map((product) =>
						product.id === productId && updatedProductPriceData.newPrice !== undefined
							? {
									...product,
									...updatedProductPriceData
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
				DISCOUNT_PRODUCT_FORM_FIELDS_MAPPING,
				{
					action: "discount_product",
					resource: "product"
				}
			)(error);
		},

		onSuccess: (_, { updatedProductPriceData }) => {
			toast.success("Product discount applied successfully", {
				description: `New price: $${updatedProductPriceData?.newPrice?.toFixed(2)}`
			});
		},

		onSettled: (_, __, { productId }) => {
			// Always re-validate after the mutation has either succeeded or failed
			queryClient.invalidateQueries({ queryKey: ["products", "productDetail", productId] });
			queryClient.invalidateQueries({ queryKey: ["products", "list"] });
		}
	});
};
