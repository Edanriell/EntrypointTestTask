"use client";

import { FC, useEffect, useState } from "react";
import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip } from "recharts";
import { useQuery } from "@tanstack/react-query";

import { statisticsQueries } from "@entities/statistics";

import { inventoryStatusColorMap, inventoryStatusMap } from "../lib";

import { InventoryStatusError } from "./inventory-status-error";
import { InventoryStatusSkeleton } from "./inventory-status-skeleton";

type InventoryStatusData = Array<InventoryStatus>;

type InventoryStatus = {
	category: string;
	products: number;
	color: string;
};

export const InventoryStatus: FC = () => {
	const [inventoryStatusData, setInventoryStatusData] = useState<InventoryStatusData | []>([]);

	const { data: inventory, isLoading, error } = useQuery(statisticsQueries.inventoryStatus());

	useEffect(() => {
		if (!inventory) return;

		setInventoryStatusData(() => {
			return inventory?.inventorySummary.map((inventoryData, index) => {
				return {
					category: inventoryStatusMap(inventoryData.inventoryStatus),
					products: inventoryData.count,
					color: inventoryStatusColorMap(index.toString())
				};
			});
		});
	}, [inventory]);

	if (error) {
		return <InventoryStatusError />;
	}

	return (
		<>
			{isLoading && <InventoryStatusSkeleton />}
			{!isLoading && inventoryStatusData && (
				<div className="rounded-lg border bg-card p-6 text-card-foreground shadow-sm col-span-2 lg:col-span-1">
					<div className="flex items-center justify-between mb-4">
						<h3 className="text-lg font-semibold">Inventory Status</h3>
					</div>
					<ResponsiveContainer width="100%" height={300}>
						<PieChart>
							<Pie
								data={inventoryStatusData}
								cx="50%"
								cy="50%"
								innerRadius={40}
								outerRadius={70}
								className="text-[10px]"
								dataKey="products"
								label={({ category, products }) => `${category}: ${products}`}
							>
								{inventoryStatusData.map((entry, index) => (
									<Cell key={`cell-${index}`} fill={entry.color} />
								))}
							</Pie>
							<Tooltip
								formatter={(value, name, props) => {
									const { category } = props.payload;
									return [`${value}`, category];
								}}
							/>
						</PieChart>
					</ResponsiveContainer>
				</div>
			)}
		</>
	);
};
