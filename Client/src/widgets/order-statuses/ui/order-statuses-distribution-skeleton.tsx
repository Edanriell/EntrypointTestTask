import { FC } from "react";

import { Skeleton } from "@shared/ui/skeleton";

export const OrderStatusesDistributionSkeleton: FC = () => {
	return <Skeleton className="rounded-lg border p-6 h-[394px] col-span-2 lg:col-span-1" />;
};
