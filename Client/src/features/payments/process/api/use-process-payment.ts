import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

import { processPayment } from "@entities/payments";

import { ErrorHandler } from "@shared/lib/handlers/error";

export const useProcessPayment = (orderId: string) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: processPayment,
		onSuccess: (_, { paymentId }) => {
			queryClient.invalidateQueries({
				queryKey: ["payments"]
			});

			queryClient.invalidateQueries({
				queryKey: ["orders"]
			});

			queryClient.invalidateQueries({
				queryKey: ["orders", "orderDetail", orderId]
			});

			toast.success("Payment processed successfully", {
				description: `Payment ID: ${paymentId}`
			});
		},
		onError: ErrorHandler.createMutationErrorHandler(undefined, undefined, {
			action: "process_payment",
			resource: "payment"
		})
	});
};
