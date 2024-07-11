import { FC, memo, useMemo } from "react";
import { CreditCard } from "lucide-react";

import type { Order } from "@entities/orders/model";

import { formatNumberWithSeparators } from "@shared/lib";
import { Card, CardContent, CardHeader, CardTitle } from "@shared/ui/card";
import { Skeleton } from "@shared/ui/skeleton";
import { Badge } from "@shared/ui/badge";

type TotalOrdersProps = {
	data?: Array<Order>;
	error: Error | null;
	isPending: boolean;
	isError: boolean;
};

const TotalOrders: FC<TotalOrdersProps> = ({ data, error, isPending, isError }) => {
	// const info = useRenderInfo("TotalOrders");
	const totalOrders = useMemo(
		() => (data ? formatNumberWithSeparators(data?.length, 3) : 0),
		[data]
	);

	if (isPending) {
		return (
			<Card x-chunk="A card showing the total orders count.">
				<Skeleton className="w-full h-[109.8px] rounded-lg" />
			</Card>
		);
	}

	if (isError) {
		return (
			<Card x-chunk="A card showing the total orders count.">
				<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
					<CardTitle className="text-sm font-medium">Total Orders</CardTitle>
					<CreditCard className="h-4 w-4 text-muted-foreground" />
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
		<Card x-chunk="A card showing the total orders count.">
			<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
				<CardTitle className="text-sm font-medium">Total Orders</CardTitle>
				<CreditCard className="h-4 w-4 text-muted-foreground" />
			</CardHeader>
			<CardContent>
				<div className="text-2xl font-bold">{totalOrders}</div>
			</CardContent>
		</Card>
	);
};

export const MemoizedTotalOrders = memo(TotalOrders);
