"use client";

import { FC, useEffect, useState } from "react";
import {
	Area,
	AreaChart,
	CartesianGrid,
	Legend,
	ResponsiveContainer,
	Tooltip,
	XAxis,
	YAxis
} from "recharts";
import { useQuery } from "@tanstack/react-query";

import { statisticsQueries } from "@entities/statistics";

import { monthsMap } from "../lib";

import { OrdersAndRevenueTrendSkeleton } from "./orders-and-revenue-trend-skeleton";
import { OrdersAndRevenueTrendError } from "./orders-and-revenue-trend-error";

type OrdersAndRevenueData = Array<OrderAndRevenue>;

type OrderAndRevenue = {
	month: string;
	orders: number;
	revenue: number;
};

export const OrdersAndRevenueTrend: FC = () => {
	const [ordersAndRevenueData, setOrdersAndRevenueData] = useState<OrdersAndRevenueData | []>([]);

	const {
		data: ordersAndRevenue,
		isLoading,
		error
	} = useQuery(statisticsQueries.ordersAndRevenueTrend());

	useEffect(() => {
		if (!ordersAndRevenue) return;

		setOrdersAndRevenueData(() => {
			return ordersAndRevenue?.trend?.map((data) => {
				return {
					month: monthsMap(data.month),
					orders: data.orders,
					revenue: data.revenue
				};
			});
		});
	}, [ordersAndRevenue]);

	if (error) {
		return <OrdersAndRevenueTrendError />;
	}

	return (
		<>
			{isLoading && <OrdersAndRevenueTrendSkeleton />}
			{!isLoading && ordersAndRevenueData && (
				<div className="col-span-2 rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
					<div className="flex items-center justify-between mb-4">
						<h3 className="text-lg font-semibold">Orders & Revenue Trend</h3>
					</div>
					<ResponsiveContainer width="100%" height={300}>
						<AreaChart data={ordersAndRevenueData}>
							<CartesianGrid strokeDasharray="3 3" />
							<XAxis dataKey="month" />
							<YAxis yAxisId="left" />
							<YAxis yAxisId="right" orientation="right" />
							<Tooltip
								formatter={(value, name) => [
									name === "orders" ? value : `$${value}`,
									name === "orders" ? "Orders" : "Revenue"
								]}
							/>
							<Legend />
							<Area
								yAxisId="left"
								type="monotone"
								dataKey="orders"
								stroke="#3b82f6"
								fill="#3b82f6"
								fillOpacity={0.6}
							/>
							<Area
								yAxisId="right"
								type="monotone"
								dataKey="revenue"
								stroke="#10b981"
								fill="#10b981"
								fillOpacity={0.3}
							/>
						</AreaChart>
					</ResponsiveContainer>
				</div>
			)}
		</>
	);
};
