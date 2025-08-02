import { FC } from "react";
import { Play } from "lucide-react";

import { Button } from "@shared/ui/button";
import { Spinner } from "@shared/ui/spinner";

import { useGetPaymentsByOrderId } from "../api/use-get-payments-by-order-id";
import { useProcessPayment } from "../api/use-process-payment";

type ProcessPaymentProps = {
	orderId: string;
	orderNumber?: string;
	disabled?: boolean;
};

export const ProcessPayment: FC<ProcessPaymentProps> = ({
															orderId,
															orderNumber,
															disabled = false
														}) => {
	const { data: paymentsData, isLoading: isLoadingPayments } = useGetPaymentsByOrderId(orderId);
	const { mutateAsync: processPayment, isPending } = useProcessPayment(orderId);

	// Find the last pending payment according to business rules
	const pendingPayment = paymentsData?.payments?.find(
		(payment) => payment.paymentStatus === "Pending"
	);

	const handleProcessPayment = async () => {
		if (!pendingPayment) {
			console.warn("No pending payment found for order:", orderId);
			return;
		}

		try {
			await processPayment({ paymentId: pendingPayment.id });
		} catch (error) {
			console.error("Error processing payment:", error);
		}
	};

	// Don't show button if no pending payment exists
	if (isLoadingPayments) {
		return (
			<Button variant="outline" size="sm" disabled>
		<Spinner className="h-4 w-4 mr-2" />
			Loading...
		</Button>
	);
	}

	if (!pendingPayment) {
		return null; // Hide button if no pending payment
	}

	return (
		<Button
			variant="outline"
	size="sm"
	onClick={handleProcessPayment}
	disabled={disabled || isPending || !pendingPayment}
	className="text-blue-600 dark:text-blue-400"
		>
		{isPending ? (
						 <>
							 <Spinner className="h-4 w-4 mr-2" />
							 Processing...
				</>
) : (
		<>
			<Play className="h-4 w-4 mr-2" />
			Process Payment
	</>
)}
	</Button>
);
};