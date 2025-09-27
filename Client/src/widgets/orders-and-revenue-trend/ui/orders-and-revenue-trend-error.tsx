import { FC } from "react";
import { TriangleAlert } from "lucide-react";

export const OrdersAndRevenueTrendError: FC = () => {
	return (
		<div className="col-span-2 rounded-lg border bg-card p-6 text-card-foreground flex flex-col items-center justify-center gap-2 h-[394px]">
			<TriangleAlert className="text-red-600" />
			<span className="text-sm font-medium">Could not load orders and revenue trend</span>
		</div>
	);
};
