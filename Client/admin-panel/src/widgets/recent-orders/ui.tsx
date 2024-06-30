import { FC } from "react";
import Link from "next/link";
import { ArrowUpRight } from "lucide-react";

import {
	Button,
	Card,
	CardContent,
	CardDescription,
	CardHeader,
	CardTitle,
	Skeleton,
	Table,
	TableBody,
	TableCell,
	TableHead,
	TableHeader,
	TableRow
} from "@shared/ui";
import { formatDateString } from "@shared/lib";

import { calculateOrderTotal } from "./lib";

type RecentOrdersProps = {
	data: any;
	error: any;
	isPending: boolean;
	isError: boolean;
};

export const RecentOrders: FC<RecentOrdersProps> = ({ data, error, isPending, isError }) => {
	if (isPending) {
		return (
			<Card className="xl:col-span-2" x-chunk="A card showing a table of recent orders.">
				<Skeleton className="w-full h-[537.74px] md:h-[full] rounded-lg" />
			</Card>
		);
	}

	if (isError) {
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
							<div className="text-2xl font-bold text-left mt-[40px]">Error: {error.message}</div>
						</TableBody>
					</Table>
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
						{data?.map((order: any) => (
							<TableRow key={order.id}>
								<TableCell>
									<div className="font-medium">
										{order.customer.name} {order.customer.surname}
									</div>
									<div className="hidden text-sm text-muted-foreground md:inline">
										{order.customer.email}
									</div>
								</TableCell>
								<TableCell>{formatDateString(order.createdAt)}</TableCell>
								<TableCell className="text-right">${calculateOrderTotal(order)}</TableCell>
							</TableRow>
						))}
					</TableBody>
				</Table>
			</CardContent>
		</Card>
	);
};
