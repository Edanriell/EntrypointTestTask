import { FC, memo, useMemo } from "react";
import { DollarSign } from "lucide-react";

import type { Order } from "@entities/orders/model";

import { Card, CardContent, CardHeader, CardTitle } from "@shared/ui/card";
import { Skeleton } from "@shared/ui/skeleton";
import { Badge } from "@shared/ui/badge";

import { calculateTotalRevenue } from "./lib";

type TotalRevenueProps = {
	data?: Array<Order>;
	error: Error | null;
	isPending: boolean;
	isError: boolean;
};

const TotalRevenue: FC<TotalRevenueProps> = ({ data, error, isPending, isError }) => {
	// const info = useRenderInfo("TotalRevenue");
	const totalRevenue = useMemo(() => (data ? calculateTotalRevenue(data) : 0), [data]);

	if (isPending) {
		return (
			<Card x-chunk="A card showing the total revenue in USD.">
				<Skeleton className="w-full h-[109.8px] rounded-lg" />
			</Card>
		);
	}

	if (isError) {
		return (
			<Card x-chunk="A card showing the total revenue in USD.">
				<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
					<CardTitle className="text-sm font-medium">Total Revenue</CardTitle>
					<DollarSign className="h-4 w-4 text-muted-foreground" />
				</CardHeader>
				<CardContent className="flex items-center justify-center">
					<Badge className="mt-4 text-left px-7 py-1 text-[12px]" variant="destructive">
						Error: {error?.message}
					</Badge>
				</CardContent>
			</Card>
		);
	}

	return (
		<Card x-chunk="A card showing the total revenue in USD.">
			<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
				<CardTitle className="text-sm font-medium">Total Revenue</CardTitle>
				<DollarSign className="h-4 w-4 text-muted-foreground" />
			</CardHeader>
			<CardContent>
				<div className="text-2xl font-bold">{totalRevenue}</div>
			</CardContent>
		</Card>
	);
};

export const MemoizedTotalRevenue = memo(TotalRevenue);
