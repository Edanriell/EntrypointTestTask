"use client";

import { useSession } from "next-auth/react";
import { useQuery } from "@tanstack/react-query";

import { CreateOrder } from "@widgets/create-order";
import { WeekSales } from "@widgets/week-sales";
import { MonthSales } from "@widgets/month-sales";

import { OrderManagement } from "@widgets/order-management";

import { getAllOrders } from "./api";

export const OrdersPage = () => {
	const { data: session, status } = useSession();

	const userId = (session as any)?.user?.id;
	const accessToken = (session as any)?.accessToken;

	const {
		data: ordersData,
		error: ordersError,
		isPending: isOrdersPending,
		isError: isOrdersError
	} = useQuery({
		queryKey: ["getAllOrders", userId],
		queryFn: () => getAllOrders(accessToken),
		enabled: !!userId && !!accessToken
	});

	return (
		<>
			<div className="grid flex-1 items-start gap-4 md:gap-8 lg:grid-cols-3 xl:grid-cols-3">
				<div className="grid auto-rows-max items-start gap-4 md:gap-8 lg:col-span-3">
					<div className="grid gap-4 sm:grid-cols-2 md:grid-cols-4 lg:grid-cols-2 xl:grid-cols-4">
						<CreateOrder />
						<WeekSales
							data={ordersData}
							error={ordersError}
							isError={isOrdersError}
							isPending={isOrdersPending}
						/>
						<MonthSales
							data={ordersData}
							error={ordersError}
							isError={isOrdersError}
							isPending={isOrdersPending}
						/>
					</div>
					{/*	onClick={() =>*/}
					{/*	handleRowClick({*/}
					{/*		               customer: "Liam Johnson",*/}
					{/*		               type: "Sale",*/}
					{/*		               status: "Fulfilled",*/}
					{/*		               date: "2023-06-23",*/}
					{/*		               amount: "$250.00"*/}
					{/*	               })*/}
					{/*}*/}
					<OrderManagement
						data={ordersData}
						error={ordersError}
						isError={isOrdersError}
						isPending={isOrdersPending}
					/>
				</div>
			</div>
		</>
	);
};
