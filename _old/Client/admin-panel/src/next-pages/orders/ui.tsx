import { FC } from "react";

import { CreateOrder } from "@widgets/create-order";
import { WeekSales } from "@widgets/week-sales";
import { MonthSales } from "@widgets/month-sales";
import { OrderTabs } from "widgets/order-tabs";

export const OrdersPage: FC = () => {
	return (
		<div className="grid flex-1 items-start gap-4 md:gap-8 lg:grid-cols-3 xl:grid-cols-3">
			<div className="grid auto-rows-max items-start gap-4 md:gap-8 lg:col-span-3">
				<div className="grid gap-4 sm:grid-cols-2 md:grid-cols-4 lg:grid-cols-2 xl:grid-cols-4">
					<CreateOrder />
					<WeekSales />
					<MonthSales />
				</div>
				<OrderTabs />
			</div>
		</div>
	);
};
