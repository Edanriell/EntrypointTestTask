"use client";

import { FC } from "react";
import { useQuery } from "@tanstack/react-query";

import { ActiveProductsSkeleton } from "@widgets/active-products/ui/active-products-skeleton";
import { ActiveProductsError } from "@widgets/active-products/ui/active-products-error";

import { statisticsQueries } from "@entities/statistics";

export const ActiveProducts: FC = () => {
	const { data: products, isLoading, error } = useQuery(statisticsQueries.activeProducts());

	if (error) {
		return <ActiveProductsError />;
	}

	return (
		<>
			{isLoading && <ActiveProductsSkeleton />}
			{!isLoading && products && (
				<div className="rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
					<div className="flex flex-row items-center justify-between space-y-0 pb-2">
						<h3 className="text-sm font-medium">Active Products</h3>
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
							<path d="M20 7h-9" />
							<path d="M14 17H5" />
							<circle cx="17" cy="17" r="3" />
							<circle cx="7" cy="7" r="3" />
						</svg>
					</div>
					<div className="space-y-1">
						<div className="text-2xl font-bold">{products?.activeProducts}</div>
						<p className="text-xs text-muted-foreground">
							{products?.lowStockProducts} low stock items
						</p>
					</div>
				</div>
			)}
		</>
	);
};
