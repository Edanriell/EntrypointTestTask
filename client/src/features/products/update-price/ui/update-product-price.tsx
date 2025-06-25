import { FC, useState } from "react";

import { Input } from "@shared/ui/input";
import { formatCurrency } from "@shared/lib/functions";

export const UpdateProductPrice: FC = () => {
	const [editingPrice, setEditingPrice] = useState(false);
	// const [tempPrice, setTempPrice] = useState(product.unitPrice.toString());
	const [tempPrice, setTempPrice] = useState(0);

	const handlePriceSubmit = () => {
		// business logic here
		// Save the price and exit editing mode
		setEditingPrice(false); // Changed from true to false
	};

	return (
		<div className="flex flex-col gap-1 min-w-0 flex-shrink-0 ">
			<div className="text-xs text-muted-foreground text-center">Price</div>
			{editingPrice ? (
				<Input
					type="number"
					min="0"
					value="0"
					onChange={(e) => setTempPrice(+e.target.value)}
					onBlur={handlePriceSubmit}
					onKeyDown={(e) => {
						if (e.key === "Enter") handlePriceSubmit();
						if (e.key === "Escape") {
							// setTempPrice(product.unitPrice.toString());
							setTempPrice(0);
							setEditingPrice(false);
						}
					}}
					className="w-20 h-8 text-sm text-center no-arrows"
					autoFocus
				/>
			) : (
				<div
					className="flex items-center gap-1 cursor-pointer hover:bg-muted/50 px-2 py-1 rounded text-sm font-medium"
					onClick={() => setEditingPrice(true)}
				>
					{/*<span>{formatCurrency(product.unitPrice)}</span>*/}
					<span>{formatCurrency(0)}</span>
				</div>
			)}
		</div>
	);
};
