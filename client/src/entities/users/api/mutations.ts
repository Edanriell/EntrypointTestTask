import { useMutation, UseMutationOptions, useQueryClient } from "@tanstack/react-query";
import { createUser } from "./create-user";
import { updateUser } from "./update-user";
import { deleteUser } from "./delete-user";
import { loginUser } from "./login-user";
import {
	AccessTokenResponse,
	LoginUserRequest,
	RegisterUserRequest,
	UpdateUserRequest
} from "../model";
import { usersQueries } from "./users.query";

export const useCreateUser = (options?: UseMutationOptions<string, Error, RegisterUserRequest>) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: createUser,
		onSuccess: () => {
			// Invalidate and refetch users list
			queryClient.invalidateQueries({ queryKey: usersQueries.lists() });
		},
		...options
	});
};

export const useUpdateUser = (options?: UseMutationOptions<void, Error, UpdateUserRequest>) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: updateUser,
		onSuccess: (_, variables) => {
			// Invalidate and refetch users list and specific user
			queryClient.invalidateQueries({ queryKey: usersQueries.lists() });
			queryClient.invalidateQueries({
				queryKey: usersQueries.details()
			});
			queryClient.invalidateQueries({
				queryKey: usersQueries.me().queryKey
			});
		},
		...options
	});
};

export const useDeleteUser = (options?: UseMutationOptions<void, Error, string>) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: deleteUser,
		onSuccess: (_, userId) => {
			// Invalidate and refetch users list
			queryClient.invalidateQueries({ queryKey: usersQueries.lists() });
			// Remove the specific user from cache
			queryClient.removeQueries({
				queryKey: [...usersQueries.details(), userId]
			});
		},
		...options
	});
};

export const useLoginUser = (
	options?: UseMutationOptions<AccessTokenResponse, Error, LoginUserRequest>
) => {
	return useMutation({
		mutationFn: loginUser,
		...options
	});
};
