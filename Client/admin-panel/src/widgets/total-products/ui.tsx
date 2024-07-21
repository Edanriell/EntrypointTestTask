"use client";

import { FC } from "react";
import { ShoppingBasket } from "lucide-react";
import { useSession } from "next-auth/react";
import { useQuery } from "@tanstack/react-query";

import type { Product } from "@entities/products/model";
import { getAllProducts } from "@entities/products/api";

import { Card, CardContent, CardHeader, CardTitle } from "@shared/ui/card";
import { Skeleton } from "@shared/ui/skeleton";
import { Badge } from "@shared/ui/badge";
import { formatNumberWithSeparators } from "@shared/lib";

export const TotalProducts: FC = () => {
	// const info = useRenderInfo("TotalRevenue");
	const { data: session, status } = useSession();
	const userId = session?.user.id;
	const accessToken = session?.accessToken;

	const {
		data: productsData,
		error: productsError,
		isPending: isProductsPending,
		isError: isProductsError
	} = useQuery({
		queryKey: ["getAllProducts", userId, accessToken],
		queryFn: (): Promise<Array<Product>> => getAllProducts(accessToken!),
		enabled: !!userId && !!accessToken
	});

	if (isProductsPending) {
		return (
			<Card x-chunk="A card showing the total products count.">
				<Skeleton className="w-full h-[109.8px] rounded-lg" />
			</Card>
		);
	}

	if (isProductsError) {
		return (
			<Card x-chunk="A card showing the total products count.">
				<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
					<CardTitle className="text-sm font-medium">Total Products</CardTitle>
					<ShoppingBasket className="h-4 w-4 text-muted-foreground" />
				</CardHeader>
				<CardContent>
					<Badge className="mt-4 text-left px-7 py-1 text-[12px]" variant="destructive">
						Error: {productsError?.message}
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
				<div className="text-2xl font-bold">
					{formatNumberWithSeparators(productsData.length, 3)}
				</div>
			</CardContent>
		</Card>
	);
};
