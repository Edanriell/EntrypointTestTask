import { FC } from "react";
import { Button } from "@shared/ui/button";
import { DollarSign } from "lucide-react";
import { OrderStatus } from "@entities/orders";

type RefundPaymentWithOrderUpdateProps = {
	order: any;
};

export const RefundPaymentWithOrderUpdate: FC<RefundPaymentWithOrderUpdateProps> = ({ order }) => {
	// TODO
	// Handler here

	const canRefund = [OrderStatus.CANCELLED, OrderStatus.RETURNED].includes(order.status);

	return (
		<>
			{canRefund && (
				<Button
					size="sm"
					variant="outline"
					// onClick={() => onRefundPayment(order.id)}
					className="h-8"
				>
					<DollarSign className="h-3 w-3 mr-1" />
					Refund
				</Button>
			)}
		</>
	);
};
