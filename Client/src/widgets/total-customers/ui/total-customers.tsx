"use client";

import { FC } from "react";
import { useQuery } from "@tanstack/react-query";

import { statisticsQueries } from "@entities/statistics";

import { TotalCustomersError } from "./total-customers-error";
import { TotalCustomersSkeleton } from "./total-customers-skeleton";

export const TotalCustomers: FC = () => {
	const { data: customers, isLoading, error } = useQuery(statisticsQueries.totalCustomers());

	if (error) {
		return <TotalCustomersError />;
	}

	return (
		<>
			{isLoading && <TotalCustomersSkeleton />}
			{!isLoading && customers && (
				<div className="rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
					<div className="flex flex-row items-center justify-between space-y-0 pb-2">
						<h3 className="text-sm font-medium">Total Customers</h3>
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
							<path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2" />
							<circle cx="9" cy="7" r="4" />
							<path d="M22 21v-2a4 4 0 0 0-3-3.87M16 3.13a4 4 0 0 1 0 7.75" />
						</svg>
					</div>
					<div className="space-y-1">
						<div className="text-2xl font-bold">{customers?.totalCustomers}</div>
						<p className="text-xs text-muted-foreground">
							+{customers?.newThisMonth} new this month
						</p>
					</div>
				</div>
			)}
		</>
	);
};
