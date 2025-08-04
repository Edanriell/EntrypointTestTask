import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

import { startProcessingOrder } from "@entities/orders";

import { ErrorHandler } from "@shared/lib/handlers/error";

export const useStartProcessingOrder = () => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: startProcessingOrder,
		onSuccess: (_, { orderId }) => {
			queryClient.invalidateQueries({
				queryKey: ["orders"]
			});

			queryClient.invalidateQueries({
				queryKey: ["orders", "orderDetail", orderId]
			});

			toast.success("Order processing started successfully", {
				description: `Order ID: ${orderId}`
			});
		},
		onError: ErrorHandler.createMutationErrorHandler(undefined, undefined, {
			action: "start_processing_order",
			resource: "order"
		})
	});
};
