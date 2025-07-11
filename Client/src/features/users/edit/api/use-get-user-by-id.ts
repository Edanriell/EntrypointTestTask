import { useQuery } from "@tanstack/react-query";

import { usersQueries } from "@entities/users";

export const useGetUserById = (userId: string) => {
	return useQuery(usersQueries.customerDetail({ id: userId }));
};
