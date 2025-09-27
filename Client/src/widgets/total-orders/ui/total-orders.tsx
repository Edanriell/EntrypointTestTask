"use client";

import { FC } from "react";
import { useQuery } from "@tanstack/react-query";

import { statisticsQueries } from "@entities/statistics";

import { TotalOrdersSkeleton } from "./total-orders-skeleton";
import { TotalOrdersError } from "./total-orders-error";

export const TotalOrders: FC = () => {
	const { data: orders, isLoading, error } = useQuery(statisticsQueries.totalOrders());

	if (error) {
		return <TotalOrdersError />;
	}

	return (
		<>
			{isLoading && <TotalOrdersSkeleton />}
			{!isLoading && orders && (
				<div className="rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
					<div className="flex flex-row items-center justify-between space-y-0 pb-2">
						<h3 className="text-sm font-medium">Total Orders</h3>
						<svg
							xmlns="http://www.w3.org/2000/svg"
							viewBox="0 0 24 24"
							fill="none"
							stroke="currentColor"
							strokeLinecap="round"
							strokeLinejoin="round"
							strokeWidth="2"
							className="h-4 w-4 text-muted-foreground"
						>
							<path d="M16 4h2a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h2" />
							<rect x="8" y="2" width="8" height="4" rx="1" ry="1" />
						</svg>
					</div>
					<div className="space-y-1">
						<div className="text-2xl font-bold">{orders?.totalOrders}</div>
						<p className="text-xs text-muted-foreground">
							{orders?.changePercent}% from last month
						</p>
					</div>
				</div>
			)}
		</>
	);
};
