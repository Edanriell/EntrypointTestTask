"use client";

import { FC } from "react";
import { Package, ShoppingCart } from "lucide-react";

import { Accordion, AccordionContent, AccordionItem, AccordionTrigger } from "@shared/ui/accordion";
import { Card, CardContent } from "@shared/ui/card";
import { Separator } from "@shared/ui/separator";
import { cn, formatCurrency } from "@shared/lib/utils";
import { Badge } from "@shared/ui/badge";

import { OrderProductResponse } from "../../../api";

type OrderProductsProps = {
	orderProducts: Array<OrderProductResponse>;
};

export const OrderProducts: FC<OrderProductsProps> = ({ orderProducts }) => {
	if (!orderProducts || orderProducts.length === 0) return null;

	const totalAmount = orderProducts.reduce((sum, p) => sum + p.totalPriceAmount, 0);

	return (
		<Accordion type="single" collapsible className="w-full">
			<AccordionItem value="products">
				<AccordionTrigger className="flex items-center justify-between px-2 py-2 rounded-lg hover:bg-muted/40">
					<div className="flex items-center space-x-2">
						<Package className="h-4 w-4 text-primary" />
						<span className="font-semibold text-foreground">
							Products ({orderProducts.length})
						</span>
					</div>
				</AccordionTrigger>
				<AccordionContent>
					<Card className="mt-3">
						<CardContent className="p-4 space-y-3">
							{orderProducts.map((product, index) => (
								<div
									key={`${product.productId}-${index}`}
									className={cn(
										"flex flex-col sm:flex-row sm:items-center justify-between gap-2 py-3",
										index < orderProducts.length - 1 && "border-b border-muted"
									)}
								>
									<div className="flex-1 space-y-1">
										<div className="flex items-center gap-2">
											<ShoppingCart className="h-4 w-4 text-muted-foreground" />
											<span className="font-medium text-sm">
												{product.productName}
											</span>
										</div>
										<div className="text-xs text-muted-foreground pl-6">
											{formatCurrency(product.unitPriceAmount)} ×{" "}
											{product.quantity}
										</div>
									</div>
									<div className="flex flex-col items-end gap-1">
										<span className="font-semibold text-sm">
											{formatCurrency(product.totalPriceAmount)}
										</span>
										<Badge
											variant="outline"
											className="text-xs text-primary border-primary/20 bg-primary/5"
										>
											Item #{index + 1}
										</Badge>
									</div>
								</div>
							))}
							<Separator className="my-3" />
							<div className="flex justify-between items-center text-sm">
								<span className="font-medium">Total Amount:</span>
								<span className="font-semibold text-green-600">
									{formatCurrency(totalAmount)}
								</span>
							</div>
						</CardContent>
					</Card>
				</AccordionContent>
			</AccordionItem>
		</Accordion>
	);
};
