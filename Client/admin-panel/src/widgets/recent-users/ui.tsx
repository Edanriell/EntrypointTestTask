import { FC } from "react";

import {
	Avatar,
	AvatarFallback,
	AvatarImage,
	Card,
	CardContent,
	CardHeader,
	CardTitle,
	ScrollArea,
	Skeleton
} from "@shared/ui";

type RecentUsersProps = {
	data: any;
	error: any;
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
					<div className="text-1xl font-bold text-left mt-[40px]">Error: {error.message}</div>
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
					{data.map((user: any) => {
						return (
							<div key={user.id} className="flex items-center gap-4 mb-[32px]">
								<Avatar className="hidden h-9 w-9 sm:flex">
									<AvatarImage src="/avatars/01.png" alt="Avatar" />
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
