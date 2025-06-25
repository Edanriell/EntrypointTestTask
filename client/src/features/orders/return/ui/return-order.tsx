import { FC, useState } from "react";
import { Input } from "@shared/ui/input";
import { Button } from "@shared/ui/button";
import { RotateCcw } from "lucide-react";
import { OrderStatus } from "@entities/orders";

type ReturnOrderProps = {
	order: any;
};

export const ReturnOrder: FC<ReturnOrderProps> = ({ order }) => {
	const [editingReturn, setEditingReturn] = useState(false);
	const [returnReason, setReturnReason] = useState("");

	const canReturn = order.status === OrderStatus.DELIVERED;

	const handleReturnOrder = () => {
		// if (editingReturn && returnReason.trim()) {
		// 	onReturnOrder(order.id, returnReason.trim());
		// 	setEditingReturn(false);
		// 	setReturnReason("");
		// } else {
		// 	setEditingReturn(true);
		// }
	};

	return (
		<>
			{canReturn && (
				<div className="flex items-center gap-2">
					{editingReturn ? (
						<>
							<Input
								value={returnReason}
								onChange={(e) => setReturnReason(e.target.value)}
								placeholder="Return reason"
								className="w-40 h-8 text-sm"
								onKeyDown={(e) => {
									if (e.key === "Enter") handleReturnOrder();
									if (e.key === "Escape") {
										setEditingReturn(false);
										setReturnReason("");
									}
								}}
								autoFocus
							/>
							<Button size="sm" onClick={handleReturnOrder} className="h-8">
								<RotateCcw className="h-3 w-3 mr-1" />
								Return
							</Button>
						</>
					) : (
						<Button size="sm" onClick={handleReturnOrder} className="h-8">
							<RotateCcw className="h-3 w-3 mr-1" />
							Return Order
						</Button>
					)}
				</div>
			)}
		</>
	);
};
