import { FC } from "react";
import { TriangleAlert } from "lucide-react";

export const TopSellingProductsError: FC = () => {
	return (
		<div className="col-span-2 rounded-lg border bg-card p-6 text-card-foreground h-[394px] flex flex-col items-center justify-center">
			<TriangleAlert className="text-red-600" />
			<span className="text-sm font-medium">Could not load top selling products</span>
		</div>
	);
};
