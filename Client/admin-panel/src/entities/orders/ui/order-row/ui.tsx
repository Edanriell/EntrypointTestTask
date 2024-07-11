import { FC } from "react";

import { TableCell, TableRow } from "@shared/ui/table";
import { Badge } from "@shared/ui/badge";
import { formatDateString } from "@shared/lib";

import { calculateTotalPrice } from "./lib";

type OrderRowProps = {
	order: any;
	onOrderClick: (order: any) => void;
};

export const OrderRow: FC<OrderRowProps> = ({ onOrderClick, order }) => {
	console.log(order);

	return (
		<TableRow onClick={onOrderClick}>
			<TableCell>
				<div className="font-medium">
					{order.customer?.name} {order.customer?.surname}
				</div>
				<div className="hidden text-sm text-muted-foreground md:inline">
					{order.customer?.email}
				</div>
			</TableCell>
			<TableCell className="hidden sm:table-cell">
				<Badge className="text-xs" variant="secondary">
					{order?.status}
				</Badge>
			</TableCell>
			<TableCell className="hidden sm:table-cell">{formatDateString(order?.createdAt)}</TableCell>
			<TableCell className="hidden md:table-cell">{formatDateString(order?.updatedAt)}</TableCell>
			<TableCell className="hidden md:table-cell max-w-[160px]">{order?.shipAddress}</TableCell>
			<TableCell className="hidden md:table-cell max-w-[160px]">
				{order?.orderInformation}
			</TableCell>
			<TableCell className="text-right">${calculateTotalPrice(order?.products)}</TableCell>
		</TableRow>
	);
};
