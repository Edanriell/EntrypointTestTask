import { FC } from "react";

import { Skeleton } from "@shared/ui/skeleton";

export const ActiveProductsSkeleton: FC = () => {
	return <Skeleton className="rounded-lg border p-6" />;
};
