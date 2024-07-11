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

import { calculateIncrease, processWeekData } from "./model";

type WeekSalesProps = {
	data: any;
	error: any;
	isPending: boolean;
	isError: boolean;
};

export const WeekSales: FC<WeekSalesProps> = ({ data, error, isPending, isError }) => {
	const { current, previous } = processWeekData(data);
	const weekIncrease = calculateIncrease(current, previous);

	if (isPending) {
		return (
			<Card x-chunk="A card showing the week sales increase.">
				<Skeleton className="w-full h-[179.77px] rounded-lg" />
			</Card>
		);
	}

	if (isError) {
		return (
			<Card x-chunk="A card showing the week sales increase.">
				<CardHeader className="pb-2">
					<CardDescription>This Week</CardDescription>
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
		<Card x-chunk="A card showing the week sales increase.">
			<CardHeader className="pb-2">
				<CardDescription>This Week</CardDescription>
				<CardTitle className="text-4xl">${current}</CardTitle>
			</CardHeader>
			<CardContent>
				<div className="text-xs text-muted-foreground">
					{`${weekIncrease.toFixed(2)}%`} from last week
				</div>
			</CardContent>
			<CardFooter>
				<Progress value={weekIncrease} aria-label={`${weekIncrease.toFixed(2)}% increase`} />
			</CardFooter>
		</Card>
	);
};
