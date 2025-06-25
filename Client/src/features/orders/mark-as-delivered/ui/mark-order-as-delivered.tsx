import { FC } from "react";
import { Button } from "@shared/ui/button";
import { CheckCircle } from "lucide-react";
import { OrderStatus } from "@entities/orders";

type MarkOrderAsDeliveredProps = {
	order: any;
};

export const MarkOrderAsDelivered: FC<MarkOrderAsDeliveredProps> = ({ order }) => {
	const canMarkDelivered = order.status === OrderStatus.SHIPPED;

	return (
		<>
			{canMarkDelivered && (
				<Button
					size="sm"
					// onClick={() => onMarkAsDelivered(order.id)}
					className="h-8"
				>
					<CheckCircle className="h-3 w-3 mr-1" />
					Mark Delivered
				</Button>
			)}
		</>
	);
};
