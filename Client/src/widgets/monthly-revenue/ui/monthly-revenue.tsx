"use client";

import { FC } from "react";
import { useQuery } from "@tanstack/react-query";

import { statisticsQueries } from "@entities/statistics";

import { MonthlyRevenueError } from "./monthly-revenue-error";
import { MonthlyRevenueSkeleton } from "./monthly-revenue-skeleton";

export const MonthlyRevenue: FC = () => {
	const { data: revenue, isLoading, error } = useQuery(statisticsQueries.monthlyRevenue());

	if (error) {
		return <MonthlyRevenueError />;
	}

	return (
		<>
			{isLoading && <MonthlyRevenueSkeleton />}
			{!isLoading && revenue && (
				<div className="rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
					<div className="flex flex-row items-center justify-between space-y-0 pb-2">
						<h3 className="text-sm font-medium">Monthly Revenue</h3>
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
							<path d="M12 2v20m9-9H3" />
						</svg>
					</div>
					<div className="space-y-1">
						<div className="text-2xl font-bold">&euro;{revenue?.totalRevenue}</div>
						<p className="text-xs text-muted-foreground">
							{revenue?.changePercent}% from last month
						</p>
					</div>
				</div>
			)}
		</>
	);
};
