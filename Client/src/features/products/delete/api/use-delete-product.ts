import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

import { deleteProduct } from "@entities/products";

import { ErrorHandler } from "@shared/lib/handlers/error";

export const useDeleteProduct = (setIsDialogOpen: (open: boolean) => void) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: deleteProduct,
		onSuccess: (_, { productId }) => {
			queryClient.invalidateQueries({
				queryKey: ["products", "list"]
			});

			queryClient.removeQueries({
				queryKey: ["products", "detail", productId]
			});

			toast.success("Product deleted successfully", {
				description: `Id: ${productId}`
			});

			setIsDialogOpen(false);
		},
		onError: ErrorHandler.createMutationErrorHandler(undefined, undefined, {
			action: "delete_product",
			resource: "product"
		})
	});
};
