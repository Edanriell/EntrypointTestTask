import type { AuthStrategy } from "@features/authentication/general/model";

import { useAuth } from "./useAuth";

export const useRequireAuth = (strategy?: AuthStrategy) => {
	const auth = useAuth(strategy);

	// All the logic is now encapsulated in the specific auth hooks
	return auth;
};
