import { AlertCircle, CheckCircle2 } from "lucide-react";

import { formatCurrency } from "@shared/lib/utils";

type GetPaymentStatusInfoParameters = {
	isFullyPaid: boolean;
	isPartiallyPaid: boolean;
	remainingAmount: number;
};

export const getPaymentStatusInfo = ({
	isFullyPaid,
	isPartiallyPaid,
	remainingAmount
}: GetPaymentStatusInfoParameters) => {
	if (isFullyPaid) {
		return {
			variant: "default" as const,
			className: "bg-green-50 text-green-700 border-green-200",
			icon: <CheckCircle2 className="h-3 w-3" />,
			text: "Fully Paid"
		};
	} else if (isPartiallyPaid) {
		return {
			variant: "secondary" as const,
			className: "bg-amber-50 text-amber-700 border-amber-200",
			icon: <AlertCircle className="h-3 w-3" />,
			text: `${formatCurrency(remainingAmount)} due`
		};
	} else {
		return {
			variant: "destructive" as const,
			className: "bg-red-50 text-red-700 border-red-200",
			icon: <AlertCircle className="h-3 w-3" />,
			text: "Payment Pending"
		};
	}
};
