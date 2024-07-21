import { FC } from "react";
import { DollarSign } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import { getAllOrders } from "@entities/orders";
import { useSession } from "next-auth/react";

import type { Order } from "@entities/orders/model";

import { Card, CardContent, CardHeader, CardTitle } from "@shared/ui/card";
import { Skeleton } from "@shared/ui/skeleton";
import { Badge } from "@shared/ui/badge";

import { calculateTotalRevenue } from "./lib";

export const TotalRevenue: FC = () => {
	// const info = useRenderInfo("TotalRevenue");
	const { data: session, status } = useSession();
	const userId = session?.user.id;
	const accessToken = session?.accessToken;

	const {
		data: ordersData,
		error: ordersError,
		isPending: isOrdersPending,
		isError: isOrdersError
	} = useQuery({
		queryKey: ["getAllOrders", userId, accessToken],
		queryFn: (): Promise<Array<Order>> => getAllOrders(accessToken!),
		enabled: !!userId && !!accessToken
	});

	if (isOrdersPending) {
		return (
			<Card x-chunk="A card showing the total revenue in USD.">
				<Skeleton className="w-full h-[109.8px] rounded-lg" />
			</Card>
		);
	}

	if (isOrdersError) {
		return (
			<Card x-chunk="A card showing the total revenue in USD.">
				<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
					<CardTitle className="text-sm font-medium">Total Revenue</CardTitle>
					<DollarSign className="h-4 w-4 text-muted-foreground" />
				</CardHeader>
				<CardContent className="flex items-center justify-center">
					<Badge className="mt-4 text-left px-7 py-1 text-[12px]" variant="destructive">
						Error: {ordersError?.message}
					</Badge>
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
				<div className="text-2xl font-bold">${calculateTotalRevenue(ordersData)}</div>
			</CardContent>
		</Card>
	);
};
