import { useRequireAuth } from "@features/authentication/general/lib/hooks";
import type { AuthStrategy } from "@features/authentication/general/model";

export const useAuthUser = (strategy?: AuthStrategy) => {
	const auth = useRequireAuth(strategy);

	return {
		...auth,
		user: auth.session.user
	};
};
