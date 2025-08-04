import { useQuery } from "@tanstack/react-query";

import { usersQueries } from "@entities/users";

export const useGetCustomerById = (customerId: string) => {
	return useQuery(usersQueries.customerDetail({ userId: customerId }));
};
