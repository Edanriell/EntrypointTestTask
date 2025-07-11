import { useMutation, useQueryClient } from "@tanstack/react-query";
import { UseFormSetError } from "react-hook-form";
import { toast } from "sonner";

import { updateUser, type UpdateUserRequest } from "@entities/users";

import { ErrorHandler } from "@shared/lib/handlers/error";

import { EDIT_USER_FORM_FIELDS_MAPPING, EditUserFormData } from "../model";

export const useUpdateUser = (setError: UseFormSetError<EditUserFormData>) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: ({ id, data }: { id: string; data: UpdateUserRequest }) => updateUser(id, data),
		onSuccess: (_, { id: userId }) => {
			queryClient.invalidateQueries({
				queryKey: ["users"]
			});

			queryClient.invalidateQueries({
				queryKey: ["users", "customerDetail", userId]
			});

			toast.success("User updated successfully", {
				description: `Id: ${userId}`
			});
		},
		onError: ErrorHandler.createMutationErrorHandler(setError, EDIT_USER_FORM_FIELDS_MAPPING, {
			action: "update_user",
			resource: "user"
		})
	});
};
