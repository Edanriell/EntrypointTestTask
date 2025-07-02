// import { signIn, signOut } from "next-auth/react";
// import { useCallback, useState } from "react";
//
// import { useBaseAuth } from "@features/authentication/general/lib/hooks";
// import type { AuthProvider } from "@features/authentication/general/model";
//
// // Import your existing login mutation
// import { useLoginUser } from "@entities/users"; // Adjust import path
//
// type CredentialsLoginOptions = {
// 	email: string;
// 	password: string;
// 	callbackUrl?: string;
// };
//
// export const useCredentialsAuth = (): AuthProvider &
// 	ReturnType<typeof useBaseAuth> & {
// 		isLoading: boolean;
// 	} => {
// 	const baseAuth = useBaseAuth();
// 	const [isLoading, setIsLoading] = useState(false);
// 	const loginUserMutation = useLoginUser();
//
// 	const login = useCallback(
// 		async (credentials: CredentialsLoginOptions) => {
// 			setIsLoading(true);
// 			baseAuth.setError(null);
//
// 			try {
// 				// First, authenticate with your backend
// 				const response = await loginUserMutation.mutateAsync({
// 					email: credentials.email,
// 					password: credentials.password
// 				});
//
// 				if (response.accessToken) {
// 					// Store the token securely
// 					localStorage.setItem("accessToken", response.accessToken);
//
// 					// Create a session with NextAuth using credentials provider
// 					const result = await signIn("credentials", {
// 						token: response.accessToken,
// 						redirect: false
// 					});
//
// 					if (result?.ok) {
// 						baseAuth.router.push(credentials.callbackUrl || "/");
// 					} else {
// 						baseAuth.setError("Failed to create session");
// 					}
// 				} else {
// 					baseAuth.setError("No access token received");
// 				}
// 			} catch (err: any) {
// 				console.error("Credentials login error:", err);
//
// 				if (err.response?.status === 401) {
// 					baseAuth.setError("Invalid email or password");
// 				} else if (err.response?.status === 400) {
// 					baseAuth.setError("Please check your email and password");
// 				} else {
// 					baseAuth.setError(err.message || "Login failed. Please try again.");
// 				}
// 				throw err;
// 			} finally {
// 				setIsLoading(false);
// 			}
// 		},
// 		[baseAuth, loginUserMutation]
// 	);
//
// 	const logout = useCallback(async () => {
// 		try {
// 			// Clear local storage
// 			localStorage.removeItem("accessToken");
//
// 			await signOut({ redirect: false });
// 			baseAuth.router.push("/");
// 		} catch (error) {
// 			console.error("Credentials logout error:", error);
// 			baseAuth.handleError(error);
// 			throw error;
// 		}
// 	}, [baseAuth.router, baseAuth.handleError]);
//
// 	const refreshSession = useCallback(async () => {
// 		// For credentials, you might need to implement your own refresh logic
// 		// or return null if tokens don't expire
// 		return null;
// 	}, []);
//
// 	return {
// 		...baseAuth,
// 		login,
// 		logout,
// 		refreshSession,
// 		isLoading: isLoading || baseAuth.session.isLoading
// 	};
// };

// Why do we use query here ?
// And why we dont use query on logout, only on login ?
// Api layer for login ?

