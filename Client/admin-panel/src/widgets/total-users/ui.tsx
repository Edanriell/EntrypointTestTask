import { FC, memo, useMemo } from "react";
import { Users } from "lucide-react";

import type { User } from "@entities/users/model";

import { Card, CardContent, CardHeader, CardTitle } from "@shared/ui/card";
import { Skeleton } from "@shared/ui/skeleton";
import { Badge } from "@shared/ui/badge";
import { formatNumberWithSeparators } from "@shared/lib";

type TotalUsersProps = {
	data?: Array<User>;
	error: Error | null;
	isPending: boolean;
	isError: boolean;
};

const TotalUsers: FC<TotalUsersProps> = ({ data, error, isPending, isError }) => {
	// const info = useRenderInfo("TotalUsers");
	const totalUsers = useMemo(
		() => (data ? formatNumberWithSeparators(data?.length, 3) : 0),
		[data]
	);

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
					<Badge className="mt-4 text-left px-7 py-1 text-[12px]" variant="destructive">
						Error: {error?.message}
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
				<div className="text-2xl font-bold">{totalUsers}</div>
			</CardContent>
		</Card>
	);
};

export const MemoizedTotalUsers = memo(TotalUsers);
