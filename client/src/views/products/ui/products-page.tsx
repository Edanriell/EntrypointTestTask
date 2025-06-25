"use client";

import { FC } from "react";

import { ProductRowCard } from "@entities/products";
import { CreateProduct } from "@features/products/create";
import { EditProduct } from "@features/products/edit";
import { DeleteProduct } from "@features/products/delete";
import { UpdateProductPrice } from "@features/products/update-price";
import { UpdateProductStock } from "@features/products/update-stock";
import { UpdateProductReservedStock } from "@features/products/update-reserved-stock";

const mockProducts = [
	{
		id: 1,
		code: "LAPTOP-001",
		productName: "Dell XPS 13 Laptop",
		description: "13-inch ultrabook with Intel i7 processor, 16GB RAM, 512GB SSD",
		unitPrice: 1299.99,
		unitsInStock: 25,
		unitsOnOrder: 5,
		createdAt: "2024-01-10"
	},
	{
		id: 2,
		code: "MOUSE-002",
		productName: "Logitech MX Master 3S",
		description: "Advanced wireless mouse with precision tracking and ergonomic design",
		unitPrice: 99.99,
		unitsInStock: 150,
		unitsOnOrder: 20,
		createdAt: "2024-01-15"
	},
	{
		id: 3,
		code: "MONITOR-003",
		productName: 'Samsung 27" 4K Monitor',
		description: "27-inch 4K UHD monitor with HDR support and USB-C connectivity",
		unitPrice: 399.99,
		unitsInStock: 8,
		unitsOnOrder: 12,
		createdAt: "2024-02-01"
	},
	{
		id: 4,
		code: "KEYBOARD-004",
		productName: "Mechanical Gaming Keyboard",
		description: "RGB backlit mechanical keyboard with Cherry MX switches",
		unitPrice: 149.99,
		unitsInStock: 45,
		unitsOnOrder: 8,
		createdAt: "2024-01-20"
	},
	{
		id: 5,
		code: "WEBCAM-005",
		productName: "Logitech C920 HD Webcam",
		description: "1080p HD webcam with auto-focus and built-in microphone",
		unitPrice: 79.99,
		unitsInStock: 0,
		unitsOnOrder: 15,
		createdAt: "2024-02-10"
	},
	{
		id: 6,
		code: "HEADSET-006",
		productName: "Sony WH-1000XM5 Headphones",
		description: "Wireless noise-canceling headphones with 30-hour battery life",
		unitPrice: 349.99,
		unitsInStock: 12,
		unitsOnOrder: 2,
		createdAt: "2024-01-25"
	},
	{
		id: 7,
		code: "TABLET-007",
		productName: "iPad Air 10.9-inch",
		description: "10.9-inch iPad Air with M1 chip, 64GB storage, Wi-Fi",
		unitPrice: 599.99,
		unitsInStock: 30,
		unitsOnOrder: 25,
		createdAt: "2024-02-05"
	},
	{
		id: 8,
		code: "PHONE-008",
		productName: "iPhone 15 Pro",
		description: "6.1-inch iPhone 15 Pro with 128GB storage and titanium design",
		unitPrice: 999.99,
		unitsInStock: 18,
		unitsOnOrder: 22,
		createdAt: "2024-02-12"
	}
];

export const ProductsPage: FC = () => {
	const handleEditProduct = (productId: number) => {
		console.log("Edit product:", productId);
		// Implement your edit-user logic here
		// e.g., navigate to edit-user page or open modal
	};

	const handleUpdatePrice = (productId: number, newPrice: number) => {
		console.log("Update product price:", productId, "New price:", newPrice);
		// Implement UpdateProductPrice command here
		// e.g., call API to update price
	};

	const handleUpdateStock = (productId: number, newStock: number) => {
		console.log("Update product stock:", productId, "New stock:", newStock);
		// Implement UpdateProductStock command here
		// e.g., call API to update stock
	};

	const handleUpdateReservedStock = (productId: number, newReservedStock: number) => {
		console.log(
			"Update product reserved stock:",
			productId,
			"New reserved stock:",
			newReservedStock
		);
		// Implement UpdateProductReservedStock command here
		// e.g., call API to update reserved stock
	};

	return (
		<>
			<div className="flex flex-1 flex-col gap-4 p-4">
				<CreateProduct />
				{mockProducts.map((product) => (
					<ProductRowCard
						key={product.id}
						product={product}
						onEdit={handleEditProduct}
						onUpdatePrice={handleUpdatePrice}
						onUpdateStock={handleUpdateStock}
						onUpdateReservedStock={handleUpdateReservedStock}
					>
						<ProductRowCard.ManagementActions>
							<EditProduct />
							<DeleteProduct />
						</ProductRowCard.ManagementActions>
						<ProductRowCard.QuickActions>
							<UpdateProductPrice />
							<UpdateProductStock />
							<UpdateProductReservedStock />
						</ProductRowCard.QuickActions>
					</ProductRowCard>
				))}
			</div>
		</>
	);
};
