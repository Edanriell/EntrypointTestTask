import { FC, memo, useMemo } from "react";
import { ShoppingBasket } from "lucide-react";

import type { Product } from "@entities/products/model";

import { Card, CardContent, CardHeader, CardTitle } from "@shared/ui/card";
import { Skeleton } from "@shared/ui/skeleton";
import { Badge } from "@shared/ui/badge";
import { formatNumberWithSeparators } from "@shared/lib";

type TotalProductsProps = {
	data?: Array<Product>;
	error: Error | null;
	isPending: boolean;
	isError: boolean;
};

const TotalProducts: FC<TotalProductsProps> = ({ data, error, isPending, isError }) => {
	// const info = useRenderInfo("TotalRevenue");
	const totalProducts = useMemo(
		() => (data ? formatNumberWithSeparators(data?.length, 3) : 0),
		[data]
	);

	if (isPending) {
		return (
			<Card x-chunk="A card showing the total products count.">
				<Skeleton className="w-full h-[109.8px] rounded-lg" />
			</Card>
		);
	}

	if (isError) {
		return (
			<Card x-chunk="A card showing the total products count.">
				<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
					<CardTitle className="text-sm font-medium">Total Products</CardTitle>
					<ShoppingBasket className="h-4 w-4 text-muted-foreground" />
				</CardHeader>
				<CardContent>
					<Badge className="mt-4 text-left px-7 py-1 text-[12px]" variant="destructive">
						Error: {error?.message}
					</Badge>
				</CardContent>
			</Card>
		);
	}

	return (
		<Card x-chunk="A card showing the total products count.">
			<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
				<CardTitle className="text-sm font-medium">Total Products</CardTitle>
				<ShoppingBasket className="h-4 w-4 text-muted-foreground" />
			</CardHeader>
			<CardContent>
				<div className="text-2xl font-bold">{totalProducts}</div>
			</CardContent>
		</Card>
	);
};

export const MemoizedTotalProducts = memo(TotalProducts);
