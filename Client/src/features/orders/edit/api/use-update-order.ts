import { useMutation, useQueryClient } from "@tanstack/react-query";
import { UseFormSetError } from "react-hook-form";
import { toast } from "sonner";

import type { GetOrderByIdResponse, UpdateOrderCommand } from "@entities/orders";
import { updateOrder } from "@entities/orders";

import { ErrorHandler } from "@shared/lib/handlers/error";

import { EDIT_ORDER_FORM_FIELDS_MAPPING, EditOrderFormData } from "../model";
import { transformToOrderFormat } from "../lib";

type MutationContext = {
	previousOrderData?: GetOrderByIdResponse;
};

export const useUpdateOrder = (setError: UseFormSetError<EditOrderFormData>) => {
	const queryClient = useQueryClient();

	return useMutation<unknown, Error, UpdateOrderCommand, MutationContext>({
		mutationFn: ({ orderId, updatedOrderData }) => updateOrder({ orderId, updatedOrderData }),

		onMutate: async ({ orderId, updatedOrderData }) => {
			await queryClient.cancelQueries({
				queryKey: ["orders", "orderDetail", orderId]
			});

			const previousOrderData = queryClient.getQueryData<GetOrderByIdResponse>([
				"orders",
				"orderDetail",
				orderId
			]);

			// Optimistically update with transformed data
			queryClient.setQueryData(
				["orders", "orderDetail", orderId],
				(oldData: GetOrderByIdResponse | undefined) =>
					oldData ? transformToOrderFormat(updatedOrderData, oldData) : oldData
			);

			return { previousOrderData };
		},

		onError: (error, { orderId }, context) => {
			if (context?.previousOrderData) {
				queryClient.setQueryData(
					["orders", "orderDetail", orderId],
					context.previousOrderData
				);
			}

			ErrorHandler.createMutationErrorHandler(setError, EDIT_ORDER_FORM_FIELDS_MAPPING, {
				action: "update_order",
				resource: "order"
			})(error);
		},

		onSuccess: (_, { orderId }) => {
			toast.success("Order updated successfully", { description: `Id: ${orderId}` });
		},

		onSettled: (_, __, { orderId }) => {
			queryClient.invalidateQueries({ queryKey: ["orders", "orderDetail", orderId] });
			queryClient.invalidateQueries({ queryKey: ["orders"] });
		}
	});
};
