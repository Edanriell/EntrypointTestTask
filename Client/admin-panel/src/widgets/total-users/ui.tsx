import { FC } from "react";
import { Users } from "lucide-react";

import { Card, CardContent, CardHeader, CardTitle, Skeleton } from "@shared/ui";

type TotalUsersProps = {
	data: any;
	error: any;
	isPending: boolean;
	isError: boolean;
};

export const TotalUsers: FC<TotalUsersProps> = ({ data, error, isPending, isError }) => {
	if (isPending) {
		return (
			<Card x-chunk="A card showing the total users caunt.">
				<Skeleton className="w-full h-[109.8px] rounded-lg" />
			</Card>
		);
	}

	if (isError) {
		return (
			<Card x-chunk="A card showing the total users caunt.">
				<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
					<CardTitle className="text-sm font-medium">Total Users</CardTitle>
					<Users className="h-4 w-4 text-muted-foreground" />
				</CardHeader>
				<CardContent>
					<div className="text-1xl font-bold">Error: {error.message}</div>
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
				<div className="text-2xl font-bold">{data.length}</div>
			</CardContent>
		</Card>
	);
};
