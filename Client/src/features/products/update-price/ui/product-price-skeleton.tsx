import { FC } from "react";

import { Skeleton } from "@shared/ui/skeleton";

export const ProductPriceSkeleton: FC = () => {
	return (
		<div className="flex flex-col gap-1 min-w-0 flex-shrink-0">
			<div className="text-xs text-muted-foreground text-center">Price</div>
			<div className="w-[80px] h-8 flex items-center justify-center">
				<Skeleton className="w-[50px] h-[20px]" />
			</div>
		</div>
	);
};
