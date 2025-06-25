import { FC, useState } from "react";
import { Archive } from "lucide-react";

import { Input } from "@shared/ui/input";

export const UpdateProductStock: FC = () => {
	const [editingStock, setEditingStock] = useState(false);
	// const [tempStock, setTempStock] = useState(product.unitsInStock.toString());
	const [tempStock, setTempStock] = useState(0);

	const handleStockSubmit = () => {
		// business logic here
		// const newStock = parseInt(tempStock);
		// if (!isNaN(newStock) && newStock >= 0) {
		// 	onUpdateStock(product.id, newStock);
		// } else {
		// 	setTempStock(product.unitsInStock.toString());
		// }
		setEditingStock(false);
	};

	return (
		<div className="flex flex-col gap-1 min-w-0 flex-shrink-0">
			<div className="text-xs text-muted-foreground text-center">Stock</div>
			{editingStock ? (
				<Input
					type="number"
					min="0"
					// value={tempStock}
					value={0}
					// onChange={(e) => setTempStock(e.target.value)}
					onChange={(e) => setTempStock(+e.target.value)}
					onBlur={handleStockSubmit}
					onKeyDown={(e) => {
						if (e.key === "Enter") handleStockSubmit();
						if (e.key === "Escape") {
							// setTempStock(product.unitsInStock.toString());
							setTempStock(0);
							setEditingStock(false);
						}
					}}
					className="w-16 h-8 text-sm text-center no-arrows"
					autoFocus
				/>
			) : (
				<div
					className="flex items-center gap-1 cursor-pointer hover:bg-muted/50 px-2 py-1 rounded text-sm font-medium"
					onClick={() => setEditingStock(true)}
				>
					<Archive className="h-3 w-3" />
					{/*<span>{product.unitsInStock}</span>*/}
					<span>{0}</span>
				</div>
			)}
		</div>
	);
};
