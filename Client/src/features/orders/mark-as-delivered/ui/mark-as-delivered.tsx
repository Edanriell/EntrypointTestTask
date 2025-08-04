// src/features/orders/mark-as-delivered/ui/mark-as-delivered.tsx
import { FC } from "react";
import { CheckCircle } from "lucide-react";

import { OrderStatus } from "@entities/orders";

import { Button } from "@shared/ui/button";
import { Spinner } from "@shared/ui/spinner";

import { useMarkAsDelivered } from "../api";

type MarkAsDeliveredProps = {
	orderId: string;
	orderNumber?: string;
	orderStatus: string;
};

export const MarkAsDelivered: FC<MarkAsDeliveredProps> = ({
	orderId,
	orderNumber,
	orderStatus
}) => {
	const { mutateAsync: markAsDelivered, isPending } = useMarkAsDelivered();

	const handleMarkAsDelivered = async () => {
		try {
			await markAsDelivered({ orderId });
		} catch (error) {
			console.error("Error marking order as delivered:", error);
		}
	};

	// Only show button for orders that are out for delivery (shipped with estimated delivery date)
	if (orderStatus !== OrderStatus.OutForDelivery) {
		return null;
	}

	return (
		<Button
			variant="outline"
			size="sm"
			onClick={handleMarkAsDelivered}
			disabled={isPending}
			className="text-green-600 dark:text-green-400 hover:text-green-700 dark:hover:text-green-300"
		>
			{isPending ? (
				<>
					<Spinner className="mr-2 h-4 w-4" />
					Marking...
				</>
			) : (
				<>
					<CheckCircle className="mr-2 h-4 w-4" />
					Mark as Delivered
				</>
			)}
		</Button>
	);
};
