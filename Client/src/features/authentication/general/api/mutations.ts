import { useMutation, UseMutationOptions } from "@tanstack/react-query";
import { AccessTokenResponse } from "@features/authentication/general/model/access-token-response";
import { LoginUserRequest } from "@features/authentication/general/model";
import { loginUser } from "@features/authentication/general/api/login-user";
import { logoutUser } from "@features/authentication/general/api/logout-user";
import { LogoutRequest } from "@features/authentication/general/model/logout-request";

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
