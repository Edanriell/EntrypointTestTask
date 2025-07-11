"use client";

import { Children, FC, isValidElement, ReactElement, ReactNode } from "react";
import { Calendar, Package } from "lucide-react";

import { ManagementActions } from "@entities/products/ui/product-row-card/management-actions";
import { QuickActions } from "@entities/products/ui/product-row-card/quick-actions";
import { Product } from "@entities/products/model";
import { getStockStatus } from "@entities/products/lib/functions";

import { Card, CardContent } from "@shared/ui/card";
import { Badge } from "@shared/ui/badge";
import { formatDate } from "@shared/lib/utils";

interface ProductRowCardProps {
	product: Product;
	children: ReactNode;
}

type ProductRowCardComponents = {
	ManagementActions: typeof ManagementActions;
	QuickActions: typeof QuickActions;
};

type ProductRowCard = FC<ProductRowCardProps> & ProductRowCardComponents;

export const ProductRowCard: ProductRowCard = ({ product, children }) => {
	const stockInfo = getStockStatus(product.stock, product.reserved);

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
				<div className="flex items-center justify-between gap-4">
					{/* Product Info Section */}
					<div className="flex items-center gap-4 flex-1 min-w-0">
						{/* Product Icon */}
						<div className="h-12 w-12 flex-shrink-0 bg-primary/10 rounded-lg flex items-center justify-center">
							<Package className="h-6 w-6 text-primary" />
						</div>

						{/* Basic Info */}
						<div className="flex-1 min-w-0">
							<div className="flex items-center gap-2 mb-1">
								<h3 className="font-semibold text-foreground truncate">
									{product.name}
								</h3>
								<Badge className={stockInfo.color} variant="secondary">
									{stockInfo.status}
								</Badge>
							</div>

							{product.description && (
								<div className="text-sm text-muted-foreground truncate">
									{product.description}
								</div>
							)}
						</div>
					</div>

					{/* Quick Actions */}
					{quickActions}

					{/* Price Input Section */}
					{/*<div className="flex flex-col gap-1 min-w-0 flex-shrink-0 ">*/}
					{/*	<div className="text-xs text-muted-foreground text-center">Price</div>*/}
					{/*	{editingPrice ? (*/}
					{/*		<Input*/}
					{/*			type="number"*/}
					{/*			min="0"*/}
					{/*			value={tempPrice}*/}
					{/*			onChange={(e) => setTempPrice(e.target.value)}*/}
					{/*			onBlur={handlePriceSubmit}*/}
					{/*			onKeyDown={(e) => {*/}
					{/*				if (e.key === "Enter") handlePriceSubmit();*/}
					{/*				if (e.key === "Escape") {*/}
					{/*					setTempPrice(product.unitPrice.toString());*/}
					{/*					setEditingPrice(false);*/}
					{/*				}*/}
					{/*			}}*/}
					{/*			className="w-20 h-8 text-sm text-center no-arrows"*/}
					{/*			autoFocus*/}
					{/*		/>*/}
					{/*	) : (*/}
					{/*		<div*/}
					{/*			className="flex items-center gap-1 cursor-pointer hover:bg-muted/50 px-2 py-1 rounded text-sm font-medium"*/}
					{/*			onClick={() => setEditingPrice(true)}*/}
					{/*		>*/}
					{/*			<span>{formatCurrency(product.unitPrice)}</span>*/}
					{/*		</div>*/}
					{/*	)}*/}
					{/*</div>*/}

					{/* Stock Input Section */}
					{/*<div className="flex flex-col gap-1 min-w-0 flex-shrink-0">*/}
					{/*	<div className="text-xs text-muted-foreground text-center">Stock</div>*/}
					{/*	{editingStock ? (*/}
					{/*		<Input*/}
					{/*			type="number"*/}
					{/*			min="0"*/}
					{/*			value={tempStock}*/}
					{/*			onChange={(e) => setTempStock(e.target.value)}*/}
					{/*			onBlur={handleStockSubmit}*/}
					{/*			onKeyDown={(e) => {*/}
					{/*				if (e.key === "Enter") handleStockSubmit();*/}
					{/*				if (e.key === "Escape") {*/}
					{/*					setTempStock(product.unitsInStock.toString());*/}
					{/*					setEditingStock(false);*/}
					{/*				}*/}
					{/*			}}*/}
					{/*			className="w-16 h-8 text-sm text-center no-arrows"*/}
					{/*			autoFocus*/}
					{/*		/>*/}
					{/*	) : (*/}
					{/*		<div*/}
					{/*			className="flex items-center gap-1 cursor-pointer hover:bg-muted/50 px-2 py-1 rounded text-sm font-medium"*/}
					{/*			onClick={() => setEditingStock(true)}*/}
					{/*		>*/}
					{/*			<Archive className="h-3 w-3" />*/}
					{/*			<span>{product.unitsInStock}</span>*/}
					{/*		</div>*/}
					{/*	)}*/}
					{/*</div>*/}

					{/* Reserved Stock Input Section */}
					{/*<div className="flex flex-col gap-1 min-w-0 flex-shrink-0 xl:mr-8">*/}
					{/*	<div className="text-xs text-muted-foreground text-center">Reserved</div>*/}
					{/*	{editingReservedStock ? (*/}
					{/*		<Input*/}
					{/*			type="number"*/}
					{/*			min="0"*/}
					{/*			value={tempReservedStock}*/}
					{/*			onChange={(e) => setTempReservedStock(e.target.value)}*/}
					{/*			onBlur={handleReservedStockSubmit}*/}
					{/*			onKeyDown={(e) => {*/}
					{/*				if (e.key === "Enter") handleReservedStockSubmit();*/}
					{/*				if (e.key === "Escape") {*/}
					{/*					setTempReservedStock(product.unitsOnOrder.toString());*/}
					{/*					setEditingReservedStock(false);*/}
					{/*				}*/}
					{/*			}}*/}
					{/*			className="w-16 h-8 text-sm text-center no-arrows"*/}
					{/*			autoFocus*/}
					{/*		/>*/}
					{/*	) : (*/}
					{/*		<div*/}
					{/*			className="flex items-center gap-1 cursor-pointer hover:bg-muted/50 px-2 py-1 rounded text-sm font-medium"*/}
					{/*			onClick={() => setEditingReservedStock(true)}*/}
					{/*		>*/}
					{/*			<AlertTriangle className="h-3 w-3" />*/}
					{/*			<span>{product.unitsOnOrder}</span>*/}
					{/*		</div>*/}
					{/*	)}*/}
					{/*</div>*/}

					{/* Available Stock Display */}
					<div className="hidden lg:flex flex-col gap-1 text-sm min-w-0 flex-shrink-0 xl:mr-8">
						<div className="text-center">
							<div className="font-semibold text-foreground">
								{product.availableQuantity}
							</div>
							<div className="text-xs text-muted-foreground">Available</div>
						</div>
					</div>

					{/* Created Date Section */}
					{product.createdAt && (
						<div className="hidden sm:flex items-center gap-1 text-sm text-muted-foreground flex-shrink-0">
							<Calendar className="h-3 w-3" />
							<span>{formatDate(product.createdAt)}</span>
						</div>
					)}

					{/* Management Actions */}
					{managementActions}
				</div>
			</CardContent>
		</Card>
	);
};

ProductRowCard.ManagementActions = ManagementActions;
ProductRowCard.QuickActions = QuickActions;
