import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import { UseFormReset, UseFormSetError } from "react-hook-form";

import { cancelOrder } from "@entities/orders";

import { ErrorHandler } from "@shared/lib/handlers/error";

import { CancelOrderFormData } from "../model";

export const useCancelOrder = (
	reset: UseFormReset<CancelOrderFormData>,
	setError: UseFormSetError<CancelOrderFormData>
) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: cancelOrder,
		onSuccess: (_, { orderId }) => {
			queryClient.invalidateQueries({
				queryKey: ["orders"]
			});

			queryClient.invalidateQueries({
				queryKey: ["orders", "orderDetail", orderId]
			});

			toast.success("Order cancelled successfully", {
				description: `Order ID: ${orderId}`
			});

			reset(); // Reset form on success
		},
		onError: ErrorHandler.createMutationErrorHandler(setError, reset as any, {
			action: "cancel_order",
			resource: "order"
		})
	});
};
