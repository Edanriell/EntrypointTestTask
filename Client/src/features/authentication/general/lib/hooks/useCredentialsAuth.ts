import { signIn, signOut } from "next-auth/react";
import { useCallback, useState } from "react";
import { FieldPath } from "react-hook-form";
import { toast } from "sonner";

import { useBaseAuth } from "@features/authentication/general/lib/hooks/useBaseAuth";
import { AuthProvider } from "@features/authentication/general/model";
import {
	useLoginMutation,
	useLogoutMutation
} from "@features/authentication/general/api/mutations";

import { ApiError, ErrorHandler } from "@shared/lib/handlers/error";

type LoginFormData = {
	email: string;
	password: string;
};

// Field mapping for login form
const LOGIN_FIELD_MAPPING: Record<string, FieldPath<LoginFormData>> = {
	Email: "email",
	Password: "password"
} as const;

type CredentialsLoginOptions = {
	email: string;
	password: string;
	callbackUrl?: string;
};

export const useCredentialsAuth = (): AuthProvider &
	ReturnType<typeof useBaseAuth> & {
		isLoading: boolean;
	} => {
	const [isLoading, setIsLoading] = useState(false);
	const baseAuth = useBaseAuth();

	const loginMutation = useLoginMutation({
		onSuccess: (data, { email }) => {
			toast.success("Welcome back!", {
				description: `Successfully signed in as ${email}`,
				duration: 3000
			});
		},
		onError: ErrorHandler.createMutationErrorHandler(baseAuth.setError, LOGIN_FIELD_MAPPING, {
			action: "sign_in",
			resource: "user"
		})
	});

	// Not implemented on the back-end, just an example
	const logoutMutation = useLogoutMutation({
		onSuccess: () => {
			toast.success("Signed out", {
				description: "You have been successfully signed out",
				duration: 2000
			});
		},
		onError: (error) =>
			ErrorHandler.handleApiError(error as ApiError, {
				action: "sign_out",
				resource: "user"
			})
	});

	const login = useCallback(
		async (credentials: CredentialsLoginOptions) => {
			setIsLoading(true);
			baseAuth.setError(null);

			try {
				const response = await loginMutation.mutateAsync({
					email: credentials.email,
					password: credentials.password
				});

				if (response.accessToken) {
					localStorage.setItem("accessToken", response.accessToken);

					const result = await signIn("credentials", {
						token: response.accessToken,
						redirect: false
					});

					if (result?.ok) {
						baseAuth.router.push(credentials.callbackUrl || "/");
					} else {
						const sessionError = {
							message: "Failed to create session",
							response: {
								status: 500,
								statusText: "Session Error",
								data: {
									type: "SessionError",
									title: "Session Creation Failed",
									status: 500,
									detail: "Failed to create session"
								}
							}
						} as ApiError;

						ErrorHandler.handleApiError(sessionError, {
							action: "create_session",
							resource: "user"
						});
						baseAuth.setError("Failed to create session");

						// const errorMsg = "Failed to create session";
						// baseAuth.setError(errorMsg);
						// toast.error("Session error", {
						// 	description: errorMsg,
						// 	duration: 5000
						// });
					}
				} else {
					const tokenError = {
						message: "No access token received",
						response: {
							status: 401,
							statusText: "Authentication Error",
							data: {
								type: "AuthenticationError",
								title: "Token Missing",
								status: 401,
								detail: "No access token received"
							}
						}
					} as ApiError;

					ErrorHandler.handleApiError(tokenError, {
						action: "authenticate",
						resource: "user"
					});
					baseAuth.setError("No access token received");

					// const errorMsg = "No access token received";
					// baseAuth.setError(errorMsg);
					// toast.error("Authentication error", {
					// 	description: errorMsg,
					// 	duration: 4000
					// });
				}
			} catch (err: any) {
				if (process.env.NODE_ENV === "development") {
					console.error("Credentials login error:", err);
					console.log("Error structure:", {
						response: err.response,
						status: err.response?.status,
						data: err.response?.data
					});
				}
			} finally {
				setIsLoading(false);
			}
		},
		[baseAuth, loginMutation]
	);

	const logout = useCallback(async () => {
		try {
			// Not implemented on the back-end, just an example
			// Optional: Call server-side logout if needed
			// await logoutMutation.mutateAsync();

			// Client-side cleanup
			localStorage.removeItem("accessToken");
			await signOut({ redirect: false });
			// Success toast (if not using server-side logout)
			toast.success("Signed out", {
				description: "You have been successfully signed out",
				duration: 2000
			});
			baseAuth.router.push("/");
		} catch (error) {
			ErrorHandler.handleApiError(
				error as ApiError,
				{
					action: "client_logout",
					resource: "user"
				},
				{
					customMessages: {
						500: "There was an issue signing out, but you've been logged out locally"
					}
				}
			);

			// console.error("Credentials logout error:", error);
			// baseAuth.handleError(error);
			// toast.error("Logout error", {
			// 	description: "There was an issue signing out",
			// 	duration: 4000
			// });
			throw error;
		}
	}, [baseAuth.router, baseAuth.handleError, logoutMutation]);

	const refreshSession = useCallback(async () => {
		return null;
	}, []);

	return {
		...baseAuth,
		login,
		logout,
		refreshSession,
		isLoading: isLoading || baseAuth.session.isLoading || loginMutation.isPending
	};
};
