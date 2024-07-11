import { FC } from "react";

import { calculateOrderTotal } from "@entities/orders/lib";
import { Order } from "@entities/orders/model";

import { TableCell, TableRow } from "@shared/ui/table";
import { formatDateString } from "@shared/lib";

type OrderRowMinimalProps = {
	order: Order;
};

export const OrderRowMinimal: FC<OrderRowMinimalProps> = ({ order }) => {
	return (
		<TableRow>
			<TableCell>
				<div className="font-medium">
					{order.customer.name} {order.customer.surname}
				</div>
				<div className="hidden text-sm text-muted-foreground md:inline">{order.customer.email}</div>
			</TableCell>
			<TableCell>{formatDateString(order.createdAt)}</TableCell>
			<TableCell className="text-right">${calculateOrderTotal(order)}</TableCell>
		</TableRow>
	);
};
