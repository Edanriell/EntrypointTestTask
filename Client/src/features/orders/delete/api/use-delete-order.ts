import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

import { deleteOrder } from "@entities/orders";

import { ErrorHandler } from "@shared/lib/handlers/error";

export const useDeleteOrder = (setIsDialogOpen: (open: boolean) => void) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: deleteOrder,
		onSuccess: (_, { orderId }) => {
			queryClient.invalidateQueries({
				queryKey: ["orders", "list"]
			});

			queryClient.removeQueries({
				queryKey: ["orders", "orderDetail", orderId]
			});

			toast.success("Order deleted successfully", {
				description: `Id: ${orderId}`
			});

			setIsDialogOpen(false);
		},
		onError: ErrorHandler.createMutationErrorHandler(undefined, undefined, {
			action: "delete_order",
			resource: "order"
		})
	});
};
