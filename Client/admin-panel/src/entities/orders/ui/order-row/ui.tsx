import { FC } from "react";

import type { Order } from "@entities/orders/model";

import { TableCell, TableRow } from "@shared/ui/table";
import { Badge } from "@shared/ui/badge";
import { formatDateString } from "@shared/lib";
// import { useRenderInfo } from "@shared/lib/hooks";
import { calculateTotalPrice, displayOrderStatus } from "./lib";

type OrderRowProps = {
	data: Order;
	onRowClick: (order: Order) => void;
	lastRowRef?: (node?: Element | null | undefined) => void;
	classes?: string;
};

export const OrderRow: FC<OrderRowProps> = ({ onRowClick, data, lastRowRef, classes }) => {
	// const renderTime = useRenderInfo("OrderRow");

	return (
		<TableRow className={classes} ref={lastRowRef} onClick={() => onRowClick(data)}>
			<TableCell className={"table-cell"}>
				<div className="font-medium">
					{data.customer?.name} {data.customer?.surname}
				</div>
				<div className="hidden text-sm text-muted-foreground md:inline">{data.customer?.email}</div>
			</TableCell>
			<TableCell className="hidden min-[520px]:table-cell">
				<Badge className="text-xs" variant="secondary">
					{displayOrderStatus(data?.status)}
				</Badge>
			</TableCell>
			<TableCell className="hidden min-[920px]:table-cell">
				{formatDateString(data?.createdAt)}
			</TableCell>
			<TableCell className="hidden min-[1040px]:table-cell">
				{formatDateString(data?.updatedAt)}
			</TableCell>
			<TableCell className="hidden min-[1200px]:table-cell min-[1200px]:max-w-[180px]">
				{data?.shipAddress}
			</TableCell>
			<TableCell className="hidden min-[1320px]:table-cell min-[1200px]:max-w-[180px]">
				{data?.orderInformation}
			</TableCell>
			<TableCell className="text-right table-cell">
				${calculateTotalPrice(data?.products)}
			</TableCell>
		</TableRow>
	);
};
