import { FC } from "react";

import type { User } from "@entities/users/model";

import { Card, CardContent, CardHeader, CardTitle } from "@shared/ui/card";
import { Skeleton } from "@shared/ui/skeleton";
import { ScrollArea } from "@shared/ui/scroll-area";
import { Avatar, AvatarFallback, AvatarImage } from "@shared/ui/avatar";
import { Badge } from "@shared/ui/badge";

type RecentUsersProps = {
	data: Array<User>;
	error: Error | null;
	isPending: boolean;
	isError: boolean;
};

export const RecentUsers: FC<RecentUsersProps> = ({ data, error, isPending, isError }) => {
	if (isPending) {
		return (
			<Card x-chunk="A card showing a list of recent registered users.">
				<Skeleton className="w-full h-[537.74px] md:h-[full] rounded-lg" />
			</Card>
		);
	}

	if (isError) {
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
						Error: {error?.message}
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
					{data.map((user) => {
						return (
							<div key={user.id} className="flex items-center gap-4 mb-[32px]">
								<Avatar className="hidden h-9 w-9 sm:flex">
									<AvatarImage src={user.photo ? user.photo : "#"} alt="Avatar" />
									<AvatarFallback>
										{user.name[0]}
										{user.surname[0]}
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

// TODO
// Figure out how to add image to user and how to use it on frontend
// TODO
