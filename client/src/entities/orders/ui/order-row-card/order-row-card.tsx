"use client";

import { Children, FC, isValidElement, ReactElement, ReactNode, useState } from "react";
import { Calendar, MapPin, RefreshCw, ShoppingCart, User } from "lucide-react";

import { Card, CardContent } from "@shared/ui/card";
import { Badge } from "@shared/ui/badge";
import { getStatusInfo } from "@entities/orders/lib/functions";
import { formatCurrency, formatFullDate } from "@shared/lib/functions";
import { ManagementActions } from "@entities/orders/ui/order-row-card/management-actions";
import { ContextActions } from "@entities/orders/ui/order-row-card/context-actions";

export enum OrderStatus {
	PENDING = "PENDING",
	CONFIRMED = "CONFIRMED",
	PROCESSING = "PROCESSING",
	SHIPPED = "SHIPPED",
	DELIVERED = "DELIVERED",
	CANCELLED = "CANCELLED",
	RETURNED = "RETURNED",
	REFUNDED = "REFUNDED"
}

interface OrderRowCardProps {
	order: {
		id: number;
		createdAt: string;
		updatedAt: string;
		status: OrderStatus;
		shipAddress: string;
		orderInformation: string;
		totalAmount: number;
		paidAmount: number;
		customer: {
			id: number;
			name: string;
			email: string;
		};
		products?: Array<{
			id: number;
			productName: string;
			unitPrice: number;
			quantity: number;
		}> | null;
	};
	onEdit: (orderId: number) => void;
	onProcessPayment: (orderId: number, paymentAmount: number) => void;
	onStartProcessing: (orderId: number) => void;
	onShipOrder: (orderId: number, address: string) => void;
	onCancelOrder: (orderId: number) => void;
	onMarkAsDelivered: (orderId: number) => void;
	onReturnOrder: (orderId: number, reason: string) => void;
	onRefundPayment: (orderId: number) => void;
	children: ReactNode;
}

type OrderRowCardComponents = {
	ManagementActions: typeof ManagementActions;
	ContextActions: typeof ContextActions;
};

type OrderRowCard = FC<OrderRowCardProps> & OrderRowCardComponents;

