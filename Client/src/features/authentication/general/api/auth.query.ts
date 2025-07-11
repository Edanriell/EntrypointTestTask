import { queryOptions } from "@tanstack/react-query";

import { getCurrentSession } from "@features/authentication/general";

export const authQueries = {
	all: () => ["auth"] as const,

	session: () => [...authQueries.all(), "session"] as const,
	sessionInfo: () =>
		queryOptions({
			queryKey: [...authQueries.session()],
			queryFn: () => getCurrentSession(),
			staleTime: 5 * 60 * 1000, // 5 minutes
			retry: false,
			refetchOnMount: false,
			refetchIntervalInBackground: false
		})
};
