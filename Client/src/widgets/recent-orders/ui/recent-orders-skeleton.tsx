import { FC } from "react";

import { Skeleton } from "@shared/ui/skeleton";

export const RecentOrdersSkeleton: FC = () => {
	return <Skeleton className="rounded-lg border p-6 h-[500px]" />;
};
