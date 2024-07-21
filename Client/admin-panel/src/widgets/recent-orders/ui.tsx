"use client";

import { FC } from "react";
import Link from "next/link";
import { ArrowUpRight } from "lucide-react";
import { useSession } from "next-auth/react";
import { useQuery } from "@tanstack/react-query";

import type { Order } from "@entities/orders/model";
import { getRecentOrders } from "@entities/orders/api";
import { OrderRowMinimal } from "@entities/orders/ui";

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@shared/ui/card";
import { Skeleton } from "@shared/ui/skeleton";
import { Button } from "@shared/ui/button";
import { Table, TableBody, TableHead, TableHeader, TableRow } from "@shared/ui/table";
import { Badge } from "@shared/ui/badge";

export const RecentOrders: FC = () => {
	const { data: session, status } = useSession();
	const userId = session?.user.id;
	const accessToken = session?.accessToken;

	const {
		data: recentOrdersData,
		error: recentOrdersError,
		isPending: isRecentOrdersPending,
		isError: isRecentOrdersError
	} = useQuery({
		queryKey: ["recentOrders", userId, accessToken],
		queryFn: (): Promise<Array<Order>> => getRecentOrders(accessToken!),
		enabled: !!userId && !!accessToken
	});

	if (isRecentOrdersPending) {
		return (
			<Card className="xl:col-span-2" x-chunk="A card showing a table of recent orders.">
				<Skeleton className="w-full h-[537.74px] md:h-[full] rounded-lg" />
			</Card>
		);
	}

	if (isRecentOrdersError) {
		return (
			<Card className="xl:col-span-2 relative" x-chunk="A card showing a table of recent orders.">
				<CardHeader className="flex flex-row items-center">
					<div className="grid gap-2">
						<CardTitle>Orders</CardTitle>
						<CardDescription>Recent orders from store.</CardDescription>
					</div>
					<Button asChild size="sm" className="ml-auto gap-1">
						<Link href="/dashboard/orders">
							View All
							<ArrowUpRight className="h-4 w-4" />
						</Link>
					</Button>
				</CardHeader>
				<CardContent>
					<Table>
						<TableHeader>
							<TableRow>
								<TableHead>Customer</TableHead>
								<TableHead>Date</TableHead>
								<TableHead className="text-right">Amount</TableHead>
							</TableRow>
						</TableHeader>
						<TableBody></TableBody>
					</Table>
					<Badge
						className="mt-10 px-9 py-2 text-[16px] text-center block max-w-max mr-auto ml-auto"
						variant="destructive"
					>
						Error: {recentOrdersError?.message}
					</Badge>
				</CardContent>
			</Card>
		);
	}

	return (
		<Card className="xl:col-span-2" x-chunk="A card showing a table of recent orders.">
			<CardHeader className="flex flex-row items-center">
				<div className="grid gap-2">
					<CardTitle>Orders</CardTitle>
					<CardDescription>Recent orders from store.</CardDescription>
				</div>
				<Button asChild size="sm" className="ml-auto gap-1">
					<Link href="/dashboard/orders">
						View All
						<ArrowUpRight className="h-4 w-4" />
					</Link>
				</Button>
			</CardHeader>
			<CardContent>
				<Table>
					<TableHeader>
						<TableRow>
							<TableHead>Customer</TableHead>
							<TableHead>Date</TableHead>
							<TableHead className="text-right">Amount</TableHead>
						</TableRow>
					</TableHeader>
					<TableBody>
						{recentOrdersData?.map((order) => (
							<OrderRowMinimal
								key={`${order.id}-${order.customer.name}-${order.customer.surname}`}
								order={order}
							/>
						))}
					</TableBody>
				</Table>
			</CardContent>
		</Card>
	);
};
