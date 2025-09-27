"use client";

import { FC } from "react";
import { Bar, BarChart, CartesianGrid, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";
import { useQuery } from "@tanstack/react-query";

import { statisticsQueries } from "@entities/statistics";

import { TopSellingProductsSkeleton } from "./top-selling-products-skeleton";
import { TopSellingProductsError } from "./top-selling-products-error";

export const TopSellingProducts: FC = () => {
	const { data: products, isLoading, error } = useQuery(statisticsQueries.topSellingProducts());

	if (error) {
		return <TopSellingProductsError />;
	}

	return (
		<>
			{isLoading && <TopSellingProductsSkeleton />}
			{!isLoading && products && (
				<div className="col-span-2 rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
					<div className="flex items-center justify-between mb-4">
						<h3 className="text-lg font-semibold">Top Selling Products</h3>
					</div>
					<ResponsiveContainer width="100%" height={300}>
						<BarChart data={products?.bestSellingProducts}>
							<CartesianGrid strokeDasharray="3 3" />
							<XAxis
								dataKey="productName"
								angle={-45}
								textAnchor="end"
								height={80}
								interval={0}
								className="text-[10px]"
							/>
							<YAxis />
							<Tooltip
								formatter={(value, name) => {
									return [
										name === "Units Sold" ? `${value} units` : `$${value}`,
										name === "Units Sold" ? "Units Sold" : "Revenue"
									];
								}}
							/>
							<Bar dataKey="unitsSold" fill="#8b5cf6" name="Units Sold" />
							<Bar dataKey="revenue" fill="#10b981" name="Revenue ($)" />
						</BarChart>
					</ResponsiveContainer>
				</div>
			)}
		</>
	);
};
