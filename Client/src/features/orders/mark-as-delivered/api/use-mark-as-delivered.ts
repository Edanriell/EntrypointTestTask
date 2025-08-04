// src/features/orders/mark-as-delivered/api/use-mark-as-delivered.ts
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

import { markOrderAsDelivered } from "@entities/orders";

export const useMarkAsDelivered = () => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: markOrderAsDelivered,
		onSuccess: (_, { orderId }) => {
			queryClient.invalidateQueries({
				queryKey: ["orders"]
			});

			queryClient.invalidateQueries({
				queryKey: ["orders", "orderDetail", orderId]
			});

			toast.success("Order marked as delivered successfully", {
				description: `Order ID: ${orderId}`
			});
		},
		onError: (error: any) => {
			toast.error("Failed to mark order as delivered", {
				description: error?.response?.data?.message || "Please try again"
			});
		}
	});
};
