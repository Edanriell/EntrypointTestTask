"use client";

import { FC } from "react";
import { CreditCard } from "lucide-react";
import { useSession } from "next-auth/react";
import { useQuery } from "@tanstack/react-query";

import type { Order } from "@entities/orders/model";
import { getAllOrders } from "@entities/orders/api";

import { formatNumberWithSeparators } from "@shared/lib";
import { Card, CardContent, CardHeader, CardTitle } from "@shared/ui/card";
import { Skeleton } from "@shared/ui/skeleton";
import { Badge } from "@shared/ui/badge";

export const TotalOrders: FC = () => {
	// const info = useRenderInfo("TotalOrders");
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
			<Card x-chunk="A card showing the total orders count.">
				<Skeleton className="w-full h-[109.8px] rounded-lg" />
			</Card>
		);
	}

	if (isOrdersError) {
		return (
			<Card x-chunk="A card showing the total orders count.">
				<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
					<CardTitle className="text-sm font-medium">Total Orders</CardTitle>
					<CreditCard className="h-4 w-4 text-muted-foreground" />
				</CardHeader>
				<CardContent>
					<Badge className="mt-4 text-left px-7 py-1 text-[12px]" variant="destructive">
						Error: {ordersError?.message}
					</Badge>
				</CardContent>
			</Card>
		);
	}

	return (
		<Card x-chunk="A card showing the total orders count.">
			<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
				<CardTitle className="text-sm font-medium">Total Orders</CardTitle>
				<CreditCard className="h-4 w-4 text-muted-foreground" />
			</CardHeader>
			<CardContent>
				<div className="text-2xl font-bold">{formatNumberWithSeparators(ordersData.length, 3)}</div>
			</CardContent>
		</Card>
	);
};
