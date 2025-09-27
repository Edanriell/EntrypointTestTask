import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

import { returnOrder } from "@entities/orders";

export const useReturnOrder = () => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: returnOrder,
		onSuccess: (_, { orderId }) => {
			queryClient.invalidateQueries({
				queryKey: ["orders"]
			});

			queryClient.invalidateQueries({
				queryKey: ["orders", "orderDetail", orderId]
			});

			toast.success("Order returned successfully", {
				description: `Order ID: ${orderId}`
			});
		},
		onError: (error: any) => {
			toast.error("Failed to return order", {
				description: error?.response?.data?.message || "Please try again"
			});
		}
	});
};
