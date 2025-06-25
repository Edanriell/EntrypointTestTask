import { FC, useState } from "react";
import { CreditCard } from "lucide-react";

import { Input } from "@shared/ui/input";
import { formatCurrency } from "@shared/lib/functions";
import { Button } from "@shared/ui/button";
import { OrderStatus } from "@entities/orders";

type ProcessPaymentWithAutomaticOrderConfirmationProps = {
	order: any;
	// onProcessPayment: (orderId: number, amount: number) => void;
};

export const ProcessPaymentWithAutomaticOrderConfirmation: FC<
	ProcessPaymentWithAutomaticOrderConfirmationProps
> = ({ order }) => {
	const [paymentAmount, setPaymentAmount] = useState("");

	const handleProcessPayment = () => {
		// const amount = parseFloat(paymentAmount);
		// if (!isNaN(amount) && amount > 0) {
		// 	onProcessPayment(order.id, amount);
		// 	setPaymentAmount("");
		// }
	};

	const isFullyPaid = order.paidAmount >= order.totalAmount;
	const remainingAmount = Math.max(0, order.totalAmount - order.paidAmount);

	const canProcessPayment =
		!isFullyPaid && [OrderStatus.PENDING, OrderStatus.CONFIRMED].includes(order.status);

	return (
		<>
			{canProcessPayment && (
				<div className="flex items-center gap-2">
					<Input
						type="number"
						step="0.01"
						min="0"
						max={remainingAmount}
						placeholder={`Max: ${formatCurrency(remainingAmount)}`}
						value={paymentAmount}
						onChange={(e) => setPaymentAmount(e.target.value)}
						className="w-32 h-8 text-sm"
					/>
					<Button
						size="sm"
						onClick={handleProcessPayment}
						disabled={!paymentAmount || parseFloat(paymentAmount) <= 0}
						className="h-8"
					>
						<CreditCard className="h-3 w-3 mr-1" />
						Pay
					</Button>
				</div>
			)}
		</>
	);
};
