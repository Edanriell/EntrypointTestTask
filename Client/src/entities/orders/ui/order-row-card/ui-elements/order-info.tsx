import { FC } from "react";
import {
	CalendarDays,
	CheckCircle,
	MapPin,
	PackageOpen,
	Phone,
	ShoppingCart,
	Truck,
	User,
	XCircle
} from "lucide-react";

import { Avatar, AvatarFallback } from "@shared/ui/avatar";
import { Badge } from "@shared/ui/badge";
import { cn } from "@shared/lib/utils";

import { getStatusInfo } from "../../../lib";
import type { OrdersResponse } from "../../../api";

type OrderInfoProps = {
	order: OrdersResponse;
};

export const OrderInfo: FC<OrderInfoProps> = ({ order }) => {
	const payments = order.payments ?? [];
	const isFullyPaid = order.paidAmount >= order.totalAmount;

	const statusInfo = getStatusInfo(order.status);

	return (
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
					<div className="flex items-center space-x-2 text-sm text-muted-foreground">
						<User className="h-4 w-4" />
						<span className="font-medium text-foreground">
							{order.client?.clientFirstName} {order.client?.clientLastName}
						</span>
						<span className="text-muted-foreground">•</span>
						<span>{order.client?.clientEmail}</span>
					</div>
					{order.client?.clientPhoneNumber && (
						<div className="flex items-center space-x-2 text-sm text-muted-foreground">
							<Phone className="h-4 w-4" />
							<span>{order.client.clientPhoneNumber}</span>
						</div>
					)}
					<div className="flex items-start space-x-2 text-sm text-muted-foreground">
						<MapPin className="h-4 w-4 mt-0.5 flex-shrink-0" />
						<span className="line-clamp-2">{order.shippingAddress}</span>
					</div>
					<div className="space-y-1 text-sm">
						<div className="flex items-center space-x-2 text-indigo-600">
							<CalendarDays className="h-4 w-4" />
							<span>Created: {new Date(order.createdAt).toLocaleString()}</span>
						</div>
						{order.confirmedAt && (
							<div className="flex items-center space-x-2 text-emerald-600">
								<CheckCircle className="h-4 w-4" />
								<span>
									Confirmed: {new Date(order.confirmedAt).toLocaleString()}
								</span>
							</div>
						)}
						{order.shippedAt && (
							<div className="flex items-center space-x-2 text-cyan-600">
								<Truck className="h-4 w-4" />
								<span>Shipped: {new Date(order.shippedAt).toLocaleString()}</span>
							</div>
						)}
						{order.deliveredAt && (
							<div className="flex items-center space-x-2 text-amber-600">
								<PackageOpen className="h-4 w-4" />
								<span>
									Delivered: {new Date(order.deliveredAt).toLocaleString()}
								</span>
							</div>
						)}
						{order.cancelledAt && (
							<div className="flex items-center space-x-2 text-rose-600">
								<XCircle className="h-4 w-4" />
								<span>
									Cancelled: {new Date(order.cancelledAt).toLocaleString()}
								</span>
							</div>
						)}
					</div>
				</div>
			</div>
		</div>
	);
};
