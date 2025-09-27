import type { AuthStrategy } from "../../model";

import { useRequireAuth } from "../hooks";

export const useAuthUser = (strategy?: AuthStrategy) => {
	const auth = useRequireAuth(strategy);

	return {
		...auth,
		user: auth.session.user
	};
};
