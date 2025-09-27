import { FC } from "react";
import { TriangleAlert } from "lucide-react";

export const RecentOrdersError: FC = () => {
	return (
		<div className="rounded-lg border bg-card p-6 text-card-foreground h-[500px]">
			<TriangleAlert className="text-red-600" />
			<span className="text-sm font-medium">Could not load recent orders</span>
		</div>
	);
};
