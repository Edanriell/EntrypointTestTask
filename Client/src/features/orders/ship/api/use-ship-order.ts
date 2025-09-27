import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import { UseFormReset, UseFormSetError } from "react-hook-form";

import { shipOrder } from "@entities/orders";

import { ErrorHandler } from "@shared/lib/handlers/error";

import { ShipOrderFormData } from "../model";

export const useShipOrder = (
	reset: UseFormReset<ShipOrderFormData>,
	setError: UseFormSetError<ShipOrderFormData>
) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: shipOrder, // This already expects { orderId, orderData }
		onSuccess: (_, { orderId }) => {
			queryClient.invalidateQueries({
				queryKey: ["orders"]
			});

			queryClient.invalidateQueries({
				queryKey: ["orders", "orderDetail", orderId]
			});

			toast.success("Order shipped successfully", {
				description: `Order ID: ${orderId}`
			});

			reset(); // Reset form on success
		},
		onError: ErrorHandler.createMutationErrorHandler(setError, reset, {
			action: "ship_order",
			resource: "order"
		})
	});
};
