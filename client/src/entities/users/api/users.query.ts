import { keepPreviousData, queryOptions } from "@tanstack/react-query";
import { getClients, getLoggedInUser, getUserById, getUsers } from "./get-users";
import { UserDetailQuery, UserListQuery } from "../model";

export const usersQueries = {
	all: () => ["users"] as const,

	lists: () => [...usersQueries.all(), "list"] as const,
	list: (params?: UserListQuery) =>
		queryOptions({
			queryKey: [...usersQueries.lists(), params],
			queryFn: () => getUsers(params),
			placeholderData: keepPreviousData,
			staleTime: 30000 // 30 seconds
		}),

	details: () => [...usersQueries.all(), "detail"] as const,
	detail: (query: UserDetailQuery) =>
		queryOptions({
			queryKey: [...usersQueries.details(), query.id],
			queryFn: () => getUserById(query.id),
			staleTime: 5000
		}),

	me: () =>
		queryOptions({
			queryKey: [...usersQueries.all(), "me"],
			queryFn: getLoggedInUser,
			staleTime: 60000 // 1 minute
		}),

	clients: () =>
		queryOptions({
			queryKey: [...usersQueries.all(), "clients"],
			queryFn: getClients,
			staleTime: 30000 // 30 seconds
		})
};
