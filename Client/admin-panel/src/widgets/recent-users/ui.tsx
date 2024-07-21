"use client";

import { FC } from "react";
import { useSession } from "next-auth/react";
import { useQuery } from "@tanstack/react-query";

import type { User } from "@entities/users/model";
import { getRecentUsers } from "@entities/users/api";

import { Card, CardContent, CardHeader, CardTitle } from "@shared/ui/card";
import { Skeleton } from "@shared/ui/skeleton";
import { ScrollArea } from "@shared/ui/scroll-area";
import { Avatar, AvatarFallback, AvatarImage } from "@shared/ui/avatar";
import { Badge } from "@shared/ui/badge";

export const RecentUsers: FC = () => {
	const { data: session, status } = useSession();
	const userId = session?.user.id;
	const accessToken = session?.accessToken;

	const {
		data: recentUsersData,
		error: recentUsersError,
		isPending: isRecentUsersPending,
		isError: isRecentUsersError
	} = useQuery({
		queryKey: ["recentUsers", userId, accessToken],
		queryFn: (): Promise<Array<User>> => getRecentUsers(accessToken!),
		enabled: !!userId && !!accessToken
	});

	if (isRecentUsersPending) {
		return (
			<Card x-chunk="A card showing a list of recent registered users.">
				<Skeleton className="w-full h-[537.74px] md:h-[full] rounded-lg" />
			</Card>
		);
	}

	if (isRecentUsersError) {
		return (
			<Card x-chunk="A card showing a list of recent registered users.">
				<CardHeader>
					<CardTitle>Recent Users</CardTitle>
				</CardHeader>
				<CardContent className="grid gap-8">
					<Badge
						className="mt-10 px-9 py-2 text-[14px] text-center block max-w-max mr-auto ml-auto"
						variant="destructive"
					>
						Error: {recentUsersError?.message}
					</Badge>
				</CardContent>
			</Card>
		);
	}

	return (
		<Card x-chunk="A card showing a list of recent registered users.">
			<CardHeader>
				<CardTitle>Recent Users</CardTitle>
			</CardHeader>
			<CardContent className="grid gap-8">
				<ScrollArea className="h-[440px] w-full rounded-lg">
					{recentUsersData?.map((user) => {
						return (
							<div key={user.id} className="flex items-center gap-4 mb-[32px]">
								<Avatar className="h-9 w-9 sm:flex">
									<AvatarImage
										src={user?.photo ? "data:image/jpeg;base64," + user?.photo : "#"}
										alt={user?.name + " " + user?.surname + " " + "avatar"}
									/>
									<AvatarFallback>
										{user?.name[0]}
										{user?.surname[0]}
									</AvatarFallback>
								</Avatar>
								<div className="grid gap-1">
									<p className="text-sm font-medium leading-none">
										{user.name} {user.surname}
									</p>
									<p className="text-sm text-muted-foreground">{user.email}</p>
								</div>
							</div>
						);
					})}
				</ScrollArea>
			</CardContent>
		</Card>
	);
};
