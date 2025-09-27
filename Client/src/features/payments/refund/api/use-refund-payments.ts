import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

import { refundPayments } from "@entities/payments";

export const useRefundPayments = () => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: refundPayments,
		onSuccess: (_, { orderId }) => {
			queryClient.invalidateQueries({
				queryKey: ["orders"]
			});

			queryClient.invalidateQueries({
				queryKey: ["orders", "orderDetail", orderId]
			});

			queryClient.invalidateQueries({
				queryKey: ["payments"]
			});

			queryClient.invalidateQueries({
				queryKey: ["payments", "order", orderId]
			});

			toast.success("Payments refunded successfully", {
				description: `Order ID: ${orderId}`
			});
		},
		onError: (error: any) => {
			toast.error("Failed to refund payments", {
				description: error?.response?.data?.message || "Please try again"
			});
		}
	});
};
