import { AlertTriangle, CheckCircle2, Clock, XCircle } from "lucide-react";

export const getPaymentStatusBadge = (status: string) => {
	switch (status.toLowerCase()) {
		case "paid":
		case "completed":
			return {
				icon: CheckCircle2,
				className: "bg-green-100 text-green-700 border-green-200 hover:bg-green-100/80",
				label: "Paid"
			};
		case "pending":
			return {
				icon: Clock,
				className: "bg-amber-100 text-amber-700 border-amber-200 hover:bg-amber-100/80",
				label: "Pending"
			};
		case "failed":
			return {
				icon: XCircle,
				className: "bg-red-100 text-red-700 border-red-200 hover:bg-red-100/80",
				label: "Failed"
			};
		case "expired":
			return {
				icon: AlertTriangle,
				className: "bg-gray-100 text-gray-700 border-gray-200 hover:bg-gray-100/80",
				label: "Expired"
			};
		default:
			return {
				icon: Clock,
				className: "bg-muted text-foreground border-border",
				label: status
			};
	}
};
