import { UseFormReset, UseFormSetError } from "react-hook-form";
import { toast } from "sonner";
import { useMutation, useQueryClient } from "@tanstack/react-query";

import { createOrder } from "@entities/orders";

import { ErrorHandler } from "@shared/lib/handlers/error";

import { CREATE_ORDER_FORM_FIELD_MAPPING, CreateOrderFormData } from "../model";

export const useCreateOrder = (
	reset: UseFormReset<CreateOrderFormData>,
	setError: UseFormSetError<CreateOrderFormData>
) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: createOrder,
		onSuccess: (orderId) => {
			queryClient.invalidateQueries({
				queryKey: ["orders", "list"]
			});

			toast.success("Order created successfully", {
				description: `Order ID: ${orderId}`
			});

			reset();
		},
		onError: ErrorHandler.createMutationErrorHandler(
			setError,
			CREATE_ORDER_FORM_FIELD_MAPPING,
			{
				action: "create_order",
				resource: "order"
			}
		)
	});
};
