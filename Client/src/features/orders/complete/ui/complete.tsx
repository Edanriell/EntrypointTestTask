// src/features/orders/complete/ui/complete-order.tsx
import { FC } from "react";
import { CheckCheck } from "lucide-react";

import { OrderStatus } from "@entities/orders";

import { Button } from "@shared/ui/button";
import { Spinner } from "@shared/ui/spinner";

import { useCompleteOrder } from "../api";

type CompleteOrderProps = {
	orderId: string;
	orderNumber?: string;
	orderStatus: string;
};

export const Complete: FC<CompleteOrderProps> = ({ orderId, orderNumber, orderStatus }) => {
	const { mutateAsync: completeOrder, isPending } = useCompleteOrder();

	const handleCompleteOrder = async () => {
		try {
			await completeOrder({ orderId });
		} catch (error) {
			console.error("Error completing order:", error);
		}
	};

	// Only show button for delivered orders
	if (orderStatus !== OrderStatus.Delivered) {
		return null;
	}

	return (
		<Button
			variant="outline"
			size="sm"
			onClick={handleCompleteOrder}
			disabled={isPending}
			className="text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300"
		>
			{isPending ? (
				<>
					<Spinner className="mr-2 h-4 w-4" />
					Completing...
				</>
			) : (
				<>
					<CheckCheck className="mr-2 h-4 w-4" />
					Complete Order
				</>
			)}
		</Button>
	);
};
