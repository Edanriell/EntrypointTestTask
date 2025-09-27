import { useSession } from "next-auth/react";
import { useRouter } from "next/navigation";
import { useCallback, useState } from "react";

import type { AuthSession, User } from "../../model";

export const useBaseAuth = () => {
	const { data: session, status } = useSession();
	const router = useRouter();
	const [error, setError] = useState<string | null>(null);

	const user: User | null = session?.user
		? {
				id: session.user.id || "",
				name: session.user.name || undefined,
				email: session.user.email || undefined,
				image: session.user.image || undefined,
				roles: session.user.roles || [],
				permissions: session.user.permissions || []
			}
		: null;

	const authSession: AuthSession = {
		user: user!,
		error: session?.error || error || undefined,
		isAuthenticated: !!session?.user,
		isLoading: status === "loading"
	};

	const handleError = useCallback((authError: any) => {
		switch (authError) {
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
				setError(`Authentication error: ${authError}`);
		}
	}, []);

	return {
		session: authSession,
		router,
		error,
		setError,
		handleError
	};
};
