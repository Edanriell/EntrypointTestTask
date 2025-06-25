"use client";

import { FC } from "react";

import { OrderRowCard, OrderStatus } from "@entities/orders";
import { CreateOrder } from "@features/orders/create";
import { EditOrder } from "@features/orders/edit";
import { DeleteOrder } from "@features/orders/delete";
import { ProcessPaymentWithAutomaticOrderConfirmation } from "@features/payments/process";
import { StartProcessingOrder } from "@features/orders/start-processing";
import { ShipOrder } from "@features/orders/ship";
import { ReturnOrder } from "@features/orders/return";
import { CancelOrder } from "@features/orders/cancel";
import { RefundPaymentWithOrderUpdate } from "@features/payments/refund";

const mockOrders = [
	{
		id: 1001,
		createdAt: "2024-06-20T10:30:00Z",
		updatedAt: "2024-06-21T14:15:00Z",
		status: OrderStatus.PENDING,
		shipAddress: "123 Main St, New York, NY 10001",
		orderInformation: "Customer requested express delivery",
		totalAmount: 1299.99,
		paidAmount: 0,
		customer: {
			id: 1,
			name: "John Doe",
			email: "john.doe@example.com"
		},
		products: [{ id: 1, productName: "Dell XPS 13 Laptop", unitPrice: 1299.99, quantity: 1 }]
	},
	{
		id: 1002,
		createdAt: "2024-06-19T09:15:00Z",
		updatedAt: "2024-06-21T16:30:00Z",
		status: OrderStatus.CONFIRMED,
		shipAddress: "456 Oak Ave, Los Angeles, CA 90210",
		orderInformation: "Gift wrap requested",
		totalAmount: 549.98,
		paidAmount: 549.98,
		customer: {
			id: 2,
			name: "Jane Smith",
			email: "jane.smith@example.com"
		},
		products: [
			{ id: 2, productName: "Logitech MX Master 3S", unitPrice: 99.99, quantity: 2 },
			{ id: 3, productName: 'Samsung 27" 4K Monitor', unitPrice: 399.99, quantity: 1 }
		]
	},
	{
		id: 1003,
		createdAt: "2024-06-18T14:20:00Z",
		updatedAt: "2024-06-21T11:45:00Z",
		status: OrderStatus.PROCESSING,
		shipAddress: "789 Pine St, Chicago, IL 60601",
		orderInformation: "Bulk order for office setup",
		totalAmount: 2049.95,
		paidAmount: 2049.95,
		customer: {
			id: 3,
			name: "Mike Johnson",
			email: "mike.johnson@techcorp.com"
		},
		products: [
			{ id: 4, productName: "Mechanical Gaming Keyboard", unitPrice: 149.99, quantity: 5 },
			{ id: 5, productName: "Logitech C920 HD Webcam", unitPrice: 79.99, quantity: 8 },
			{ id: 6, productName: "Sony WH-1000XM5 Headphones", unitPrice: 349.99, quantity: 3 }
		]
	},
	{
		id: 1004,
		createdAt: "2024-06-17T08:45:00Z",
		updatedAt: "2024-06-20T13:20:00Z",
		status: OrderStatus.SHIPPED,
		shipAddress: "321 Elm Dr, Miami, FL 33101",
		orderInformation: "Standard shipping",
		totalAmount: 1199.98,
		paidAmount: 1199.98,
		customer: {
			id: 4,
			name: "Sarah Wilson",
			email: "sarah.wilson@example.com"
		},
		products: [{ id: 7, productName: "iPad Air 10.9-inch", unitPrice: 599.99, quantity: 2 }]
	},
	{
		id: 1005,
		createdAt: "2024-06-16T16:10:00Z",
		updatedAt: "2024-06-19T10:30:00Z",
		status: OrderStatus.DELIVERED,
		shipAddress: "654 Maple Rd, Seattle, WA 98101",
		orderInformation: "Delivered to front door",
		totalAmount: 999.99,
		paidAmount: 999.99,
		customer: {
			id: 5,
			name: "David Brown",
			email: "david.brown@example.com"
		},
		products: [{ id: 8, productName: "iPhone 15 Pro", unitPrice: 999.99, quantity: 1 }]
	},
	{
		id: 1006,
		createdAt: "2024-06-15T12:30:00Z",
		updatedAt: "2024-06-21T09:15:00Z",
		status: OrderStatus.CANCELLED,
		shipAddress: "987 Cedar St, Boston, MA 02101",
		orderInformation: "Customer requested cancellation",
		totalAmount: 749.98,
		paidAmount: 749.98,
		customer: {
			id: 6,
			name: "Lisa Garcia",
			email: "lisa.garcia@example.com"
		},
		products: [
			{ id: 2, productName: "Logitech MX Master 3S", unitPrice: 99.99, quantity: 1 },
			{ id: 6, productName: "Sony WH-1000XM5 Headphones", unitPrice: 349.99, quantity: 1 },
			{ id: 1, productName: "Dell XPS 13 Laptop", unitPrice: 299.99, quantity: 1 }
		]
	}
];

export const OrdersPage: FC = () => {
	const handleEditOrder = (orderId: number) => {
		console.log("Edit order:", orderId);
	};

	const handleProcessPayment = (orderId: number, paymentAmount: number) => {
		console.log("Process payment for order:", orderId, "Amount:", paymentAmount);
		// Implement ProcessPaymentWithAutomaticOrderConfirmation command
	};

	const handleStartProcessing = (orderId: number) => {
		console.log("Start processing order:", orderId);
		// Implement StartProcessingOrder command
	};

	const handleShipOrder = (orderId: number, address: string) => {
		console.log("Ship order:", orderId, "Address:", address);
		// Implement ShipOrder command
	};

	const handleCancelOrder = (orderId: number) => {
		console.log("Cancel order:", orderId);
		// Implement CancelOrder command
	};

	const handleMarkAsDelivered = (orderId: number) => {
		console.log("Mark order as delivered:", orderId);
		// Implement MarkOrderAsDelivered command
	};

	const handleReturnOrder = (orderId: number, reason: string) => {
		console.log("Return order:", orderId, "Reason:", reason);
		// Implement ReturnOrder command
	};

	const handleRefundPayment = (orderId: number) => {
		console.log("Refund payment for order:", orderId);
		// Implement RefundPaymentWithOrderUpdate command
	};

	return (
		<>
			<div className="flex flex-1 flex-col gap-4 p-4">
				<CreateOrder />
				{mockOrders.map((order) => (
					<OrderRowCard
						key={order.id}
						order={order}
						onEdit={handleEditOrder}
						onProcessPayment={handleProcessPayment}
						onStartProcessing={handleStartProcessing}
						onShipOrder={handleShipOrder}
						onCancelOrder={handleCancelOrder}
						onMarkAsDelivered={handleMarkAsDelivered}
						onReturnOrder={handleReturnOrder}
						onRefundPayment={handleRefundPayment}
					>
						<OrderRowCard.ManagementActions>
							<EditOrder />
							<DeleteOrder />
						</OrderRowCard.ManagementActions>
						<OrderRowCard.ContextActions>
							<ProcessPaymentWithAutomaticOrderConfirmation order={order} />
							<StartProcessingOrder order={order} />
							<ShipOrder order={order} />
							<ReturnOrder order={order} />
							<CancelOrder order={order} />
							<RefundPaymentWithOrderUpdate order={order} />
						</OrderRowCard.ContextActions>
					</OrderRowCard>
				))}
			</div>
		</>
	);
};
