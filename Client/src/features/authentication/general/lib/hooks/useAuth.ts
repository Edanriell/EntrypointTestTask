import type { AuthStrategy } from "../../model";

import { useCredentialsAuth, useKeycloakAuth } from "../hooks";

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
