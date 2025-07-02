import { useCredentialsAuth, useKeycloakAuth } from "@features/authentication/general/lib/hooks";
import type { AuthStrategy } from "@features/authentication/general/model";

export const useAuth = (strategy: AuthStrategy = "keycloak") => {
	switch (strategy) {
		case "keycloak":
			return useKeycloakAuth();
		case "credentials":
			return useCredentialsAuth();
		default:
			return useKeycloakAuth();
	}
};

// Convenience hooks for specific strategies
// export { useKeycloakAuth, useCredentialsAuth };
