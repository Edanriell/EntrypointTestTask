import { FC } from "react";

import { Skeleton } from "@shared/ui/skeleton";

export const ReservedStockSkeleton: FC = () => {
	return (
		<div className="flex flex-col gap-1 min-w-0 flex-shrink-0 xl:mr-8">
			<div className="text-xs text-muted-foreground text-center">Reserved</div>
			<div className="w-[80px] h-8 flex items-center gap-1 px-2 py-1 rounded text-sm font-medium">
				<Skeleton className="w-[50px] h-[20px]" />
			</div>
		</div>
	);
};
