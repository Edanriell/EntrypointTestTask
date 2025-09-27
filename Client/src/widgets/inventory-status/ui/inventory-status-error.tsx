import { FC } from "react";
import { TriangleAlert } from "lucide-react";

export const InventoryStatusError: FC = () => {
	return (
		<div className="rounded-lg border bg-card p-6 text-card-foreground h-[394px] flex flex-col items-center justify-center gap-2 col-span-2 lg:col-span-1">
			<TriangleAlert className="text-red-600" />
			<span className="text-sm font-medium">Could not load inventory status</span>
		</div>
	);
};
