import { FC } from "react";
import { CheckCircle } from "lucide-react";

import { Button } from "@shared/ui/button";
import { Spinner } from "@shared/ui/spinner";

import { useConfirmOrder } from "../api";

type ConfirmOrderProps = {
	orderId: string;
	orderNumber?: string;
	isFullyPaid: boolean;
	disabled?: boolean;
};

export const ConfirmOrder: FC<ConfirmOrderProps> = ({
	orderId,
	orderNumber,
	isFullyPaid,
	disabled = false
}) => {
	const { mutateAsync: confirmOrder, isPending } = useConfirmOrder();

	const handleConfirmOrder = async () => {
		try {
			await confirmOrder({ orderId });
		} catch (error) {
			console.error("Error confirming order:", error);
		}
	};

	// Only show button if order is fully paid
	if (!isFullyPaid) {
		return null;
	}

	return (
		<Button
			variant="outline"
			size="sm"
			onClick={handleConfirmOrder}
			disabled={disabled || isPending}
			className="text-green-600 dark:text-green-400"
		>
			{isPending ? (
				<>
					<Spinner className="h-4 w-4 mr-2" />
					Confirming...
				</>
			) : (
				<>
					<CheckCircle className="h-4 w-4 mr-2" />
					Confirm Order
				</>
			)}
		</Button>
	);
};
