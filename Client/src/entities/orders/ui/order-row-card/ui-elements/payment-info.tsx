import { FC } from "react";

import { cn, formatCurrency } from "@shared/lib/utils";
import { Badge } from "@shared/ui/badge";

import { OrdersResponse } from "../../../api";

import { getPaymentStatusInfo } from "../helpers";

type PaymentInfoProps = {
	order: OrdersResponse;
};

export const PaymentInfo: FC<PaymentInfoProps> = ({ order }) => {
	const isFullyPaid = order.paidAmount >= order.totalAmount;
	const isPartiallyPaid = order.paidAmount > 0;
	const remainingAmount = Math.max(0, order.totalAmount - order.paidAmount);

	const paymentStatus = getPaymentStatusInfo({ isFullyPaid, isPartiallyPaid, remainingAmount });

	return (
		<>
			<div className="text-right space-y-1">
				<div className="text-2xl font-bold">{formatCurrency(order.totalAmount)}</div>
				<div className="text-sm text-muted-foreground">
					Paid: {formatCurrency(order.paidAmount)}
				</div>
			</div>
			<Badge variant={paymentStatus.variant} className={cn("gap-1", paymentStatus.className)}>
				{paymentStatus.icon}
				{paymentStatus.text}
			</Badge>
			{order.outstandingAmount > 0 && (
				<Badge variant="destructive" className="text-xs">
					Outstanding: {formatCurrency(order.outstandingAmount)}
				</Badge>
			)}
		</>
	);
};
