import { UseFormReset, UseFormSetError } from "react-hook-form";
import { toast } from "sonner";
import { useMutation, useQueryClient } from "@tanstack/react-query";

import { createProduct } from "@entities/products";

import { ErrorHandler } from "@shared/lib/handlers/error";

import { CREATE_PRODUCT_FORM_FIELD_MAPPING, CreateProductFormData } from "../model";

export const useCreateProduct = (
	reset: UseFormReset<CreateProductFormData>,
	setError: UseFormSetError<CreateProductFormData>
) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: createProduct,
		onSuccess: (productId) => {
			queryClient.invalidateQueries({
				queryKey: ["products", "list"]
			});

			toast.success("Product created successfully", {
				description: `Product ID: ${productId}`
			});

			reset();
		},
		onError: ErrorHandler.createMutationErrorHandler(
			setError,
			CREATE_PRODUCT_FORM_FIELD_MAPPING,
			{
				action: "create_product",
				resource: "product"
			}
		)
	});
};
