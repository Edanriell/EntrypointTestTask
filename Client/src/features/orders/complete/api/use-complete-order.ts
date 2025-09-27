import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

import { completeOrder } from "@entities/orders";

export const useCompleteOrder = () => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: completeOrder,
		onSuccess: (_, { orderId }) => {
			queryClient.invalidateQueries({
				queryKey: ["orders"]
			});

			queryClient.invalidateQueries({
				queryKey: ["orders", "orderDetail", orderId]
			});

			toast.success("Order completed successfully", {
				description: `Order ID: ${orderId}`
			});
		},
		onError: (error: any) => {
			toast.error("Failed to complete order", {
				description: error?.response?.data?.message || "Please try again"
			});
		}
	});
};
