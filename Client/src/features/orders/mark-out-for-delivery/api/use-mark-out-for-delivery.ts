import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import { UseFormReset, UseFormSetError } from "react-hook-form";

import { markOutForDelivery } from "@entities/orders";

import { ErrorHandler } from "@shared/lib/handlers/error";

import { MarkOutForDeliveryFormData } from "../model";

export const useMarkOutForDelivery = (
	reset: UseFormReset<MarkOutForDeliveryFormData>,
	setError: UseFormSetError<MarkOutForDeliveryFormData>
) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: markOutForDelivery,
		onSuccess: (_, { orderId }) => {
			queryClient.invalidateQueries({
				queryKey: ["orders"]
			});

			queryClient.invalidateQueries({
				queryKey: ["orders", "orderDetail", orderId]
			});

			toast.success("Order marked out for delivery successfully", {
				description: `Order ID: ${orderId}`
			});

			reset(); // Reset form on success
		},
		onError: ErrorHandler.createMutationErrorHandler(setError, reset as any, {
			action: "mark_out_for_delivery",
			resource: "order"
		})
	});
};
