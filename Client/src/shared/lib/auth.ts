// import { useCallback, useEffect, useState } from "react";
// import { useRouter } from "next/navigation";
// import { getSession, signIn, signOut, useSession } from "next-auth/react";
// import { useLoginUser } from "@entities/users";
//
// // export const useAuth = () => {
// // 	const { data: session, status } = useSession();
// // 	const router = useRouter();
// //
// // 	const login = useCallback(async (provider = "keycloak") => {
// // 		try {
// // 			await signIn(provider);
// // 		} catch (error) {
// // 			console.error("Login error:", error);
// // 			throw error;
// // 		}
// // 	}, []);
// //
// // 	const logout = useCallback(async () => {
// // 		try {
// // 			await signOut({ redirect: false });
// // 			router.push("/");
// // 		} catch (error) {
// // 			console.error("Logout error:", error);
// // 			throw error;
// // 		}
// // 	}, [router]);
// //
// // 	const refreshSession = useCallback(async () => {
// // 		return await getSession();
// // 	}, []);
// //
// // 	return {
// // 		user: session?.user,
// // 		accessToken: session?.accessToken,
// // 		refreshToken: session?.refreshToken,
// // 		isAuthenticated: !!session,
// // 		isLoading: status === "loading",
// // 		login,
// // 		logout,
// // 		refreshSession,
// // 		session
// // 	};
// // };
//
// // WORKS
// export const useAuth = () => {
// 	const { data: session, status } = useSession();
// 	const router = useRouter();
//
// 	const login = useCallback(async (provider = "keycloak", callbackUrl = "/") => {
// 		try {
// 			await signIn(provider, { callbackUrl });
// 		} catch (error) {
// 			console.error("Login error:", error);
// 			throw error;
// 		}
// 	}, []);
//
// 	const logout = useCallback(async () => {
// 		try {
// 			await signOut({ redirect: false });
// 			router.push("/");
// 		} catch (error) {
// 			console.error("Logout error:", error);
// 			throw error;
// 		}
// 	}, [router]);
//
// 	const refreshSession = useCallback(async () => {
// 		return await getSession();
// 	}, []);
//
// 	return {
// 		user: session?.user,
// 		isAuthenticated: !!session?.user,
// 		isLoading: status === "loading",
// 		login,
// 		logout,
// 		refreshSession,
// 		session
// 	};
// };
//
// // export const useRequireAuth = () => {
// // 	const {
// // 		user,
// // 		isAuthenticated,
// // 		isLoading,
// // 		session,
// // 		accessToken,
// // 		refreshToken,
// // 		login,
// // 		logout,
// // 		refreshSession
// // 	} = useAuth();
// //
// // 	const [error, setError] = useState<string | null>(null);
// //
// // 	useEffect(() => {
// // 		// Clear any previous errors when auth state changes
// // 		setError(null);
// //
// // 		// Handle authentication errors from session
// // 		if (session?.error) {
// // 			switch (session.error) {
// // 				case "RefreshAccessTokenError":
// // 					setError("Your session has expired. Please sign in again.");
// // 					break;
// // 				case "AccessDenied":
// // 					setError("Access denied. You don't have permission to access this resource.");
// // 					break;
// // 				case "InvalidToken":
// // 					setError("Invalid authentication token. Please sign in again.");
// // 					break;
// // 				default:
// // 					setError(`Authentication error: ${session.error}`);
// // 			}
// // 		}
// // 	}, [session?.error]);
// //
// // 	return {
// // 		user,
// // 		isAuthenticated,
// // 		isLoading,
// // 		error,
// // 		session,
// // 		accessToken,
// // 		refreshToken,
// // 		login,
// // 		logout,
// // 		refreshSession
// // 	};
// // };
//
// export const useRequireAuth = () => {
// 	const { user, isAuthenticated, isLoading, session, login, logout, refreshSession } = useAuth();
// 	const [error, setError] = useState<string | null>(null);
//
// 	useEffect(() => {
// 		// Clear any previous errors when auth state changes
// 		setError(null);
//
// 		// Handle authentication errors from session
// 		if (session?.error) {
// 			switch (session.error) {
// 				case "RefreshAccessTokenError":
// 					setError("Your session has expired. Please sign in again.");
// 					break;
// 				case "AccessDenied":
// 					setError("Access denied. You don't have permission to access this resource.");
// 					break;
// 				case "InvalidToken":
// 					setError("Invalid authentication token. Please sign in again.");
// 					break;
// 				default:
// 					setError(`Authentication error: ${session.error}`);
// 			}
// 		}
// 	}, [session?.error]);
//
// 	return {
// 		user,
// 		isAuthenticated,
// 		isLoading,
// 		error,
// 		session,
// 		login,
// 		logout,
// 		refreshSession
// 	};
// };
//
// export const useDirectAuth = () => {
// 	const [isLoading, setIsLoading] = useState(false);
// 	const [error, setError] = useState<string | null>(null);
// 	const router = useRouter();
//
// 	const loginUserMutation = useLoginUser();
//
// 	const directLogin = async (credentials: { email: string; password: string }) => {
// 		setIsLoading(true);
// 		setError(null);
//
// 		try {
// 			// First, authenticate with your backend
// 			const response = await loginUserMutation.mutateAsync(credentials);
//
// 			if (response.accessToken) {
// 				// Store the token securely
// 				localStorage.setItem("accessToken", response.accessToken);
//
// 				// Create a session with NextAuth using credentials provider
// 				const result = await signIn("credentials", {
// 					token: response.accessToken,
// 					redirect: false
// 				});
//
// 				if (result?.ok) {
// 					router.push("/");
// 				} else {
// 					setError("Failed to create session");
// 				}
// 			} else {
// 				setError("No access token received");
// 			}
// 		} catch (err: any) {
// 			console.error("Login error:", err);
// 			// Handle different types of errors
// 			if (err.response?.status === 401) {
// 				setError("Invalid email or password");
// 			} else if (err.response?.status === 400) {
// 				setError("Please check your email and password");
// 			} else {
// 				setError(err.message || "Login failed. Please try again.");
// 			}
// 		} finally {
// 			setIsLoading(false);
// 		}
// 	};
//
// 	return {
// 		directLogin,
// 		isLoading,
// 		error
// 	};
// };

// TODO
// Can remove
