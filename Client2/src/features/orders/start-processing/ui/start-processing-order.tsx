import { FC } from "react";
import { Play } from "lucide-react";

import { Button } from "@shared/ui/button";
import { OrderStatus } from "@entities/orders";

type StartProcessingOrderProps = {
	order: any;
};

export const StartProcessingOrder: FC<StartProcessingOrderProps> = ({ order }) => {
	const isFullyPaid = order.paidAmount >= order.totalAmount;

	const canStartProcessing = isFullyPaid && order.status === OrderStatus.CONFIRMED;

	return (
		<>
			{canStartProcessing && (
				<Button
					size="sm"
					// onClick={() => onStartProcessing(order.id)}
					className="h-8"
				>
					<Play className="h-3 w-3 mr-1" />
					Start Processing
				</Button>
			)}
		</>
	);
};
