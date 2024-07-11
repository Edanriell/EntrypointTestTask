"use client";

import { FC, Fragment, useState } from "react";

import { OrderRow } from "@entities/orders/ui/order-row";

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@shared/ui/card";
import { Table, TableBody, TableHead, TableHeader, TableRow } from "@shared/ui/table";

import { OrderDrawer } from "@widgets/order-drawer";

type OrdersTableProps = {
	data: any;
	description: string;
};

export const OrdersTable: FC<OrdersTableProps> = ({ data, description }) => {
	const [isOrderDrawerOpen, setIsOrderDrawerOpen] = useState(false);
	const [selectedOrder, setSelectedOrder] = useState(null);

	const handleOrderRowClick = (order) => {
		console.log(order);
		setSelectedOrder(order);
		setIsOrderDrawerOpen(true);
	};

	const handleOrderDrawerClose = () => {
		setIsOrderDrawerOpen(false);
	};

	return (
		<Fragment>
			<Card x-chunk="All orders">
				<CardHeader className="px-7">
					<CardTitle>Orders</CardTitle>
					<CardDescription>{description}</CardDescription>
				</CardHeader>
				<CardContent>
					<Table>
						<TableHeader>
							<TableRow>
								<TableHead>Customer</TableHead>
								<TableHead className="hidden sm:table-cell">Status</TableHead>
								<TableHead className="hidden md:table-cell">Date</TableHead>
								<TableHead className="hidden md:table-cell">Last update</TableHead>
								<TableHead className="hidden md:table-cell">Ship address</TableHead>
								<TableHead className="hidden md:table-cell">Order information</TableHead>
								<TableHead className="text-right">Amount</TableHead>
							</TableRow>
						</TableHeader>
						<TableBody>
							{data?.length === 0 && (
								<div className="font-medium py-[40px] text-xl w-full">
									<p className="text-left">No orders has been found.</p>
								</div>
							)}
							{data?.map((order: any, index: number) => (
								<OrderRow
									onOrderClick={() => handleOrderRowClick(order)}
									key={index}
									order={order}
								/>
							))}
						</TableBody>
					</Table>
				</CardContent>
			</Card>
			<OrderDrawer
				onDrawerClose={handleOrderDrawerClose}
				isOrderDrawerOpen={isOrderDrawerOpen}
				order={selectedOrder}
			/>
		</Fragment>
	);
};
