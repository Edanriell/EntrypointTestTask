import { FC } from "react";
import { AlertTriangle } from "lucide-react";

type ProductReservedStock = {
	productId: string;
};

export const ProductReservedStock: FC<ProductReservedStock> = () => {
	return (
		<div className="flex flex-col gap-1 min-w-0 flex-shrink-0 xl:mr-8">
			<div className="text-xs text-muted-foreground text-center">Reserved</div>
			<div className="flex items-center gap-1 cursor-pointer hover:bg-muted/50 px-2 py-1 rounded text-sm font-medium">
				<AlertTriangle className="h-3 w-3" />
				{/*<span>{product.unitsOnOrder}</span>*/}
				<span>{0}</span>
			</div>
		</div>
	);
};