// import { signIn, signOut } from "next-auth/react";
// import { useCallback, useState } from "react";
// import { useMutation } from "@tanstack/react-query";
//
// import { useBaseAuth } from "@features/authentication/general/lib/hooks";
// import { apiClient } from "@shared/api";
// import type {
// 	AuthProvider,
// 	LoginUserRequest,
// 	LoginUserResponse
// } from "@features/authentication/general/model";
//
// type CredentialsLoginOptions = {
// 	email: string;
// 	password: string;
// 	callbackUrl?: string;
// };
//
// export const useCredentialsAuth = (): AuthProvider &
// 	ReturnType<typeof useBaseAuth> & {
// 		isLoading: boolean;
// 	} => {
// 	const baseAuth = useBaseAuth();
// 	const [isLoading, setIsLoading] = useState(false);
//
// 	// Type the mutation properly with LoginUserResponse
// 	const loginMutation = useMutation<LoginUserResponse, Error, LoginUserRequest>({
// 		mutationFn: async (credentials: LoginUserRequest): Promise<LoginUserResponse> => {
// 			return apiClient.post<LoginUserResponse>("/auth/login", credentials);
// 		},
// 		onError: (error: any) => {
// 			if (error.response?.status === 401) {
// 				baseAuth.setError("Invalid email or password");
// 			} else if (error.response?.status === 400) {
// 				baseAuth.setError("Please check your email and password");
// 			} else {
// 				baseAuth.setError(error.message || "Login failed. Please try again.");
// 			}
// 		}
// 	});
//
// 	const login = useCallback(
// 		async (credentials: CredentialsLoginOptions) => {
// 			setIsLoading(true);
// 			baseAuth.setError(null);
//
// 			try {
// 				const response = await loginMutation.mutateAsync({
// 					email: credentials.email,
// 					password: credentials.password
// 				});
//
// 				// Now response is properly typed as LoginUserResponse
// 				if (response.accessToken) {
// 					localStorage.setItem("accessToken", response.accessToken);
//
// 					const result = await signIn("credentials", {
// 						token: response.accessToken,
// 						redirect: false
// 					});
//
// 					if (result?.ok) {
// 						baseAuth.router.push(credentials.callbackUrl || "/");
// 					} else {
// 						baseAuth.setError("Failed to create session");
// 					}
// 				} else {
// 					baseAuth.setError("No access token received");
// 				}
// 			} catch (err: any) {
// 				console.error("Credentials login error:", err);
// 				throw err;
// 			} finally {
// 				setIsLoading(false);
// 			}
// 		},
// 		[baseAuth, loginMutation]
// 	);
//
// 	const logout = useCallback(async () => {
// 		try {
// 			localStorage.removeItem("accessToken");
// 			await signOut({ redirect: false });
// 			baseAuth.router.push("/");
// 		} catch (error) {
// 			console.error("Credentials logout error:", error);
// 			baseAuth.handleError(error);
// 			throw error;
// 		}
// 	}, [baseAuth.router, baseAuth.handleError]);
//
// 	const refreshSession = useCallback(async () => {
// 		return null;
// 	}, []);
//
// 	return {
// 		...baseAuth,
// 		login,
// 		logout,
// 		refreshSession,
// 		isLoading: isLoading || baseAuth.session.isLoading || loginMutation.isPending
// 	};
// };

import { signIn, signOut } from "next-auth/react";
import { useCallback, useState } from "react";
import { useBaseAuth } from "@features/authentication/general/lib/hooks/useBaseAuth";
import { AuthProvider } from "@features/authentication/general/model";
import {
	useLoginMutation,
	useLogoutMutation
} from "@features/authentication/general/api/mutations";

type CredentialsLoginOptions = {
	email: string;
	password: string;
	callbackUrl?: string;
};

export const useCredentialsAuth = (): AuthProvider &
	ReturnType<typeof useBaseAuth> & {
		isLoading: boolean;
	} => {
	const baseAuth = useBaseAuth();
	const [isLoading, setIsLoading] = useState(false);

	const loginMutation = useLoginMutation({
		onError: (error: any) => {
			if (error.response?.status === 401) {
				baseAuth.setError("Invalid email or password");
			} else if (error.response?.status === 400) {
				baseAuth.setError("Please check your email and password");
			} else {
				baseAuth.setError(error.message || "Login failed. Please try again.");
			}
		}
	});

	// Not implemented on the back-end, just an example
	const logoutMutation = useLogoutMutation({
		onError: (error) => {
			console.error("Server logout error:", error);
			// Continue with client-side logout even if server fails
		}
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
						baseAuth.setError("Failed to create session");
					}
				} else {
					baseAuth.setError("No access token received");
				}
			} catch (err: any) {
				console.error("Credentials login error:", err);
				throw err;
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
			baseAuth.router.push("/");
		} catch (error) {
			console.error("Credentials logout error:", error);
			baseAuth.handleError(error);
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
