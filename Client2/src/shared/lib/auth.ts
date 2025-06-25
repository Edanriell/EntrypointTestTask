import { getSession, signIn, signOut, useSession } from "next-auth/react";
import { useRouter } from "next/navigation";
import { useCallback } from "react";

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
	const { isAuthenticated, isLoading } = useAuth();
	const router = useRouter();

	if (!isLoading && !isAuthenticated) {
		router.push("/auth/signin");
	}

	return { isAuthenticated, isLoading };
};
