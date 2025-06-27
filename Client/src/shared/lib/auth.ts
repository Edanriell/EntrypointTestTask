import { useCallback, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { getSession, signIn, signOut, useSession } from "next-auth/react";

export const useAuth = () => {
	const { data: session, status } = useSession();
	const router = useRouter();

	const login = useCallback(async (provider = "keycloak") => {
		try {
			await signIn(provider);
		} catch (error) {
			console.error("Login error:", error);
			throw error;
		}
	}, []);

	const logout = useCallback(async () => {
		try {
			await signOut({ redirect: false });
			router.push("/");
		} catch (error) {
			console.error("Logout error:", error);
			throw error;
		}
	}, [router]);

	const refreshSession = useCallback(async () => {
		return await getSession();
	}, []);

	return {
		user: session?.user,
		accessToken: session?.accessToken,
		refreshToken: session?.refreshToken,
		isAuthenticated: !!session,
		isLoading: status === "loading",
		login,
		logout,
		refreshSession,
		session
	};
};

export const useRequireAuth = () => {
	const {
		user,
		isAuthenticated,
		isLoading,
		session,
		accessToken,
		refreshToken,
		login,
		logout,
		refreshSession
	} = useAuth();

	const [error, setError] = useState<string | null>(null);

	useEffect(() => {
		// Clear any previous errors when auth state changes
		setError(null);

		// Handle authentication errors from session
		if (session?.error) {
			switch (session.error) {
				case "RefreshAccessTokenError":
					setError("Your session has expired. Please sign in again.");
					break;
				case "AccessDenied":
					setError("Access denied. You don't have permission to access this resource.");
					break;
				case "InvalidToken":
					setError("Invalid authentication token. Please sign in again.");
					break;
				default:
					setError(`Authentication error: ${session.error}`);
			}
		}
	}, [session?.error]);

	return {
		user,
		isAuthenticated,
		isLoading,
		error,
		session,
		accessToken,
		refreshToken,
		login,
		logout,
		refreshSession
	};
};
