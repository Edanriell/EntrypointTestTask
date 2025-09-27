"use client";

import { Children, FC, isValidElement, ReactElement, ReactNode } from "react";
import { Calendar, Package, Percent } from "lucide-react";

import { ManagementActions } from "@entities/products/ui/product-row-card/management-actions";
import { QuickActions } from "@entities/products/ui/product-row-card/quick-actions";
import { Product, ProductStatus } from "@entities/products/model";
import { getStockStatus } from "@entities/products/lib/functions";

import { Card, CardContent } from "@shared/ui/card";
import { formatDate } from "@shared/lib/utils";
import { Badge } from "@shared/ui/badge";

type ProductRowCardProps = {
	product: Product;
	children: ReactNode;
};

type ProductRowCardComponents = {
	ManagementActions: typeof ManagementActions;
	QuickActions: typeof QuickActions;
};

type ProductRowCard = FC<ProductRowCardProps> & ProductRowCardComponents;

export const ProductRowCard: ProductRowCard = ({ product, children }) => {
	const stockInfo = getStockStatus(product.totalStock, product.reserved);

	const childrenArray = Children.toArray(children);

	const quickActions = childrenArray.find(
		(child) => isValidElement(child) && child.type === QuickActions
	) as ReactElement | undefined;
	const managementActions = childrenArray.find(
		(child) => isValidElement(child) && child.type === ManagementActions
	) as ReactElement | undefined;

	return (
		<Card className="w-full hover:shadow-md transition-shadow duration-200 pt-[unset] pb-[unset]">
			<CardContent className="pt-4 pb-4 pl-6 pr-6">
				<div className="flex items-center justify-between gap-2">
					<div className="flex items-center gap-4 flex-1 min-w-0">
						<div className="h-12 w-12 flex-shrink-0 bg-primary/10 rounded-lg flex items-center justify-center">
							<Package className="h-6 w-6 text-primary" />
						</div>
						<div className="flex-1 min-w-0">
							<div className="flex items-center gap-2 mb-1">
								<h3 className="font-semibold text-foreground truncate">
									{product.name}
								</h3>
								{product.status !== ProductStatus.Deleted && (
									<>
										<Badge className={stockInfo.color} variant="secondary">
											{stockInfo.status}
										</Badge>
										{product.status === ProductStatus.Discontinued && (
											<Badge
												className="bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-300"
												variant="secondary"
											>
												<Percent className="h-3 w-3 mr-1" />
												discounted
											</Badge>
										)}
									</>
								)}
							</div>
							{product.description && (
								<div className="text-sm text-muted-foreground truncate max-w-[420px]">
									{product.description}
								</div>
							)}
						</div>
					</div>
					{quickActions}
					{product.status !== ProductStatus.Deleted && (
						<div className="hidden lg:flex flex-col gap-1 text-sm min-w-0 flex-shrink-0 xl:mr-8">
							<div className="text-center">
								<div className="font-semibold text-foreground">
									{product.available}
								</div>
								<div className="text-xs text-muted-foreground">Available</div>
							</div>
						</div>
					)}
					{product.createdAt && (
						<div className="hidden sm:flex items-center gap-1 text-sm text-muted-foreground flex-shrink-0">
							<Calendar className="h-3 w-3" />
							<span>{formatDate(product.createdAt)}</span>
						</div>
					)}
					{managementActions}
				</div>
			</CardContent>
		</Card>
	);
};

ProductRowCard.ManagementActions = ManagementActions;
ProductRowCard.QuickActions = QuickActions;
