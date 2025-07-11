import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import { UseFormReset, UseFormSetError } from "react-hook-form";

import { createCustomer } from "@entities/users";

import { ErrorHandler } from "@shared/lib/handlers/error";

import { CREATE_CUSTOMER_FORM_FIELD_MAPPING, CreateCustomerFormData } from "../model";

export const useCreateCustomer = (
	reset: UseFormReset<CreateCustomerFormData>,
	setError: UseFormSetError<CreateCustomerFormData>
) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: createCustomer,
		onSuccess: (userId) => {
			queryClient.invalidateQueries({
				queryKey: ["users", "list", "customers"]
			});

			toast.success("User created successfully", {
				description: `Id: ${userId}`
			});

			reset();
		},
		onError: ErrorHandler.createMutationErrorHandler(
			setError,
			CREATE_CUSTOMER_FORM_FIELD_MAPPING,
			{
				action: "create_customer",
				resource: "user"
			}
		)
	});
};
