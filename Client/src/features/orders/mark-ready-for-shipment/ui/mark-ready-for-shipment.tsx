import { FC } from "react";
import { PackageCheck } from "lucide-react";

import { OrderStatus } from "@entities/orders";

import { Button } from "@shared/ui/button";
import { Spinner } from "@shared/ui/spinner";

import { useMarkReadyForShipment } from "../api";

type MarkReadyForShipmentProps = {
	orderId: string;
	orderNumber?: string;
	orderStatus: string;
	disabled?: boolean;
};

export const MarkReadyForShipment: FC<MarkReadyForShipmentProps> = ({
	orderId,
	orderNumber,
	orderStatus,
	disabled = false
}) => {
	const { mutateAsync: markReadyForShipment, isPending } = useMarkReadyForShipment();

	const handleMarkReady = async () => {
		try {
			await markReadyForShipment({ orderId });
		} catch (error) {
			console.error("Error marking order ready for shipment:", error);
		}
	};

	// Only show button if order is processing
	if (orderStatus !== OrderStatus.Processing) {
		return null;
	}

	return (
		<Button
			variant="outline"
			size="sm"
			onClick={handleMarkReady}
			disabled={disabled || isPending}
			className="text-amber-600 dark:text-amber-400"
		>
			{isPending ? (
				<>
					<Spinner className="h-4 w-4 mr-2" />
					Marking Ready...
				</>
			) : (
				<>
					<PackageCheck className="h-4 w-4 mr-2" />
					Mark Ready
				</>
			)}
		</Button>
	);
};
