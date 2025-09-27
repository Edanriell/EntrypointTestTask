import { FC } from "react";

import { Skeleton } from "@shared/ui/skeleton";

export const TotalOrdersSkeleton: FC = () => {
	return <Skeleton className="rounded-lg border p-6 h-[130px]" />;
};
