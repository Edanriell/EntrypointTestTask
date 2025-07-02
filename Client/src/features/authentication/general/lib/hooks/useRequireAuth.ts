import { useAuth } from "./useAuth";
import type { AuthStrategy } from "@features/authentication/general/model";

export const useRequireAuth = (strategy?: AuthStrategy) => {
	const auth = useAuth(strategy);

	// All the logic is now encapsulated in the specific auth hooks
	return auth;
};
