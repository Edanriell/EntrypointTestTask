import { FC } from "react";

import { Skeleton } from "@shared/ui/skeleton";

export const InventoryStatusSkeleton: FC = () => {
	return (
		<Skeleton className="rounded-lg border p-6 h-[394px] flex flex-col items-center justify-center gap-2 col-span-2 lg:col-span-1" />
	);
};
