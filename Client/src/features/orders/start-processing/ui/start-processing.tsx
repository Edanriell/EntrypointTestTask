import { FC } from "react";
import { Zap } from "lucide-react";

import { OrderStatus } from "@entities/orders";

import { Button } from "@shared/ui/button";
import { Spinner } from "@shared/ui/spinner";

import { useStartProcessingOrder } from "../api";

type StartProcessingOrderProps = {
	orderId: string;
	orderNumber?: string;
	orderStatus: string;
	disabled?: boolean;
};

export const StartProcessing: FC<StartProcessingOrderProps> = ({
	orderId,
	orderNumber,
	orderStatus,
	disabled = false
}) => {
	const { mutateAsync: startProcessingOrder, isPending } = useStartProcessingOrder();

	const handleStartProcessing = async () => {
		try {
			await startProcessingOrder({ orderId });
		} catch (error) {
			console.error("Error starting order processing:", error);
		}
	};

	// Only show button if order is confirmed
	if (orderStatus !== OrderStatus.Confirmed) {
		return null;
	}

	return (
		<Button
			variant="outline"
			size="sm"
			onClick={handleStartProcessing}
			disabled={disabled || isPending}
			className="text-blue-600 dark:text-blue-400"
		>
			{isPending ? (
				<>
					<Spinner className="h-4 w-4 mr-2" />
					Starting...
				</>
			) : (
				<>
					<Zap className="h-4 w-4 mr-2" />
					Start Processing
				</>
			)}
		</Button>
	);
};
