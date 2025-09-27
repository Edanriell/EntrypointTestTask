"use client";

import { FC, useEffect, useState } from "react";
import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip } from "recharts";
import { useQuery } from "@tanstack/react-query";

import { statisticsQueries } from "@entities/statistics";

import { orderStatusColorMap, orderStatusDataMap } from "../lib";

import { OrderStatusesDistributionSkeleton } from "./order-statuses-distribution-skeleton";
import { OrderStatusesDistributionError } from "./order-statuses-distribution-error";

type OrderStatusesDistributionData = Array<OrdersAndStatuses>;

type OrdersAndStatuses = {
	status: string;
	count: number;
	color: string;
};

export const OrderStatusesDistribution: FC = () => {
	const [orderStatusesDistributionData, setOrderStatusesDistributionData] = useState<
		OrderStatusesDistributionData | []
	>([]);

	const {
		data: orderStatusesDistribution,
		isLoading,
		error
	} = useQuery(statisticsQueries.orderStatusDistribution());

	useEffect(() => {
		if (!orderStatusesDistribution) return;

		setOrderStatusesDistributionData(
			orderStatusesDistribution.orderStatusDistributions.map((data, index) => {
				return {
					status: orderStatusDataMap(data.status),
					count: data.count,
					color: orderStatusColorMap(index.toString())
				};
			})
		);
	}, [orderStatusesDistribution]);

	if (error) {
		return <OrderStatusesDistributionError />;
	}

	return (
		<>
			{isLoading && <OrderStatusesDistributionSkeleton />}
			{!isLoading && orderStatusesDistributionData && (
				<div className="rounded-lg border bg-card p-6 text-card-foreground shadow-sm col-span-2 lg:col-span-1">
					<div className="flex items-center justify-between mb-4">
						<h3 className="text-lg font-semibold">Order Status</h3>
					</div>
					<ResponsiveContainer width="100%" height={300}>
						<PieChart>
							<Pie
								data={orderStatusesDistributionData}
								cx="50%"
								cy="50%"
								outerRadius={70}
								dataKey="count"
								className="text-[10px]"
								label={({ status, count }) => `${status}: ${count}`}
							>
								{orderStatusesDistributionData.map((data, index) => (
									<Cell key={`cell-${data.status}`} fill={data.color} />
								))}
							</Pie>
							<Tooltip
								formatter={(value, name, props) => {
									const { status } = props.payload;
									return [`${value}`, status];
								}}
							/>
						</PieChart>
					</ResponsiveContainer>
				</div>
			)}
		</>
	);
};
