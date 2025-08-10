"use client";

import { Children, FC, isValidElement, ReactElement, ReactNode, useState } from "react";
import {
	AlertCircle,
	CheckCircle2,
	ChevronDown,
	Clock,
	CreditCard,
	Info,
	MapPin,
	Package,
	PackageCheck,
	Phone,
	ShoppingCart,
	Truck,
	TruckIcon,
	User,
	Zap
} from "lucide-react";

import type { OrdersResponse } from "@entities/orders";

import { Card, CardContent, CardHeader } from "@shared/ui/card";
import { Badge } from "@shared/ui/badge";
import { Button } from "@shared/ui/button";
import { Separator } from "@shared/ui/separator";
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from "@shared/ui/collapsible";
import { Avatar, AvatarFallback } from "@shared/ui/avatar";
import { cn, formatCurrency, formatDate } from "@shared/lib/utils";

import { getStatusInfo } from "../../lib";
import { OrderStatus } from "../../model";

import { ManagementActions } from "./management-actions";
import { ContextActions } from "./context-actions";

type OrderRowCardProps = {
	order: OrdersResponse;
	children: ReactNode;
};

type OrderRowCardComponents = {
	ManagementActions: typeof ManagementActions;
	ContextActions: typeof ContextActions;
};

type OrderRowCard = FC<OrderRowCardProps> & OrderRowCardComponents;

