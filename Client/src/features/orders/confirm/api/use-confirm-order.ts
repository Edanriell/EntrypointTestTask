import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

import { confirmOrder } from "@entities/orders";

import { ErrorHandler } from "@shared/lib/handlers/error";

export const useConfirmOrder = () => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: confirmOrder,
		onSuccess: (_, { orderId }) => {
			queryClient.invalidateQueries({
				queryKey: ["orders"]
			});

			queryClient.invalidateQueries({
				queryKey: ["orders", "orderDetail", orderId]
			});

			toast.success("Order confirmed successfully", {
				description: `Order ID: ${orderId}`
			});
		},
		onError: ErrorHandler.createMutationErrorHandler(undefined, undefined, {
			action: "confirm_order",
			resource: "order"
		})
	});
};