export const OrderRowCard: OrderRowCard = ({
	order,
	onEdit,
	onProcessPayment,
	onStartProcessing,
	onShipOrder,
	onCancelOrder,
	onMarkAsDelivered,
	onReturnOrder,
	onRefundPayment,
	children
}) => {
	const [paymentAmount, setPaymentAmount] = useState("");
	const [shippingAddress, setShippingAddress] = useState(order.shipAddress);
	const [returnReason, setReturnReason] = useState("");
	const [editingAddress, setEditingAddress] = useState(false);
	const [editingReturn, setEditingReturn] = useState(false);

	const isFullyPaid = order.paidAmount >= order.totalAmount;
	const remainingAmount = Math.max(0, order.totalAmount - order.paidAmount);

	const handleProcessPayment = () => {
		const amount = parseFloat(paymentAmount);
		if (!isNaN(amount) && amount > 0) {
			onProcessPayment(order.id, amount);
			setPaymentAmount("");
		}
	};

	const handleShipOrder = () => {
		if (editingAddress && shippingAddress.trim()) {
			onShipOrder(order.id, shippingAddress.trim());
			setEditingAddress(false);
		} else {
			setEditingAddress(true);
		}
	};

	const handleReturnOrder = () => {
		if (editingReturn && returnReason.trim()) {
			onReturnOrder(order.id, returnReason.trim());
			setEditingReturn(false);
			setReturnReason("");
		} else {
			setEditingReturn(true);
		}
	};

	const statusInfo = getStatusInfo(order.status);

	const canProcessPayment =
		!isFullyPaid && [OrderStatus.PENDING, OrderStatus.CONFIRMED].includes(order.status);
	const canStartProcessing = isFullyPaid && order.status === OrderStatus.CONFIRMED;
	const canShip = order.status === OrderStatus.PROCESSING;
	const canMarkDelivered = order.status === OrderStatus.SHIPPED;
	const canCancel = [OrderStatus.PENDING, OrderStatus.CONFIRMED, OrderStatus.PROCESSING].includes(
		order.status
	);
	const canReturn = order.status === OrderStatus.DELIVERED;
	const canRefund = [OrderStatus.CANCELLED, OrderStatus.RETURNED].includes(order.status);

	const childrenArray = Children.toArray(children);

	const contextActions = childrenArray.find(
		(child) => isValidElement(child) && child.type === ContextActions
	) as ReactElement | undefined;
	const managementActions = childrenArray.find(
		(child) => isValidElement(child) && child.type === ManagementActions
	) as ReactElement | undefined;

	return (
		<Card className="w-full hover:shadow-md transition-shadow duration-200 pt-[unset] pb-[unset]">
			<CardContent className="pt-4 pb-4 pl-6 pr-6">
				{/* Main Info Section */}
				<div className="flex items-center justify-between gap-4 mb-4">
					{/* Order Basic Info */}
					<div className="flex items-center gap-4 flex-1 min-w-0">
						<div className="h-12 w-12 flex-shrink-0 bg-primary/10 rounded-lg flex items-center justify-center">
							<ShoppingCart className="h-6 w-6 text-primary" />
						</div>

						<div className="flex-1 min-w-0">
							<div className="flex items-center gap-2 mb-1">
								<h3 className="font-semibold text-foreground">Order #{order.id}</h3>
								<Badge className={statusInfo.color} variant="secondary">
									{statusInfo.label}
								</Badge>
							</div>

							<div className="text-sm text-muted-foreground mb-1">
								<User className="h-3 w-3 inline mr-1" />
								{order.customer.name} ({order.customer.email})
							</div>

							<div className="text-sm text-muted-foreground">
								<MapPin className="h-3 w-3 inline mr-1" />
								{order.shipAddress}
							</div>
						</div>
					</div>

					{/* Payment Info */}
					<div className="flex flex-col gap-1 min-w-0 flex-shrink-0 xl:mr-8">
						<div className="text-center">
							<div className="text-sm font-semibold text-foreground">
								{formatCurrency(order.paidAmount)} /{" "}
								{formatCurrency(order.totalAmount)}
							</div>
							<div className="text-xs text-muted-foreground">
								{isFullyPaid ? "✓ Paid" : `${formatCurrency(remainingAmount)} due`}
							</div>
						</div>
					</div>

					{/* Dates */}
					<div className="hidden lg:flex flex-col gap-1 text-xs text-muted-foreground flex-shrink-0">
						<div className="flex items-center gap-1">
							<Calendar className="h-3 w-3" />
							<span>Created: {formatFullDate(order.createdAt)}</span>
						</div>
						<div className="flex items-center gap-1">
							<RefreshCw className="h-3 w-3" />
							<span>Updated: {formatFullDate(order.updatedAt)}</span>
						</div>
					</div>

					{/* Management Actions */}
					{managementActions}
				</div>

				{/* Context Actions */}
				{contextActions}
				{/*<div className="flex items-center gap-2 flex-wrap border-t pt-3">*/}
				{/* Payment Processing */}
				{/*{canProcessPayment && (*/}
				{/*	<div className="flex items-center gap-2">*/}
				{/*		<Input*/}
				{/*			type="number"*/}
				{/*			step="0.01"*/}
				{/*			min="0"*/}
				{/*			max={remainingAmount}*/}
				{/*			placeholder={`Max: ${formatCurrency(remainingAmount)}`}*/}
				{/*			value={paymentAmount}*/}
				{/*			onChange={(e) => setPaymentAmount(e.target.value)}*/}
				{/*			className="w-32 h-8 text-sm"*/}
				{/*		/>*/}
				{/*		<Button*/}
				{/*			size="sm"*/}
				{/*			onClick={handleProcessPayment}*/}
				{/*			disabled={!paymentAmount || parseFloat(paymentAmount) <= 0}*/}
				{/*			className="h-8"*/}
				{/*		>*/}
				{/*			<CreditCard className="h-3 w-3 mr-1" />*/}
				{/*			Pay*/}
				{/*		</Button>*/}
				{/*	</div>*/}
				{/*)}*/}

				{/* Start Processing */}
				{/*{canStartProcessing && (*/}
				{/*	<Button*/}
				{/*		size="sm"*/}
				{/*		onClick={() => onStartProcessing(order.id)}*/}
				{/*		className="h-8"*/}
				{/*	>*/}
				{/*		<Play className="h-3 w-3 mr-1" />*/}
				{/*		Start Processing*/}
				{/*	</Button>*/}
				{/*)}*/}

				{/* Ship Order */}
				{/*{canShip && (*/}
				{/*	<div className="flex items-center gap-2">*/}
				{/*		{editingAddress ? (*/}
				{/*			<>*/}
				{/*				<Input*/}
				{/*					value={shippingAddress}*/}
				{/*					onChange={(e) => setShippingAddress(e.target.value)}*/}
				{/*					placeholder="Shipping address"*/}
				{/*					className="w-48 h-8 text-sm"*/}
				{/*					onKeyDown={(e) => {*/}
				{/*						if (e.key === "Enter") handleShipOrder();*/}
				{/*						if (e.key === "Escape") {*/}
				{/*							setEditingAddress(false);*/}
				{/*							setShippingAddress(order.shipAddress);*/}
				{/*						}*/}
				{/*					}}*/}
				{/*					autoFocus*/}
				{/*				/>*/}
				{/*				<Button size="sm" onClick={handleShipOrder} className="h-8">*/}
				{/*					<Truck className="h-3 w-3 mr-1" />*/}
				{/*					Ship*/}
				{/*				</Button>*/}
				{/*			</>*/}
				{/*		) : (*/}
				{/*			<Button size="sm" onClick={handleShipOrder} className="h-8">*/}
				{/*				<Truck className="h-3 w-3 mr-1" />*/}
				{/*				Ship Order*/}
				{/*			</Button>*/}
				{/*		)}*/}
				{/*	</div>*/}
				{/*)}*/}

				{/* Mark as Delivered */}
				{/*{canMarkDelivered && (*/}
				{/*	<Button*/}
				{/*		size="sm"*/}
				{/*		onClick={() => onMarkAsDelivered(order.id)}*/}
				{/*		className="h-8"*/}
				{/*	>*/}
				{/*		<CheckCircle className="h-3 w-3 mr-1" />*/}
				{/*		Mark Delivered*/}
				{/*	</Button>*/}
				{/*)}*/}

				{/* Cancel Order */}
				{/*{canCancel && (*/}
				{/*	<Button*/}
				{/*		size="sm"*/}
				{/*		variant="destructive"*/}
				{/*		onClick={() => onCancelOrder(order.id)}*/}
				{/*		className="h-8"*/}
				{/*	>*/}
				{/*		<XCircle className="h-3 w-3 mr-1" />*/}
				{/*		Cancel*/}
				{/*	</Button>*/}
				{/*)}*/}

				{/* Return Order */}
				{/*{canReturn && (*/}
				{/*	<div className="flex items-center gap-2">*/}
				{/*		{editingReturn ? (*/}
				{/*			<>*/}
				{/*				<Input*/}
				{/*					value={returnReason}*/}
				{/*					onChange={(e) => setReturnReason(e.target.value)}*/}
				{/*					placeholder="Return reason"*/}
				{/*					className="w-40 h-8 text-sm"*/}
				{/*					onKeyDown={(e) => {*/}
				{/*						if (e.key === "Enter") handleReturnOrder();*/}
				{/*						if (e.key === "Escape") {*/}
				{/*							setEditingReturn(false);*/}
				{/*							setReturnReason("");*/}
				{/*						}*/}
				{/*					}}*/}
				{/*					autoFocus*/}
				{/*				/>*/}
				{/*				<Button size="sm" onClick={handleReturnOrder} className="h-8">*/}
				{/*					<RotateCcw className="h-3 w-3 mr-1" />*/}
				{/*					Return*/}
				{/*				</Button>*/}
				{/*			</>*/}
				{/*		) : (*/}
				{/*			<Button size="sm" onClick={handleReturnOrder} className="h-8">*/}
				{/*				<RotateCcw className="h-3 w-3 mr-1" />*/}
				{/*				Return Order*/}
				{/*			</Button>*/}
				{/*		)}*/}
				{/*	</div>*/}
				{/*)}*/}

				{/* Refund Payment */}
				{/*{canRefund && (*/}
				{/*	<Button*/}
				{/*		size="sm"*/}
				{/*		variant="outline"*/}
				{/*		onClick={() => onRefundPayment(order.id)}*/}
				{/*		className="h-8"*/}
				{/*	>*/}
				{/*		<DollarSign className="h-3 w-3 mr-1" />*/}
				{/*		Refund*/}
				{/*	</Button>*/}
				{/*)}*/}
				{/*</div>*/}

				{/* Order Information */}
				{order.orderInformation && (
					<div className="mt-3 pt-3 border-t">
						<div className="text-sm text-muted-foreground">
							<span className="font-medium">Info:</span> {order.orderInformation}
						</div>
					</div>
				)}
			</CardContent>
		</Card>
	);
};

OrderRowCard.ManagementActions = ManagementActions;
OrderRowCard.ContextActions = ContextActions;
