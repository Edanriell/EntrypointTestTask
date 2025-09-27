import { FC } from "react";

import { Skeleton } from "@shared/ui/skeleton";

export const MonthlyRevenueSkeleton: FC = () => {
	return <Skeleton className="rounded-lg border p-6 h-[130px] h-[130px]" />;
};
