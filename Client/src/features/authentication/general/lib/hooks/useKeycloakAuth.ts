import { signIn, signOut } from "next-auth/react";
import { useCallback } from "react";

import { useBaseAuth } from "@features/authentication/general/lib/hooks";
import type { AuthProvider } from "@features/authentication/general/model";

export const useKeycloakAuth = (): AuthProvider & ReturnType<typeof useBaseAuth> => {
	const baseAuth = useBaseAuth();

	const login = useCallback(
		async (options?: { callbackUrl?: string }) => {
			try {
				await signIn("keycloak", {
					callbackUrl: options?.callbackUrl || "/"
				});
			} catch (error) {
				console.error("Keycloak login error:", error);
				baseAuth.handleError(error);
				throw error;
			}
		},
		[baseAuth]
	);

	const logout = useCallback(async () => {
		try {
			await signOut({ redirect: false });
			baseAuth.router.push("/");
		} catch (error) {
			console.error("Keycloak logout error:", error);
			baseAuth.handleError(error);
			throw error;
		}
	}, [baseAuth.router, baseAuth.handleError]);

	const refreshSession = useCallback(async () => {
		// Keycloak token refresh is handled automatically by NextAuth
		return null;
	}, []);

	return {
		...baseAuth,
		login,
		logout,
		refreshSession
	};
};