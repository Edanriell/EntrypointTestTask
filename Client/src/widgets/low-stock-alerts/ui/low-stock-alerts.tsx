"use client";

import { FC } from "react";
import { useQuery } from "@tanstack/react-query";

import { statisticsQueries } from "@entities/statistics";

import { LowStockAlertsError } from "./low-stock-alerts-error";
import { LowStockAlertsSkeleton } from "./low-stock-alerts-skeleton";

export const LowStockAlerts: FC = () => {
	const { data: products, isLoading, error } = useQuery(statisticsQueries.lowStockAlerts());

	if (error) {
		return <LowStockAlertsError />;
	}

	return (
		<>
			{isLoading && <LowStockAlertsSkeleton />}
			{!isLoading && products && (
				<div className="rounded-lg border bg-card p-6 text-card-foreground shadow-sm col-span-2 lg:col-span-1">
					<h3 className="text-lg font-semibold mb-4">Low Stock Alerts</h3>
					<div className="space-y-3">
						{products?.lowStockProducts.map((item, index) => (
							<div
								key={index}
								className="flex items-center justify-between p-3 bg-red-50 dark:bg-red-900/20 rounded-lg"
							>
								<div>
									<p className="text-sm font-medium">{item.productName}</p>
									<p className="text-xs text-muted-foreground">
										{item.unitsInStock} units left
									</p>
								</div>
								<div className="text-red-600 dark:text-red-400">
									<svg
										className="w-5 h-5"
										fill="currentColor"
										viewBox="0 0 20 20"
									>
										<path
											fillRule="evenodd"
											d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z"
											clipRule="evenodd"
										/>
									</svg>
								</div>
							</div>
						))}
					</div>
				</div>
			)}
		</>
	);
};
