import { FC } from "react";

import { Skeleton } from "@shared/ui/skeleton";

type CustomersPageSkeletonProps = {
	pageSize?: number;
};

export const CustomersListSkeleton: FC<CustomersPageSkeletonProps> = ({ pageSize = 10 }) => {
	return (
		<div className="flex flex-col gap-y-[16px]">
			{Array.from({ length: pageSize }).map((_, index) => (
				<Skeleton
					key={index}
					className="shrink-0 grow-0 rounded-[14px]! h-[84px] w-full pt-[unset] pb-[unset]"
				/>
			))}
		</div>
	);
};
