"use client";

import { FC, useMemo } from "react";
import { useSession } from "next-auth/react";
import { useQuery } from "@tanstack/react-query";

import type { Order } from "@entities/orders/model";
import { calculateIncrease } from "@entities/orders/lib";
import { getAllOrders } from "@entities/orders/api";

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
import { Badge } from "@shared/ui/badge";

import { processWeekData } from "./model";

export const WeekSales: FC = () => {
	// const info = useRenderInfo("WeekSales");
	const { data: session, status } = useSession();
	const userId = (session as any)?.user?.id;
	const accessToken = (session as any)?.accessToken;

	const {
		data: ordersData,
		error: ordersError,
		isPending: isOrdersPending,
		isError: isOrdersError
	} = useQuery({
		queryKey: ["getAllOrders", userId, accessToken],
		queryFn: (): Promise<Array<Order>> => getAllOrders(accessToken),
		enabled: !!userId && !!accessToken
	});

	const { current, previous } = useMemo(
		() => (ordersData ? processWeekData(ordersData) : { current: 0, previous: 0 }),
		[ordersData]
	);

	const weekIncrease = useMemo(
		() => (ordersData ? calculateIncrease(current, previous) : 0),
		[current, previous, ordersData]
	);

	if (isOrdersPending) {
		return (
			<Card x-chunk="A card showing the week sales increase.">
				<Skeleton className="w-full h-[179.77px] rounded-lg" />
			</Card>
		);
	}

	if (isOrdersError) {
		return (
			<Card x-chunk="A card showing the week sales increase.">
				<CardHeader className="pb-2">
					<CardDescription>This Week</CardDescription>
				</CardHeader>
				<CardContent>
					<Badge className="mt-4 text-left px-7 py-1 text-[12px]" variant="destructive">
						Error: {ordersError?.message}
					</Badge>
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
					{weekIncrease <= 0 ? `${weekIncrease.toFixed(2)}%` : `+${weekIncrease.toFixed(2)}%`} from
					last week
				</div>
			</CardContent>
			<CardFooter>
				<Progress value={weekIncrease} aria-label={`${weekIncrease.toFixed(2)}% increase`} />
			</CardFooter>
		</Card>
	);
};
