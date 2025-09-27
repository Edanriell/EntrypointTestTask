import { FC } from "react";
import { TriangleAlert } from "lucide-react";

export const ActiveProductsError: FC = () => {
	return (
		<div className="rounded-lg border bg-card p-6 text-card-foreground flex flex-col items-center justify-center gap-2 h-[130px]">
			<TriangleAlert className="text-red-600" />
			<span className="text-sm font-medium">Could not load active products</span>
		</div>
	);
};
