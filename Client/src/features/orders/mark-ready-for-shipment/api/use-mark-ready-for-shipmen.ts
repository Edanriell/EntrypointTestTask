import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

import { markReadyForShipment } from "@entities/orders";

import { ErrorHandler } from "@shared/lib/handlers/error";

export const useMarkReadyForShipment = () => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: markReadyForShipment,
		onSuccess: (_, { orderId }) => {
			queryClient.invalidateQueries({
				queryKey: ["orders"]
			});

			queryClient.invalidateQueries({
				queryKey: ["orders", "orderDetail", orderId]
			});

			toast.success("Order marked ready for shipment successfully", {
				description: `Order ID: ${orderId}`
			});
		},
		onError: ErrorHandler.createMutationErrorHandler(undefined, undefined, {
			action: "mark_ready_for_shipment",
			resource: "order"
		})
	});
};
