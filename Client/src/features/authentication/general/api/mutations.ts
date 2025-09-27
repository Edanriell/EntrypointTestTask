import { useMutation, UseMutationOptions } from "@tanstack/react-query";

import { AccessTokenResponse, LoginUserRequest, LogoutRequest } from "../model";
import { loginUser, logoutUser } from "../api";

export const useLoginMutation = (
	options?: UseMutationOptions<AccessTokenResponse, Error, LoginUserRequest>
) => {
	return useMutation({
		mutationFn: loginUser,
		...options
	});
};

export const useLogoutMutation = (
	options?: UseMutationOptions<void, Error, LogoutRequest | undefined>
) => {
	return useMutation({
		mutationFn: logoutUser,
		...options
	});
};
