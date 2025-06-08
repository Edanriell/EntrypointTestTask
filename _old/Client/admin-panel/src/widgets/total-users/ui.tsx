"use client";

import { FC } from "react";
import { Users } from "lucide-react";
import { useSession } from "next-auth/react";
import { useQuery } from "@tanstack/react-query";

import type { User } from "@entities/users/model";
import { getAllUsers } from "@entities/users/api";

import { Card, CardContent, CardHeader, CardTitle } from "@shared/ui/card";
import { Skeleton } from "@shared/ui/skeleton";
import { Badge } from "@shared/ui/badge";
import { formatNumberWithSeparators } from "@shared/lib";

export const TotalUsers: FC = () => {
	// const info = useRenderInfo("TotalUsers");
	const { data: session, status } = useSession();
	const userId = session?.user.id;
	const accessToken = session?.accessToken;

	const {
		data: usersData,
		error: usersError,
		isPending: isUsersPending,
		isError: isUsersError
	} = useQuery({
		queryKey: ["getAllUsers", userId, accessToken],
		queryFn: (): Promise<Array<User>> => getAllUsers(accessToken!),
		enabled: !!userId && !!accessToken
	});

	if (isUsersPending) {
		return (
			<Card x-chunk="A card showing the total users caunt.">
				<Skeleton className="w-full h-[109.8px] rounded-lg" />
			</Card>
		);
	}

	if (isUsersError) {
		return (
			<Card x-chunk="A card showing the total users caunt.">
				<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
					<CardTitle className="text-sm font-medium">Total Users</CardTitle>
					<Users className="h-4 w-4 text-muted-foreground" />
				</CardHeader>
				<CardContent>
					<Badge className="mt-4 text-left px-7 py-1 text-[12px]" variant="destructive">
						Error: {usersError?.message}
					</Badge>
				</CardContent>
			</Card>
		);
	}

	return (
		<Card x-chunk="A card showing the total users caunt.">
			<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
				<CardTitle className="text-sm font-medium">Total Users</CardTitle>
				<Users className="h-4 w-4 text-muted-foreground" />
			</CardHeader>
			<CardContent>
				<div className="text-2xl font-bold">{formatNumberWithSeparators(usersData.length, 3)}</div>
			</CardContent>
		</Card>
	);
};
