import { FC, useState } from "react";
import { AlertTriangle } from "lucide-react";

import { Input } from "@shared/ui/input";

export const UpdateProductReservedStock: FC = () => {
	const [editingReservedStock, setEditingReservedStock] = useState(false);

	// const [tempReservedStock, setTempReservedStock] = useState(product.unitsOnOrder.toString());
	const [tempReservedStock, setTempReservedStock] = useState(0);

	const handleReservedStockSubmit = () => {
		// business logic here
		// const newReservedStock = parseInt(tempReservedStock);
		// if (!isNaN(newReservedStock) && newReservedStock >= 0) {
		// 	onUpdateReservedStock(product.id, newReservedStock);
		// } else {
		// 	setTempReservedStock(product.unitsOnOrder.toString());
		// }
		// setEditingReservedStock(false);
		setEditingReservedStock(false);
	};

	return (
		<div className="flex flex-col gap-1 min-w-0 flex-shrink-0 xl:mr-8">
			<div className="text-xs text-muted-foreground text-center">Reserved</div>
			{editingReservedStock ? (
				<Input
					type="number"
					min="0"
					value={tempReservedStock}
					onChange={(e) => setTempReservedStock(+e.target.value)}
					onBlur={handleReservedStockSubmit}
					onKeyDown={(e) => {
						if (e.key === "Enter") handleReservedStockSubmit();
						if (e.key === "Escape") {
							// setTempReservedStock(product.unitsOnOrder.toString());
							setTempReservedStock(0);
							setEditingReservedStock(false);
						}
					}}
					className="w-16 h-8 text-sm text-center no-arrows"
					autoFocus
				/>
			) : (
				<div
					className="flex items-center gap-1 cursor-pointer hover:bg-muted/50 px-2 py-1 rounded text-sm font-medium"
					onClick={() => setEditingReservedStock(true)}
				>
					<AlertTriangle className="h-3 w-3" />
					{/*<span>{product.unitsOnOrder}</span>*/}
					<span>{0}</span>
				</div>
			)}
		</div>
	);
};
