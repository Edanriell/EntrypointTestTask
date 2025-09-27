import { FC, useLayoutEffect, useState } from "react";
import { Reorder } from "motion/react";

import { EditProduct } from "@features/products/edit";
import { DeleteProduct } from "@features/products/delete";
import { UpdateProductPrice } from "@features/products/update-price";
import { UpdateProductStock } from "@features/products/update-stock";
import { ProductReservedStock } from "@features/products/reserved-stock";
import { DiscountProduct } from "@features/products/discount-product/ui";
import { RestoreProduct } from "@features/products/restore-product";

import { Product, ProductRowCard, ProductStatus } from "@entities/products";

type ProductsListProps = {
	products: Array<Product>;
};

export const ProductsList: FC<ProductsListProps> = ({ products }) => {
	const [items, setItems] = useState(products);

	useLayoutEffect(() => {
		setItems(products);
	}, [products]);

	return (
		<Reorder.Group axis="y" values={items} onReorder={setItems} className="space-y-4">
			{items.map((product) => (
				<Reorder.Item key={product.id} value={product}>
					<ProductRowCard product={product}>
						{product.status !== ProductStatus.Deleted && (
							<ProductRowCard.ManagementActions>
								<EditProduct productId={product.id} />
								<DeleteProduct productId={product.id} productName={product.name} />
							</ProductRowCard.ManagementActions>
						)}
						<ProductRowCard.QuickActions>
							{product.status !== ProductStatus.Deleted && (
								<>
									<UpdateProductPrice productId={product.id} />
									<UpdateProductStock productId={product.id} />
									<ProductReservedStock productId={product.id} />
									<DiscountProduct productId={product.id} />
								</>
							)}
							{product.status === ProductStatus.Deleted && (
								<RestoreProduct productId={product.id} />
							)}
						</ProductRowCard.QuickActions>
					</ProductRowCard>
				</Reorder.Item>
			))}
		</Reorder.Group>
	);
};
