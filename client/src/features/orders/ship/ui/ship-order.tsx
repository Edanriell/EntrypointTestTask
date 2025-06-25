import { FC, useState } from "react";
import { Input } from "@shared/ui/input";
import { Button } from "@shared/ui/button";
import { Truck } from "lucide-react";
import { OrderStatus } from "@entities/orders";

type ShipOrderProps = {
	order: any;
};

export const ShipOrder: FC<ShipOrderProps> = ({ order }) => {
	const [editingAddress, setEditingAddress] = useState(false);
	const [shippingAddress, setShippingAddress] = useState(order.shipAddress);

	const canShip = order.status === OrderStatus.PROCESSING;

	const handleShipOrder = () => {
		// if (editingAddress && shippingAddress.trim()) {
		// 	onShipOrder(order.id, shippingAddress.trim());
		// 	setEditingAddress(false);
		// } else {
		// 	setEditingAddress(true);
		// }
	};

	return (
		<>
			{canShip && (
				<div className="flex items-center gap-2">
					{editingAddress ? (
						<>
							<Input
								value={shippingAddress}
								onChange={(e) => setShippingAddress(e.target.value)}
								placeholder="Shipping address"
								className="w-48 h-8 text-sm"
								onKeyDown={(e) => {
									if (e.key === "Enter") handleShipOrder();
									if (e.key === "Escape") {
										setEditingAddress(false);
										setShippingAddress(order.shipAddress);
									}
								}}
								autoFocus
							/>
							<Button size="sm" onClick={handleShipOrder} className="h-8">
								<Truck className="h-3 w-3 mr-1" />
								Ship
							</Button>
						</>
					) : (
						<Button size="sm" onClick={handleShipOrder} className="h-8">
							<Truck className="h-3 w-3 mr-1" />
							Ship Order
						</Button>
					)}
				</div>
			)}
		</>
	);
};
