"use client";

import { Fragment } from "react";

import { TotalRevenue } from "@widgets/total-revenue";
import { TotalUsers } from "@widgets/total-users";
import { TotalOrders } from "@widgets/total-orders";
import { TotalProducts } from "@widgets/total-products";
import { RecentOrders } from "@widgets/recent-orders";
import { RecentUsers } from "@widgets/recent-users";

export const DashboardPage = () => {
	return (
		<Fragment>
			<div className="grid gap-4 md:grid-cols-2 md:gap-8 lg:grid-cols-4">
				<TotalRevenue />
				<TotalUsers />
				<TotalOrders />
				<TotalProducts />
			</div>
			<div className="grid gap-4 md:gap-8 lg:grid-cols-2 xl:grid-cols-3">
				<RecentOrders />
				<RecentUsers />
			</div>
		</Fragment>
	);
};
