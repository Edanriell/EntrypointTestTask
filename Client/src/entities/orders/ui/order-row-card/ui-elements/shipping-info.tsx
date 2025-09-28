import { FC } from "react";
import { Clock, Package, Truck } from "lucide-react";

import { Card, CardContent } from "@shared/ui/card";
import { Badge } from "@shared/ui/badge";
import { formatDate } from "@shared/lib/utils";

import { OrdersResponse } from "../../../api";

type ShippingInfoProps = {
	order: OrdersResponse;
};

export const ShippingInfo: FC<ShippingInfoProps> = ({ order }) => {
	const { trackingNumber, courier, estimatedDeliveryDate } = order;

	if (!trackingNumber && !courier && !estimatedDeliveryDate) return null;

	return (
		<Card className="bg-muted/30 border border-border/50 shadow-sm">
			<CardContent className="py-3 px-4">
				<div className="flex flex-wrap items-center gap-x-6 gap-y-2 text-sm">
					{trackingNumber && (
						<div className="flex items-center gap-1.5">
							<Truck className="h-4 w-4 text-primary/80" />
							<span className="text-muted-foreground">Tracking:</span>
							<Badge
								variant="outline"
								className="font-mono text-xs bg-primary/5 text-primary border-primary/20"
							>
								{trackingNumber}
							</Badge>
						</div>
					)}
					{courier && (
						<div className="flex items-center gap-1.5">
							<Package className="h-4 w-4 text-blue-500" />
							<span className="text-muted-foreground">Courier:</span>
							<span className="font-medium">{courier}</span>
						</div>
					)}
					{estimatedDeliveryDate && (
						<div className="flex items-center gap-1.5">
							<Clock className="h-4 w-4 text-amber-500" />
							<span className="text-muted-foreground">Est. Delivery:</span>
							<span className="font-medium">{formatDate(estimatedDeliveryDate)}</span>
						</div>
					)}
				</div>
			</CardContent>
		</Card>
	);
};
