import { FC } from "react";

import { TotalOrders } from "@widgets/total-orders";
import { ActiveProducts } from "@widgets/active-products";
import { TotalCustomers } from "@widgets/total-customers";
import { MonthlyRevenue } from "@widgets/monthly-revenue";
import { OrdersAndRevenueTrend } from "@widgets/orders-and-revenue-trend";
import { OrderStatusesDistribution } from "@widgets/order-statuses";
import { TopSellingProducts } from "@widgets/top-selling-products";
import { InventoryStatus } from "@widgets/inventory-status";
import { CustomerGrowthAndOrderVolume } from "@widgets/customer-growth-and-order-volume";
import { LowStockAlerts } from "@widgets/low-stock-alerts";
import { RecentOrders } from "@widgets/recent-orders";

export const HomePage: FC = () => {
	return (
		<div className="flex flex-1 flex-col gap-6 p-6">
			<div className="flex flex-col gap-2">
				<h1 className="text-3xl font-bold tracking-tight">Business Dashboard</h1>
				<p className="text-muted-foreground">
					Monitor your orders, products, and customer metrics in real-time.
				</p>
			</div>
			<div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
				<TotalOrders />
				<ActiveProducts />
				<TotalCustomers />
				<MonthlyRevenue />
			</div>
			<div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
				<OrdersAndRevenueTrend />
				<OrderStatusesDistribution />
				<TopSellingProducts />
				<InventoryStatus />
				<CustomerGrowthAndOrderVolume />
				<LowStockAlerts />
			</div>
			<RecentOrders />
		</div>
	);
};
