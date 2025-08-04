import { FC } from "react";
import { AlertTriangle } from "lucide-react";

import { useGetProductById } from "../api";

type ProductReservedStock = {
	productId: string;
};

export const ProductReservedStock: FC<ProductReservedStock> = ({ productId }) => {
	const { data: productData, isLoading: isLoadingProduct } = useGetProductById(productId);

	return (
		<div className="flex flex-col gap-1 min-w-0 flex-shrink-0 xl:mr-8">
			<div className="text-xs text-muted-foreground text-center">Reserved</div>
			<div className="flex items-center gap-1 px-2 py-1 rounded text-sm font-medium">
				<AlertTriangle className="h-3 w-3" />
				<span>{productData?.reserved}</span>
			</div>
		</div>
	);
};
