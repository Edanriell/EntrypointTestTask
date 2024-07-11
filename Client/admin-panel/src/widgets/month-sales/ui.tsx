import { FC } from "react";

import {
	Card,
	CardContent,
	CardDescription,
	CardFooter,
	CardHeader,
	CardTitle
} from "@shared/ui/card";
import { Skeleton } from "@shared/ui/skeleton";
import { Progress } from "@shared/ui/progress";

import { calculateIncrease, processMonthData } from "./model";

type MonthSalesProps = {
	data: any;
	error: any;
	isPending: boolean;
	isError: boolean;
};

export const MonthSales: FC<MonthSalesProps> = ({ data, error, isPending, isError }) => {
	const { current, previous } = processMonthData(data);
	const monthIncrease = calculateIncrease(current, previous);

	if (isPending) {
		return (
			<Card x-chunk="A card showing the month sales increase.">
				<Skeleton className="w-full h-[179.77px] rounded-lg" />
			</Card>
		);
	}

	if (isError) {
		return (
			<Card x-chunk="A card showing the month sales increase.">
				<CardHeader className="pb-2">
					<CardDescription>This Month</CardDescription>
					<CardTitle className="text-4xl">${current}</CardTitle>
				</CardHeader>
				<CardContent>
					<div className="text-1xl font-medium">Error: {error.message}</div>
				</CardContent>
				<CardFooter></CardFooter>
			</Card>
		);
	}

	return (
		<Card x-chunk="A card showing the month sales increase.">
			<CardHeader className="pb-2">
				<CardDescription>This Month</CardDescription>
				<CardTitle className="text-4xl">${current}</CardTitle>
			</CardHeader>
			<CardContent>
				<div className="text-xs text-muted-foreground">
					{`${monthIncrease.toFixed(2)}%`} from last month
				</div>
			</CardContent>
			<CardFooter>
				<Progress value={monthIncrease} aria-label={`${monthIncrease.toFixed(2)}% increase`} />
			</CardFooter>
		</Card>
	);
};