export const OrderRowCard: OrderRowCard = ({ order, children }) => {
	const [isProductsOpen, setIsProductsOpen] = useState(false);
	const [isPaymentsOpen, setIsPaymentsOpen] = useState(false);

	console.log(order);

	const isFullyPaid = order.paidAmount >= order.totalAmount;
	const remainingAmount = Math.max(0, order.totalAmount - order.paidAmount);
	const statusInfo = getStatusInfo(order.status);

	// Use payments from the order response - check both 'payments' and 'payment' fields
	const payments = (order as any).payments || order.payment || [];

	const childrenArray = Children.toArray(children);
	const contextActions = childrenArray.find(
		(child) => isValidElement(child) && child.type === ContextActions
	) as ReactElement | undefined;
	const managementActions = childrenArray.find(
		(child) => isValidElement(child) && child.type === ManagementActions
	) as ReactElement | undefined;

	// Helper function to get payment status info
	const getPaymentStatusInfo = () => {
		if (isFullyPaid) {
			return {
				variant: "default" as const,
				className: "bg-green-50 text-green-700 border-green-200",
				icon: <CheckCircle2 className="h-3 w-3" />,
				text: "Fully Paid"
			};
		} else if (order.paidAmount > 0) {
			return {
				variant: "secondary" as const,
				className: "bg-amber-50 text-amber-700 border-amber-200",
				icon: <AlertCircle className="h-3 w-3" />,
				text: `${formatCurrency(remainingAmount)} due`
			};
		} else {
			return {
				variant: "destructive" as const,
				className: "bg-red-50 text-red-700 border-red-200",
				icon: <AlertCircle className="h-3 w-3" />,
				text: "Payment Pending"
			};
		}
	};

	const paymentStatus = getPaymentStatusInfo();

	// Helper function to get payment status badge info
	const getPaymentStatusBadge = (status: string) => {
		switch (status.toLowerCase()) {
			case "paid":
			case "completed":
				return {
					variant: "default" as const,
					className: "bg-green-50 text-green-700 border-green-200",
					label: "Paid"
				};
			case "pending":
				return {
					variant: "secondary" as const,
					className: "bg-amber-50 text-amber-700 border-amber-200",
					label: "Pending"
				};
			case "failed":
				return {
					variant: "destructive" as const,
					className: "bg-red-50 text-red-700 border-red-200",
					label: "Failed"
				};
			case "expired":
				return {
					variant: "outline" as const,
					className: "bg-gray-50 text-gray-700 border-gray-200",
					label: "Expired"
				};
			default:
				return {
					variant: "outline" as const,
					className: "",
					label: status
				};
		}
	};

	// Helper function to check if a status is completed
	const isStatusCompleted = (targetStatus: OrderStatus): boolean => {
		const statusOrder = [
			OrderStatus.Pending,
			OrderStatus.Confirmed,
			OrderStatus.Processing,
			OrderStatus.ReadyForShipment,
			OrderStatus.Shipped,
			OrderStatus.OutForDelivery,
			OrderStatus.Delivered,
			OrderStatus.Completed
		];

		const currentIndex = statusOrder.indexOf(order.status as OrderStatus);
		const targetIndex = statusOrder.indexOf(targetStatus);

		return currentIndex >= targetIndex;
	};

	// Timeline data based on your actual order statuses
	const timelineItems = [
		{
			label: "Confirmed",
			status: OrderStatus.Confirmed,
			completed: isStatusCompleted(OrderStatus.Confirmed),
			color: "bg-green-500",
			icon: <CheckCircle2 className="h-2.5 w-2.5 text-white" />
		},
		{
			label: "Processing",
			status: OrderStatus.Processing,
			completed: isStatusCompleted(OrderStatus.Processing),
			color: "bg-blue-500",
			icon: <Zap className="h-2.5 w-2.5 text-white" />
		},
		{
			label: "Ready",
			status: OrderStatus.ReadyForShipment,
			completed: isStatusCompleted(OrderStatus.ReadyForShipment),
			color: "bg-amber-500",
			icon: <PackageCheck className="h-2.5 w-2.5 text-white" />
		},
		{
			label: "Shipped",
			status: OrderStatus.Shipped,
			completed: isStatusCompleted(OrderStatus.Shipped),
			color: "bg-purple-500",
			icon: <TruckIcon className="h-2.5 w-2.5 text-white" />
		},
		{
			label: "Delivered",
			status: OrderStatus.Delivered,
			completed: isStatusCompleted(OrderStatus.Delivered),
			color: "bg-emerald-500",
			icon: <Package className="h-2.5 w-2.5 text-white" />
		}
	];

	// Filter timeline based on current status - don't show future steps for certain statuses
	const getVisibleTimelineItems = () => {
		if (
			[OrderStatus.Cancelled, OrderStatus.Returned, OrderStatus.Failed].includes(
				order.status as OrderStatus
			)
		) {
			// For terminal statuses, show only up to the last completed step
			return timelineItems.filter((item) => item.completed);
		}
		return timelineItems;
	};

	const visibleTimelineItems = getVisibleTimelineItems();

	return (
		<Card>
			<CardHeader className="pb-3">
				<div className="flex items-start justify-between">
					{/* Left: Order Info */}
					<div className="flex items-start space-x-4 flex-1">
						<Avatar className="h-12 w-12 bg-primary/10">
							<AvatarFallback className="bg-primary/10 text-primary">
								<ShoppingCart className="h-6 w-6" />
							</AvatarFallback>
						</Avatar>

						<div className="flex-1 space-y-2">
							<div className="flex items-center space-x-3 flex-wrap">
								<h3 className="text-lg font-semibold tracking-tight">
									#{order.orderNumber.replace("ORDER-", "")}
								</h3>
								<Badge className={statusInfo.color} variant="secondary">
									{statusInfo.label}
								</Badge>
								{payments.length > 0 && (
									<Badge
										variant="outline"
										className={cn(
											isFullyPaid
												? "border-green-200 text-green-700 bg-green-50"
												: "border-amber-200 text-amber-700 bg-amber-50"
										)}
									>
										{payments.length} Payment{payments.length > 1 ? "s" : ""}
									</Badge>
								)}
							</div>

							<div className="space-y-1">
								{/* Client Info */}
								<div className="flex items-center space-x-2 text-sm text-muted-foreground">
									<User className="h-4 w-4" />
									<span className="font-medium text-foreground">
										{order.client?.clientFirstName}{" "}
										{order.client?.clientLastName}
									</span>
									<span className="text-muted-foreground">•</span>
									<span>{order.client?.clientEmail}</span>
								</div>

								{/* Phone */}
								{order.client?.clientPhoneNumber && (
									<div className="flex items-center space-x-2 text-sm text-muted-foreground">
										<Phone className="h-4 w-4" />
										<span>{order.client.clientPhoneNumber}</span>
									</div>
								)}

								{/* Address */}
								<div className="flex items-start space-x-2 text-sm text-muted-foreground">
									<MapPin className="h-4 w-4 mt-0.5 flex-shrink-0" />
									<span className="line-clamp-2">{order.shippingAddress}</span>
								</div>
							</div>
						</div>
					</div>

					{/* Right: Payment & Management */}
					<div className="flex flex-col items-end space-y-3">
						<div className="text-right space-y-1">
							<div className="text-2xl font-bold">
								{formatCurrency(order.totalAmount)}
							</div>
							<div className="text-sm text-muted-foreground">
								Paid: {formatCurrency(order.paidAmount)}
							</div>
						</div>

						<Badge
							variant={paymentStatus.variant}
							className={cn("gap-1", paymentStatus.className)}
						>
							{paymentStatus.icon}
							{paymentStatus.text}
						</Badge>

						{order.outstandingAmount > 0 && (
							<Badge variant="destructive" className="text-xs">
								Outstanding: {formatCurrency(order.outstandingAmount)}
							</Badge>
						)}

						{managementActions}
					</div>
				</div>
			</CardHeader>

			<CardContent className="space-y-4">
				{visibleTimelineItems.length > 0 && (
					<div className="space-y-4">
						<h4 className="text-sm font-medium text-foreground">Order Progress</h4>
						<div className="relative px-4 pb-12">
							{/* Fixed height container to prevent layout shifts */}
							<div className="flex items-center justify-between relative h-6">
								{visibleTimelineItems.map((item, index) => (
									<div
										key={item.status}
										className="flex flex-col items-center relative z-20"
									>
										{/* Timeline dot */}
										<div
											className={cn(
												"h-6 w-6 rounded-full border-2 flex items-center justify-center shadow-sm",
												item.completed
													? cn(item.color, "border-transparent")
													: "bg-white border-muted-foreground"
											)}
										>
											{item.completed ? (
												item.icon
											) : (
												<div className="h-2 w-2 rounded-full bg-muted-foreground" />
											)}
										</div>

										{/* Connecting line to next dot */}
										{index < visibleTimelineItems.length - 1 && (
											<div
												className="absolute top-3 left-6 h-0.5 -z-10"
												style={{
													width: `calc(100vw / ${visibleTimelineItems.length} - 24px)`,
													maxWidth: "300px"
												}}
											>
												<div
													className={cn(
														"h-full transition-all duration-300",
														item.completed &&
															visibleTimelineItems[index + 1]
																?.completed
															? "bg-primary"
															: item.completed
																? "bg-gradient-to-r from-primary to-muted"
																: "bg-muted"
													)}
												/>
											</div>
										)}

										{/* Timeline labels - positioned absolutely to prevent layout shifts */}
										<div className="absolute top-8 left-1/2 transform -translate-x-1/2 text-center min-w-max">
											<div
												className={cn(
													"text-xs font-medium whitespace-nowrap",
													item.completed
														? "text-foreground"
														: "text-muted-foreground"
												)}
											>
												{item.label}
											</div>

											{/* Status label - always reserve space */}
											<div className="h-4 mt-1 flex items-center justify-center">
												{item.completed && item.status === order.status && (
													<div className="text-xs text-primary font-medium">
														Current
													</div>
												)}
												{!item.completed && (
													<div className="text-xs text-muted-foreground">
														Pending
													</div>
												)}
											</div>
										</div>
									</div>
								))}
							</div>
						</div>
					</div>
				)}

				{/* Special status messages for terminal states */}
				{[OrderStatus.Cancelled, OrderStatus.Returned, OrderStatus.Failed].includes(
					order.status as OrderStatus
				) && (
					<Card className="bg-destructive/5 border-destructive/20">
						<CardContent className="pt-4">
							<div className="flex items-center space-x-2 text-sm">
								<AlertCircle className="h-4 w-4 text-destructive" />
								<span className="font-medium text-destructive">
									Order {order.status}
								</span>
								{order.status === OrderStatus.Cancelled &&
									order.cancellationReason && (
										<span className="text-muted-foreground">
											- {order.cancellationReason}
										</span>
									)}
								{order.status === OrderStatus.Returned && order.returnReason && (
									<span className="text-muted-foreground">
										- {order.returnReason}
									</span>
								)}
							</div>
						</CardContent>
					</Card>
				)}
				{/*{Fix for refund reason we need to display it}*/}
				{order.refundReason && (
					<Card className="bg-destructive/5 border-destructive/20">
						<CardContent className="pt-4">
							<div className="flex items-center space-x-2 text-sm">
								<AlertCircle className="h-4 w-4 text-destructive" />
								<span className="font-medium text-destructive">
									Order {order.status}
								</span>
								{order.status === OrderStatus.Cancelled &&
									order.cancellationReason && (
										<span className="text-muted-foreground">
											- {order.refundReason}
										</span>
									)}
								{order.status === OrderStatus.Returned && order.returnReason && (
									<span className="text-muted-foreground">
										- {order.refundReason}
									</span>
								)}
							</div>
						</CardContent>
					</Card>
				)}

				{/* Payments Section */}
				{payments.length > 0 && (
					<Collapsible open={isPaymentsOpen} onOpenChange={setIsPaymentsOpen}>
						<CollapsibleTrigger asChild>
							<Button variant="ghost" className="w-full justify-between p-0 h-auto">
								<div className="flex items-center space-x-2">
									<CreditCard className="h-4 w-4" />
									<span className="font-medium">
										Payments ({payments.length})
									</span>
								</div>
								<ChevronDown
									className={cn(
										"h-4 w-4 transition-transform duration-200",
										isPaymentsOpen && "rotate-180"
									)}
								/>
							</Button>
						</CollapsibleTrigger>
						<CollapsibleContent className="space-y-0">
							<Card className="mt-3">
								<CardContent className="pt-4">
									<div className="space-y-4">
										{payments.map((payment: any, index: number) => {
											const statusBadge = getPaymentStatusBadge(
												payment.paymentStatus
											);

											return (
												<div
													key={payment.paymentId}
													className="flex items-start justify-between py-3 border-b last:border-b-0"
												>
													<div className="flex-1 space-y-2">
														<div className="flex items-center space-x-3">
															<div className="flex items-center space-x-2">
																<CreditCard className="h-3 w-3" />
																<span className="font-medium text-sm">
																	Payment #{index + 1}
																</span>
															</div>
															<Badge
																variant={statusBadge.variant}
																className={cn(
																	"text-xs",
																	statusBadge.className
																)}
															>
																{statusBadge.label}
															</Badge>
														</div>

														<div className="text-xs text-muted-foreground">
															Payment ID: {payment.paymentId}
														</div>
													</div>

													<div className="text-right space-y-1">
														<div className="font-semibold">
															{formatCurrency(
																payment.paymentTotalAmount
															)}
														</div>
														<div className="text-xs text-muted-foreground">
															{payment.paymentStatus}
														</div>
													</div>
												</div>
											);
										})}

										{/* Payment Summary */}
										<div className="pt-3 border-t bg-muted/30 -mx-4 px-4 pb-0">
											<div className="flex justify-between items-center text-sm">
												<span className="font-medium">Total Paid:</span>
												<span className="font-semibold text-green-600">
													{formatCurrency(
														payments.reduce(
															(sum: number, payment: any) =>
																payment.paymentStatus.toLowerCase() ===
																"paid"
																	? sum +
																		payment.paymentTotalAmount
																	: sum,
															0
														)
													)}
												</span>
											</div>
											{remainingAmount > 0 && (
												<div className="flex justify-between items-center text-sm mt-1">
													<span className="font-medium">Remaining:</span>
													<span className="font-semibold text-amber-600">
														{formatCurrency(remainingAmount)}
													</span>
												</div>
											)}
										</div>
									</div>
								</CardContent>
							</Card>
						</CollapsibleContent>
					</Collapsible>
				)}

				{/* Shipping Info */}
				{(order.trackingNumber || order.courier || order.estimatedDeliveryDate) && (
					<Card className="bg-muted/50">
						<CardContent className="pt-4">
							<div className="flex flex-wrap items-center gap-4 text-sm">
								{order.trackingNumber && (
									<div className="flex items-center space-x-2">
										<Truck className="h-4 w-4 text-muted-foreground" />
										<span className="font-medium">Tracking:</span>
										<Badge variant="outline" className="font-mono text-xs">
											{order.trackingNumber}
										</Badge>
									</div>
								)}

								{order.courier && (
									<div className="flex items-center space-x-2">
										<Package className="h-4 w-4 text-muted-foreground" />
										<span className="font-medium">Courier:</span>
										<Badge variant="secondary">{order.courier}</Badge>
									</div>
								)}

								{order.estimatedDeliveryDate && (
									<div className="flex items-center space-x-2">
										<Clock className="h-4 w-4 text-muted-foreground" />
										<span className="font-medium">Est. Delivery:</span>
										<span className="text-muted-foreground">
											{formatDate(order.estimatedDeliveryDate)}
										</span>
									</div>
								)}
							</div>
						</CardContent>
					</Card>
				)}

				{/* Products Section */}
				{order.orderProducts && order.orderProducts.length > 0 && (
					<Collapsible open={isProductsOpen} onOpenChange={setIsProductsOpen}>
						<CollapsibleTrigger asChild>
							<Button variant="ghost" className="w-full justify-between p-0 h-auto">
								<div className="flex items-center space-x-2">
									<Package className="h-4 w-4" />
									<span className="font-medium">
										Products ({order.orderProducts.length})
									</span>
								</div>
								<ChevronDown
									className={cn(
										"h-4 w-4 transition-transform duration-200",
										isProductsOpen && "rotate-180"
									)}
								/>
							</Button>
						</CollapsibleTrigger>
						<CollapsibleContent className="space-y-0">
							<Card className="mt-3">
								<CardContent className="pt-4">
									<div className="space-y-3">
										{order.orderProducts.map((product, index) => (
											<div
												key={`${product.productId}-${index}`}
												className="flex items-center justify-between py-2 border-b last:border-b-0"
											>
												<div className="flex-1 space-y-1">
													<div className="font-medium text-sm">
														{product.productName}
													</div>
													<div className="text-sm text-muted-foreground">
														{formatCurrency(product.unitPriceAmount)} ×{" "}
														{product.quantity}
													</div>
												</div>
												<div className="font-semibold">
													{formatCurrency(product.totalPriceAmount)}
												</div>
											</div>
										))}
									</div>
								</CardContent>
							</Card>
						</CollapsibleContent>
					</Collapsible>
				)}

				{/* Context Actions */}
				{contextActions && <>{contextActions}</>}

				{/* Additional Info */}
				{(order.info ||
					order.cancellationReason ||
					order.returnReason ||
					order.refundReason) && (
					<>
						<Separator />
						<div className="space-y-3">
							{order.info && (
								<div className="flex items-start space-x-2">
									<Info className="h-4 w-4 text-blue-500 mt-0.5 flex-shrink-0" />
									<div className="text-sm">
										<span className="font-medium">Info:</span>
										<span className="text-muted-foreground ml-2">
											{order.info}
										</span>
									</div>
								</div>
							)}

							{/*{order.cancellationReason && (*/}
							{/*	<div className="flex items-start space-x-2">*/}
							{/*		<AlertCircle className="h-4 w-4 text-destructive mt-0.5 flex-shrink-0" />*/}
							{/*		<div className="text-sm">*/}
							{/*			<span className="font-medium">Cancellation:</span>*/}
							{/*			<span className="text-muted-foreground ml-2">*/}
							{/*				{order.cancellationReason}*/}
							{/*			</span>*/}
							{/*		</div>*/}
							{/*	</div>*/}
							{/*)}*/}

							{order.returnReason && (
								<div className="flex items-start space-x-2">
									<AlertCircle className="h-4 w-4 text-orange-500 mt-0.5 flex-shrink-0" />
									<div className="text-sm">
										<span className="font-medium">Return:</span>
										<span className="text-muted-foreground ml-2">
											{order.returnReason}
										</span>
									</div>
								</div>
							)}

							{order.refundReason && (
								<div className="flex items-start space-x-2">
									<AlertCircle className="h-4 w-4 text-purple-500 mt-0.5 flex-shrink-0" />
									<div className="text-sm">
										<span className="font-medium">Refund:</span>
										<span className="text-muted-foreground ml-2">
											{order.refundReason}
										</span>
									</div>
								</div>
							)}
						</div>
					</>
				)}
			</CardContent>
		</Card>
	);
};

OrderRowCard.ManagementActions = ManagementActions;
OrderRowCard.ContextActions = ContextActions;
