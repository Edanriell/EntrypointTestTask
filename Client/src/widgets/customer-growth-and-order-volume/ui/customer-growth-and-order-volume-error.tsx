import { FC } from "react";
import { TriangleAlert } from "lucide-react";

export const CustomerGrowthAndOrderVolumeError: FC = () => {
	return (
		<div className="col-span-2 rounded-lg border bg-card p-6 text-card-foreground h-[502px] flex flex-col items-center justify-center">
			<TriangleAlert className="text-red-600" />
			<span className="text-sm font-medium">
				Could not load customer growth and order volume
			</span>
		</div>
	);
};
