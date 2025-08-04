import { useMutation, useQueryClient } from "@tanstack/react-query";
import { UseFormSetError } from "react-hook-form";
import { toast } from "sonner";

import type { GetCustomerByIdResponse, UpdateUserCommand } from "@entities/users";
import { updateUser } from "@entities/users";

import { ErrorHandler } from "@shared/lib/handlers/error";

import { EDIT_USER_FORM_FIELDS_MAPPING, EditUserFormData } from "../model";

type MutationContext = {
	previousUserData?: GetCustomerByIdResponse;
};

export const useUpdateUser = (setError: UseFormSetError<EditUserFormData>) => {
	const queryClient = useQueryClient();

	return useMutation<
		unknown, // data returned by mutationFn (we ignore it)
		Error, // error type
		UpdateUserCommand, // variables type
		MutationContext // context type
	>({
		mutationFn: ({ userId, updatedUserData }) => updateUser({ userId, updatedUserData }),

		onMutate: async ({ userId, updatedUserData }) => {
			// IMPORTANT!
			// Cancel any outgoing fetches so they don’t overwrite our optimistic update
			await queryClient.cancelQueries({
				queryKey: ["users", "customerDetail", userId]
			});

			// IMPORTANT!
			// Snapshot the current value
			const previousUserData = queryClient.getQueryData<GetCustomerByIdResponse>([
				"users",
				"customerDetail",
				userId
			]);

			// Optimistically update the cache
			queryClient.setQueryData(
				["users", "customerDetail", userId],
				(oldData: GetCustomerByIdResponse | undefined) =>
					oldData
						? {
								...oldData,
								...updatedUserData,
								fullName:
									updatedUserData.firstName && updatedUserData.lastName
										? `${updatedUserData.firstName} ${updatedUserData.lastName}`
										: `${updatedUserData.firstName ?? oldData.firstName} ${
												updatedUserData.lastName ?? oldData.lastName
											}`
							}
						: oldData
			);

			// Return context so it’s available in onError/onSettled
			return { previousUserData };
		},

		onError: (error, { userId }, context) => {
			// Roll back if we have a snapshot
			if (context?.previousUserData) {
				queryClient.setQueryData(
					["users", "customerDetail", userId],
					context.previousUserData
				);
			}

			ErrorHandler.createMutationErrorHandler(setError, EDIT_USER_FORM_FIELDS_MAPPING, {
				action: "update_user",
				resource: "user"
			})(error);
		},

		onSuccess: (_, { userId }) => {
			toast.success("User updated successfully", { description: `Id: ${userId}` });
		},

		onSettled: (_, __, { userId }) => {
			// IMPORTANT!
			// Always re-validate after the mutation has either succeeded or failed
			queryClient.invalidateQueries({ queryKey: ["users", "customerDetail", userId] });
			queryClient.invalidateQueries({ queryKey: ["users"] });
		}
	});
};
