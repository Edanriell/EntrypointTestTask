import { FC } from "react";
import { Button } from "@shared/ui/button";
import { XCircle } from "lucide-react";
import { OrderStatus } from "@entities/orders";

type CancelOrderProps = {
	order: any;
};

export const CancelOrder: FC<CancelOrderProps> = ({ order }) => {
	const canCancel = [OrderStatus.PENDING, OrderStatus.CONFIRMED, OrderStatus.PROCESSING].includes(
		order.status
	);

	return (
		<>
			{canCancel && (
				<Button
					size="sm"
					variant="destructive"
					// onClick={() => onCancelOrder(order.id)}
					className="h-8"
				>
					<XCircle className="h-3 w-3 mr-1" />
					Cancel
				</Button>
			)}
		</>
	);
};
