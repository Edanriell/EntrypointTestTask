import { useMutation, useQueryClient } from "@tanstack/react-query";
import { UseFormSetError } from "react-hook-form";
import { toast } from "sonner";

import type { GetProductByIdResponse, UpdateProductCommand } from "@entities/products";
import { updateProduct } from "@entities/products";

import { ErrorHandler } from "@shared/lib/handlers/error";

import { EDIT_PRODUCT_FORM_FIELDS_MAPPING, EditProductFormData } from "../model";

type MutationContext = {
	previousProductData?: GetProductByIdResponse;
};

export const useUpdateProduct = (setError: UseFormSetError<EditProductFormData>) => {
	const queryClient = useQueryClient();

	return useMutation<
		unknown, // data returned by mutationFn (we ignore it)
		Error, // error type
		UpdateProductCommand, // variables type
		MutationContext // context type
	>({
		mutationFn: ({ productId, updatedProductData }) =>
			updateProduct({ productId, updatedProductData }),

		onMutate: async ({ productId, updatedProductData }) => {
			// Cancel any outgoing fetches so they don't overwrite our optimistic update
			await queryClient.cancelQueries({
				queryKey: ["products", "productDetail", productId]
			});

			// Snapshot the current value
			const previousProductData = queryClient.getQueryData<GetProductByIdResponse>([
				"products",
				"productDetail",
				productId
			]);

			// Optimistically update the cache
			queryClient.setQueryData(
				["products", "productDetail", productId],
				(oldData: GetProductByIdResponse | undefined) =>
					oldData
						? {
								...oldData,
								...updatedProductData
							}
						: oldData
			);

			// Return context so it's available in onError/onSettled
			return { previousProductData };
		},

		onError: (error, { productId }, context) => {
			// Roll back if we have a snapshot
			if (context?.previousProductData) {
				queryClient.setQueryData(
					["products", "productDetail", productId],
					context.previousProductData
				);
			}

			ErrorHandler.createMutationErrorHandler(setError, EDIT_PRODUCT_FORM_FIELDS_MAPPING, {
				action: "update_product",
				resource: "product"
			})(error);
		},

		onSuccess: (_, { productId }) => {
			toast.success("Product updated successfully", { description: `Id: ${productId}` });
		},

		onSettled: (_, __, { productId }) => {
			// Always re-validate after the mutation has either succeeded or failed
			queryClient.invalidateQueries({ queryKey: ["products", "productDetail", productId] });
			queryClient.invalidateQueries({ queryKey: ["products"] });
		}
	});
};
