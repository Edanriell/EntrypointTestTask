import { FC } from "react";
import { ListFilter } from "lucide-react";

import { Tabs, TabsContent, TabsList, TabsTrigger } from "@shared/ui/tabs";
import {
	DropdownMenu,
	DropdownMenuCheckboxItem,
	DropdownMenuContent,
	DropdownMenuLabel,
	DropdownMenuSeparator,
	DropdownMenuTrigger
} from "@shared/ui/dropdown";
import { Button } from "@shared/ui/button";

import { OrderStatus } from "@entities/orders";
import { OrdersTable } from "@widgets/oders-table";
import { Search } from "@/features";

import { filterOrdersByStatus } from "./lib";
import { Card } from "@shared/ui/card";
import { Skeleton } from "@shared/ui/skeleton";

export type OrderManagementProps = {
	data: any;
	error: any;
	isPending: boolean;
	isError: boolean;
};

export const OrderManagement: FC<OrderManagementProps> = ({ data, error, isPending, isError }) => {
	// TODO
	// Fix sorting
	// Fix searching
	// FIX ALL TYPES
	// ALL ENTITY RELATED API REQUESTS MUST BE IN ENTITIES
	// TODO

	const allOrdersData = data;
	const pendingForPaymentOrdersData = filterOrdersByStatus(data, OrderStatus.PendingForPayment);
	const paidOrdersData = filterOrdersByStatus(data, OrderStatus.Paid);
	const inTransitOrdersData = filterOrdersByStatus(data, OrderStatus.InTransit);
	const deliveredOrdersData = filterOrdersByStatus(data, OrderStatus.Delivered);
	const cancelledOrdersData = filterOrdersByStatus(data, OrderStatus.Cancelled);

	if (isPending) {
		return (
			<Tabs defaultValue="created">
				<div className="flex items-center">
					<Skeleton className="w-[540px] h-[40px] rounded-lg border shadow-sm" />
					<div className="ml-auto flex items-center gap-2">
						<Skeleton className="w-[320px] h-[40px] rounded-lg border shadow-sm" />
						<Skeleton className="w-[70px] h-[40px] rounded-lg border shadow-sm" />
					</div>
				</div>
				<TabsContent value="created">
					<Card x-chunk="Orders">
						<Skeleton className="w-full h-[480px] rounded-lg" />
					</Card>
				</TabsContent>
			</Tabs>
		);
	}

	if (isError) {
		return (
			<Card x-chunk="Orders" className="h-[520px] w-full">
				<div className="text-1xl font-medium p-6">Error: {error.message}</div>
			</Card>
		);
	}

	return (
		<Tabs defaultValue="created">
			<div className="flex items-center">
				<TabsList>
					<TabsTrigger value="created">Created</TabsTrigger>
					<TabsTrigger value="pendingforpayment">Pending for payment</TabsTrigger>
					<TabsTrigger value="paid">Paid</TabsTrigger>
					<TabsTrigger value="intransit">In transit</TabsTrigger>
					<TabsTrigger value="delivered">Delivered</TabsTrigger>
					<TabsTrigger value="cancelled">Cancelled</TabsTrigger>
				</TabsList>
				<div className="ml-auto flex items-center gap-2">
					<Search />
					<DropdownMenu>
						<DropdownMenuTrigger asChild>
							<Button variant="outline" size="sm" className="h-10 gap-1 text-sm">
								<ListFilter className="h-3.5 w-3.5" />
								<span className="sr-only sm:not-sr-only">Sort</span>
							</Button>
						</DropdownMenuTrigger>
						<DropdownMenuContent align="end">
							<DropdownMenuLabel>Sort by creation date</DropdownMenuLabel>
							<DropdownMenuSeparator />
							<DropdownMenuCheckboxItem checked>Newest</DropdownMenuCheckboxItem>
							<DropdownMenuCheckboxItem>Oldest</DropdownMenuCheckboxItem>
						</DropdownMenuContent>
					</DropdownMenu>
				</div>
			</div>
			<TabsContent value="created">
				<OrdersTable data={allOrdersData} description={"List of all orders in store."} />
			</TabsContent>
			<TabsContent value="pendingforpayment">
				<OrdersTable
					data={pendingForPaymentOrdersData}
					description={"List of all orders pending for payment."}
				/>
			</TabsContent>
			<TabsContent value="paid">
				<OrdersTable data={paidOrdersData} description={"List of all paid orders."} />
			</TabsContent>
			<TabsContent value="intransit">
				<OrdersTable data={inTransitOrdersData} description={"List of all orders in transit."} />
			</TabsContent>
			<TabsContent value="delivered">
				<OrdersTable data={deliveredOrdersData} description={"List of all delivered orders."} />
			</TabsContent>
			<TabsContent value="cancelled">
				<OrdersTable data={cancelledOrdersData} description={"List of all cancelled orders."} />
			</TabsContent>
		</Tabs>
	);
};
