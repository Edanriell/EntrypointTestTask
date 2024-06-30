import { FC } from "react";

import { Card, CardContent, CardHeader, CardTitle, Skeleton } from "@shared/ui";
import { CreditCard } from "lucide-react";

import { formatNumberWithSeparators } from "@shared/lib";

type TotalOrdersProps = {
	data: any;
	error: any;
	isPending: boolean;
	isError: boolean;
};

export const TotalOrders: FC<TotalOrdersProps> = ({ data, error, isPending, isError }) => {
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
					<div className="text-1xl font-bold">Error: {error.message}</div>
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
				<div className="text-2xl font-bold">{formatNumberWithSeparators(data.length, 3)}</div>
			</CardContent>
		</Card>
	);
};
