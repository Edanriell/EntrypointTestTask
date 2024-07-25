"use client";

import { FC, Fragment, useState } from "react";
import { ThreeDots } from "react-loader-spinner";

import type { Order } from "@entities/orders/model";

import {
	Card,
	CardContent,
	CardDescription,
	CardFooter,
	CardHeader,
	CardTitle
} from "@shared/ui/card";
import { Table, TableBody, TableHead, TableHeader, TableRow } from "@shared/ui/table";
import { Badge } from "@shared/ui/badge";
import { Skeleton } from "@shared/ui/skeleton";

type OrdersTableProps = {
	data: any;
	error: Error | null;
	isPending: boolean;
	isError: boolean;
	hasNextPage: boolean;
	isFetchingNextPage: boolean;
	status: "error" | "success" | "pending";
	description: string;
	tableLastRowRef?: (node?: Element | null | undefined) => void;
	columnsData: Array<{
		name: string;
		classes?: string;
	}>;
	Row: FC<any>;
	Drawer?: FC<any>;
	DrawerInnerContent?: FC<any>;
};

export const ContentTable: FC<OrdersTableProps> = ({
	tableLastRowRef,
	data,
	error,
	isPending,
	isError,
	isFetchingNextPage,
	hasNextPage,
	status,
	description,
	columnsData,
	Row,
	Drawer,
	DrawerInnerContent
}) => {
	const [isOrderDrawerOpen, setIsOrderDrawerOpen] = useState<boolean>(false);
	const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);

	const handleOrderRowClick = (order: Order) => {
		console.log(order);
		setSelectedOrder(order);
		setIsOrderDrawerOpen(true);
	};

	const handleOrderDrawerClose = () => setIsOrderDrawerOpen(false);

	if (isPending) {
		return (
			<Fragment>
				<Card x-chunk={description}>
					<CardHeader className="px-7">
						<CardTitle>Orders</CardTitle>
						<CardDescription>{description}</CardDescription>
					</CardHeader>
					<CardContent>
						<Skeleton className="w-full h-[48px] mb-[10px] rounded-lg" />
						<Skeleton className="w-full h-[72px] mb-[10px] rounded-lg" />
						<Skeleton className="w-full h-[72px] mb-[10px] rounded-lg" />
						<Skeleton className="w-full h-[72px] mb-[10px] rounded-lg" />
						<Skeleton className="w-full h-[72px] mb-[10px] rounded-lg" />
						<Skeleton className="w-full h-[72px] mb-[10px] rounded-lg" />
						<Skeleton className="w-full h-[72px] mb-[10px] rounded-lg" />
					</CardContent>
				</Card>
			</Fragment>
		);
	}

	if (isError) {
		return (
			<Fragment>
				<Card x-chunk={description}>
					<CardHeader className="px-7">
						<CardTitle>Orders</CardTitle>
						<CardDescription>{description}</CardDescription>
					</CardHeader>
					<CardContent>
						<Badge
							className="mb-[180px] mt-[140px] px-9 py-2 text-[14px] text-center block max-w-max mr-auto ml-auto"
							variant="destructive"
						>
							Error: {error?.message}
						</Badge>
					</CardContent>
				</Card>
			</Fragment>
		);
	}

	return (
		<Fragment>
			<Card x-chunk={description}>
				<CardHeader className="px-7">
					<CardTitle>Orders</CardTitle>
					<CardDescription>{description}</CardDescription>
				</CardHeader>
				<CardContent>
					<Table>
						<TableHeader>
							<TableRow>
								{columnsData.map(({ name, classes }, index) => (
									<TableHead key={index + "-" + name} className={classes}>
										{name}
									</TableHead>
								))}
							</TableRow>
						</TableHeader>
						<TableBody>
							{data?.pages.map((page: Array<Order>) =>
								page.map((order, index) => {
									if (page.length === index + 1) {
										return (
											<Row
												lastRowRef={tableLastRowRef}
												onRowClick={() => handleOrderRowClick(order)}
												key={order.id + order.customer.email + index}
												data={order}
												classes={Drawer && DrawerInnerContent ? "cursor-pointer" : ""}
											/>
										);
									} else {
										return (
											<Row
												onRowClick={() => handleOrderRowClick(order)}
												key={order.id + order.customer.email + index}
												data={order}
												classes={Drawer && DrawerInnerContent ? "cursor-pointer" : ""}
											/>
										);
									}
								})
							)}
						</TableBody>
					</Table>
				</CardContent>
				<CardFooter>
					{!hasNextPage && status === "success" && (
						<div className="text-xs text-muted-foreground">
							<strong>All content</strong> has been <strong>displayed</strong>
						</div>
					)}
					{hasNextPage && isFetchingNextPage && (
						<div className="flex flex-row items-center justify-center w-full ml-auto mr-auto">
							<ThreeDots
								visible={true}
								height="80"
								width="80"
								color="rgb(9, 9, 11)"
								radius="9"
								ariaLabel="loading orders"
							/>
						</div>
					)}
				</CardFooter>
			</Card>
			{Drawer && DrawerInnerContent ? (
				<Drawer
					data={selectedOrder}
					onDrawerClose={handleOrderDrawerClose}
					isOrderDrawerOpen={isOrderDrawerOpen}
					DrawerInnerContent={DrawerInnerContent}
				/>
			) : null}
		</Fragment>
	);
};
