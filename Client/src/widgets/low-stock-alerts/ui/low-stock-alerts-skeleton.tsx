import { FC } from "react";

import { Skeleton } from "@shared/ui/skeleton";

export const LowStockAlertsSkeleton: FC = () => {
	return (
		<Skeleton className="rounded-lg border p-6 h-[502px] flex flex-col items-center justify-center gap-2 col-span-2 lg:col-span-1" />
	);
};
