import { keepPreviousData, queryOptions } from "@tanstack/react-query";
import { getClients, getLoggedInUser, getUserById, getUsers } from "./get-users";
import { UserDetailQuery, UserListQuery } from "../model";

export const usersQueries = {
	all: () => ["users"] as const,

	lists: () => [...usersQueries.all(), "list"] as const,
	list: (query?: UserListQuery) =>
		queryOptions({
			queryKey: [...usersQueries.lists(), query],
			queryFn: () => getUsers(query),
			placeholderData: keepPreviousData
		}),

	clients: () => [...usersQueries.all(), "clients"] as const,
	clientsList: () =>
		queryOptions({
			queryKey: [...usersQueries.clients()],
			queryFn: () => getClients(),
			placeholderData: keepPreviousData
		}),

	details: () => [...usersQueries.all(), "detail"] as const,
	detail: (query?: UserDetailQuery) =>
		queryOptions({
			queryKey: [...usersQueries.details(), query?.id],
			queryFn: () => getUserById(query?.id!),
			enabled: !!query?.id,
			staleTime: 5 * 60 * 1000 // 5 minutes
		}),

	me: () =>
		queryOptions({
			queryKey: [...usersQueries.all(), "me"],
			queryFn: () => getLoggedInUser(),
			staleTime: 10 * 60 * 1000 // 10 minutes
		})
};
