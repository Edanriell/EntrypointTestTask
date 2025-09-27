"use client";

import { FC, useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";

import { statisticsQueries } from "@entities/statistics";

import { RecentOrdersSkeleton } from "./recent-orders-skeleton";
import { RecentOrdersError } from "./recent-orders-error";
import { formatDate } from "@shared/lib/utils";

type RecentOrdersData = Array<RecentOrder>;

type RecentOrder = {
	number: string;
	customer: string;
	status: string;
	total: number;
	date: string;
};

export const RecentOrders: FC = () => {
	const [recentOrdersData, setRecentOrdersData] = useState<RecentOrdersData | []>([]);

	const { data: orders, isLoading, error } = useQuery(statisticsQueries.recentOrders());

	if (error) {
		return <RecentOrdersError />;
	}

	useEffect(() => {
		if (!orders) return;

		setRecentOrdersData(() => {
			return orders.recentOrders.map((data) => {
				return {
					number: data.number.slice(7),
					customer: data.customer,
					status: data.status,
					total: data.total,
					date: formatDate(data.date)
				};
			});
		});
	}, [orders]);

	return (
		<>
			{isLoading && <RecentOrdersSkeleton />}
			{!isLoading && recentOrdersData && (
				<div className="rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
					<h3 className="text-lg font-semibold mb-4">Recent Orders</h3>
					<div className="overflow-x-auto">
						<table className="w-full text-sm">
							<thead>
								<tr className="border-b">
									<th className="text-left p-2">Order Number</th>
									<th className="text-left p-2">Customer</th>
									<th className="text-left p-2">Status</th>
									<th className="text-left p-2">Total</th>
									<th className="text-left p-2">Date</th>
								</tr>
							</thead>
							<tbody>
								{recentOrdersData.map((order, index) => (
									<tr key={index} className="border-b hover:bg-muted/50">
										<td className="p-2 font-medium">{order.number}</td>
										<td className="p-2">{order.customer}</td>
										<td className="p-2">
											<span
												className={`px-2 py-1 rounded-full text-xs ${
													order.status === "Delivered"
														? "bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-400"
														: order.status === "InTransit"
															? "bg-purple-100 text-purple-800 dark:bg-purple-900/20 dark:text-purple-400"
															: order.status === "Paid"
																? "bg-blue-100 text-blue-800 dark:bg-blue-900/20 dark:text-blue-400"
																: order.status ===
																	  "PendingForPayment"
																	? "bg-yellow-100 text-yellow-800 dark:bg-yellow-900/20 dark:text-yellow-400"
																	: order.status === "Created"
																		? "bg-gray-100 text-gray-800 dark:bg-gray-900/20 dark:text-gray-400"
																		: "bg-red-100 text-red-800 dark:bg-red-900/20 dark:text-red-400"
												}`}
											>
												{order.status}
											</span>
										</td>
										<td className="p-2">{order.total}</td>
										<td className="p-2">{order.date}</td>
									</tr>
								))}
							</tbody>
						</table>
					</div>
				</div>
			)}
		</>
	);
};
