import { FC } from "react";
import { DollarSign } from "lucide-react";

import { Card, CardContent, CardHeader, CardTitle, Skeleton } from "@shared/ui";

import { calculateTotalRevenue } from "./lib";

type TotalRevenueProps = {
	data: any;
	error: any;
	isPending: boolean;
	isError: boolean;
};

export const TotalRevenue: FC<TotalRevenueProps> = ({ data, error, isPending, isError }) => {
	if (isPending) {
		return (
			<Card x-chunk="A card showing the total revenue in USD.">
				<Skeleton className="w-full h-[109.8px] rounded-lg" />
			</Card>
		);
	}

	if (isError) {
		return (
			<Card x-chunk="A card showing the total revenue in USD.">
				<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
					<CardTitle className="text-sm font-medium">Total Revenue</CardTitle>
					<DollarSign className="h-4 w-4 text-muted-foreground" />
				</CardHeader>
				<CardContent>
					<div className="text-1xl font-bold">Error: {error.message}</div>
				</CardContent>
			</Card>
		);
	}

	return (
		<Card x-chunk="A card showing the total revenue in USD.">
			<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
				<CardTitle className="text-sm font-medium">Total Revenue</CardTitle>
				<DollarSign className="h-4 w-4 text-muted-foreground" />
			</CardHeader>
			<CardContent>
				<div className="text-2xl font-bold">${calculateTotalRevenue(data)}</div>
			</CardContent>
		</Card>
	);
};
