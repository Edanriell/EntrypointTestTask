"use client";

import { FC, useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";

import {
	CartesianGrid,
	Legend,
	Line,
	LineChart,
	ResponsiveContainer,
	Tooltip,
	XAxis,
	YAxis
} from "recharts";

import { statisticsQueries } from "@entities/statistics";

import { monthsMap } from "../lib";

import { CustomerGrowthAndOrderVolumeError } from "./customer-growth-and-order-volume-error";
import { CustomerGrowthAndOrderVolumeSkeleton } from "./customer-growth-and-order-volume-skeleton";

type CustomersAndOrders = Array<CustomerAndOrder>;

type CustomerAndOrder = {
	month: string;
	totalCustomers: number;
	totalOrders: number;
};

export const CustomerGrowthAndOrderVolume: FC = () => {
	const [customersAndOrdersData, setCustomersAndOrdersData] = useState<CustomersAndOrders | []>(
		[]
	);

	const {
		data: customersAndOrders,
		isLoading,
		error
	} = useQuery(statisticsQueries.customerGrowthAndOrderVolume());

	useEffect(() => {
		if (!customersAndOrders) return;

		setCustomersAndOrdersData(() => {
			return customersAndOrders?.trend.map((data) => {
				return {
					month: monthsMap(data.month),
					totalCustomers: data.totalCustomers,
					totalOrders: data.totalOrders
				};
			});
		});
	}, [customersAndOrders]);

	if (error) {
		return <CustomerGrowthAndOrderVolumeError />;
	}

	return (
		<>
			{isLoading && <CustomerGrowthAndOrderVolumeSkeleton />}
			{!isLoading && customersAndOrdersData && (
				<div className="col-span-2 rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
					<div className="flex items-center justify-between mb-4">
						<h3 className="text-lg font-semibold">Customer Growth & Order Volume</h3>
					</div>
					<ResponsiveContainer width="100%" height={300}>
						<LineChart data={customersAndOrdersData}>
							<CartesianGrid strokeDasharray="3 3" />
							<XAxis dataKey="month" />
							<YAxis yAxisId="left" />
							<YAxis yAxisId="right" orientation="right" />
							<Tooltip
								formatter={(value, name) => [
									value,
									name === "totalCustomers" ? "Total Customers" : "Total Orders"
								]}
							/>
							<Legend />
							<Line
								yAxisId="left"
								type="monotone"
								dataKey="totalCustomers"
								stroke="#3b82f6"
								strokeWidth={3}
								dot={{ fill: "#3b82f6", strokeWidth: 2, r: 4 }}
								name="Total Customers"
							/>
							<Line
								yAxisId="right"
								type="monotone"
								dataKey="totalOrders"
								stroke="#10b981"
								strokeWidth={3}
								dot={{ fill: "#10b981", strokeWidth: 2, r: 4 }}
								name="Total Orders"
							/>
						</LineChart>
					</ResponsiveContainer>
				</div>
			)}
		</>
	);
};
