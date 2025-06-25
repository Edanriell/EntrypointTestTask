import { FC, ReactNode } from "react";
import type { Metadata } from "next";

import { generateStaticMetadata } from "@shared/lib/functions";

export const metadata: Metadata = generateStaticMetadata({
	title: "Orders - Business Management",
	description:
		"Track and manage all your orders in one place. Monitor order status, process transactions, and maintain complete order history with ease.",
	ogTitle: "Order Management Dashboard",
	ogDescription:
		"Efficient order processing and tracking system. Manage order lifecycle, monitor status updates, and streamline your fulfillment process."
});

type OrdersLayoutProps = {
	children: ReactNode;
};

export const OrdersLayout: FC<OrdersLayoutProps> = ({ children }) => {
	return <>{children}</>;
};
