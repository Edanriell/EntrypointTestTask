import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import { UseFormReset, UseFormSetError } from "react-hook-form";

import { addPayment } from "@entities/payments";

import { ErrorHandler } from "@shared/lib/handlers/error";

import { AddPaymentFormData } from "../model";

export const useAddPayment = (
	reset: UseFormReset<AddPaymentFormData>,
	setError: UseFormSetError<AddPaymentFormData>
) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: addPayment,
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

			toast.success("Payment added successfully", {
				description: `Order ID: ${orderId}`
			});

			reset(); // Reset form on success
		},
		onError: ErrorHandler.createMutationErrorHandler(setError, reset, {
			action: "add_payment",
			resource: "payment"
		})
	});
};
