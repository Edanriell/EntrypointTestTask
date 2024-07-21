import { FC } from "react";

import type { Order } from "@entities/orders/model";

import { TableCell, TableRow } from "@shared/ui/table";
import { Badge } from "@shared/ui/badge";
import { formatDateString } from "@shared/lib";
// import { useRenderInfo } from "@shared/lib/hooks";
import { calculateTotalPrice, displayOrderStatus } from "./lib";

type OrderRowProps = {
	order: Order;
	onOrderClick: (order: Order) => void;
	lastOrderRowRef?: (node?: Element | null | undefined) => void;
};

export const OrderRow: FC<OrderRowProps> = ({ onOrderClick, order, lastOrderRowRef }) => {
	// const renderTime = useRenderInfo("OrderRow");

	return (
		<TableRow className="cursor-pointer" ref={lastOrderRowRef} onClick={() => onOrderClick(order)}>
			<TableCell className={"block"}>
				<div className="font-medium">
					{order.customer?.name} {order.customer?.surname}
				</div>
				<div className="hidden text-sm text-muted-foreground md:inline">
					{order.customer?.email}
				</div>
			</TableCell>
			<TableCell className="hidden min-[520px]:table-cell">
				<Badge className="text-xs" variant="secondary">
					{displayOrderStatus(order?.status)}
				</Badge>
			</TableCell>
			<TableCell className="hidden min-[920px]:table-cell">
				{formatDateString(order?.createdAt)}
			</TableCell>
			<TableCell className="hidden min-[1040px]:table-cell">
				{formatDateString(order?.updatedAt)}
			</TableCell>
			<TableCell className="hidden min-[1200px]:table-cell min-[1200px]:max-w-[180px]">
				{order?.shipAddress}
			</TableCell>
			<TableCell className="hidden min-[1320px]:table-cell min-[1200px]:max-w-[180px]">
				{order?.orderInformation}
			</TableCell>
			<TableCell className="text-right">${calculateTotalPrice(order?.products)}</TableCell>
		</TableRow>
	);
};
