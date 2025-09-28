"use client";

import { Children, FC, isValidElement, ReactElement, ReactNode } from "react";

import { Card, CardContent, CardHeader } from "@shared/ui/card";

import type { OrdersResponse } from "../../api";

import {
	AdditionalInfo,
	OrderInfo,
	PaymentInfo
} from "@entities/orders/ui/order-row-card/ui-elements";
import { Payments } from "@entities/orders/ui/order-row-card/ui-elements/payments";
import { OrderProducts } from "@entities/orders/ui/order-row-card/ui-elements/order-products";
import { ShippingInfo } from "@entities/orders/ui/order-row-card/ui-elements/shipping-info";
import { Timeline } from "@entities/orders/ui/order-row-card/ui-elements/timeline";

import { ManagementActions } from "./management-actions";
import { ContextActions } from "./context-actions";

type OrderRowCardProps = {
	order: OrdersResponse;
	children: ReactNode;
};

type OrderRowCardComponents = {
	ManagementActions: typeof ManagementActions;
	ContextActions: typeof ContextActions;
};

type OrderRowCard = FC<OrderRowCardProps> & OrderRowCardComponents;

export const OrderRowCard: OrderRowCard = ({ order, children }) => {
	const childrenArray = Children.toArray(children);
	const contextActions = childrenArray.find(
		(child) => isValidElement(child) && child.type === ContextActions
	) as ReactElement | undefined;
	const managementActions = childrenArray.find(
		(child) => isValidElement(child) && child.type === ManagementActions
	) as ReactElement | undefined;

	return (
		<Card>
			<CardHeader className="pb-3">
				<div className="flex items-start justify-between">
					<OrderInfo order={order} />
					<div className="flex flex-col items-end space-y-3">
						<PaymentInfo order={order} />
						{managementActions}
					</div>
				</div>
			</CardHeader>
			<CardContent className="space-y-4">
				<Timeline order={order} />
				<ShippingInfo order={order} />
				<Payments
					payments={order.payments}
					remainingAmount={Math.max(0, order.totalAmount - order.paidAmount)}
				/>
				<OrderProducts orderProducts={order.orderProducts} />
				{contextActions && <>{contextActions}</>}
				<AdditionalInfo order={order} />
			</CardContent>
		</Card>
	);
};

OrderRowCard.ManagementActions = ManagementActions;
OrderRowCard.ContextActions = ContextActions;
