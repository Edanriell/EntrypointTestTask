import { FC, useLayoutEffect, useState } from "react";
import { Reorder } from "motion/react";

import { EditOrder } from "@features/orders/edit";
import { DeleteOrder } from "@features/orders/delete";
import { ConfirmOrder } from "@features/orders/confirm";
import { MarkReadyForShipment } from "@features/orders/mark-ready-for-shipment";
import { ProcessPayment } from "@features/payments/process";
import { StartProcessing } from "@features/orders/start-processing";
import { Refund } from "@features/payments/refund";
import { MarkOutForDelivery } from "@features/orders/mark-out-for-delivery";
import { AddPayment } from "@features/payments/add";
import { Return } from "@features/orders/return";
import { Ship } from "@features/orders/ship";
import { Complete } from "@features/orders/complete";
import { CancelOrder } from "@features/orders/cancel";
import { MarkAsDelivered } from "@features/orders/mark-as-delivered";

import { OrderRowCard, type OrdersResponse, OrderStatus } from "@entities/orders";

type OrdersListProps = {
	orders: Array<OrdersResponse>;
};

export const OrdersList: FC<OrdersListProps> = ({ orders }) => {
	const [items, setItems] = useState(orders);

	useLayoutEffect(() => {
		setItems(orders);
	}, [orders]);

	return (
		<Reorder.Group axis="y" values={items} onReorder={setItems} className="space-y-4">
			{items.map((order) => (
				<Reorder.Item key={order.id} value={order}>
					<OrderRowCard order={order}>
						<OrderRowCard.ManagementActions>
							<EditOrder orderId={order.id} />
							<DeleteOrder orderId={order.id} orderNumber={order.orderNumber} />
						</OrderRowCard.ManagementActions>
						<OrderRowCard.ContextActions>
							<ConfirmOrder
								orderId={order.id}
								orderNumber={order.orderNumber}
								isFullyPaid={order.paidAmount >= order.totalAmount}
								isCancelled={order.status === "Cancelled"}
								isNotConfirmed={order.status !== OrderStatus.Confirmed}
							/>
							<MarkReadyForShipment
								orderId={order.id}
								orderNumber={order.orderNumber}
								orderStatus={order.status}
							/>
							<ProcessPayment orderId={order.id} orderNumber={order.orderNumber} />
							<StartProcessing
								orderId={order.id}
								orderNumber={order.orderNumber}
								isConfirmed={order.status === OrderStatus.Confirmed}
							/>
							<Refund
								orderId={order.id}
								orderNumber={order.orderNumber}
								orderStatus={order.status}
								paidAmount={order.paidAmount}
								currency={order.currency}
							/>
							<MarkOutForDelivery
								orderId={order.id}
								orderNumber={order.orderNumber}
								orderStatus={order.status}
							/>
							<AddPayment
								orderId={order.id}
								orderNumber={order.orderNumber}
								outstandingAmount={order.outstandingAmount}
								orderCurrency={order.currency}
								orderStatus={order.status}
								isFullyPaid={order.paidAmount >= order.totalAmount}
							/>
							<Return
								orderId={order.id}
								orderNumber={order.orderNumber}
								orderStatus={order.status}
							/>
							<Ship
								orderId={order.id}
								orderNumber={order.orderNumber}
								orderStatus={order.status}
							/>
							<Complete
								orderId={order.id}
								orderNumber={order.orderNumber}
								orderStatus={order.status}
							/>
							<CancelOrder
								orderId={order.id}
								orderNumber={order.orderNumber}
								orderStatus={order.status}
							/>
							<MarkAsDelivered
								orderId={order.id}
								orderNumber={order.orderNumber}
								orderStatus={order.status}
							/>
						</OrderRowCard.ContextActions>
					</OrderRowCard>
				</Reorder.Item>
			))}
		</Reorder.Group>
	);
};
