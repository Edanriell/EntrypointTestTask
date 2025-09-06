import { FC } from "react";

import { ProductSkeleton } from "@entities/products";

type ProductsPageSkeletonProps = {
	pageSize?: number;
};

export const ProductsPageSkeleton: FC<ProductsPageSkeletonProps> = ({ pageSize = 10 }) => {
	return (
		<div className="flex flex-col gap-y-[16px]">
			{Array.from({ length: pageSize }).map((_, index) => (
				<ProductSkeleton key={index} />
			))}
		</div>
	);
};
