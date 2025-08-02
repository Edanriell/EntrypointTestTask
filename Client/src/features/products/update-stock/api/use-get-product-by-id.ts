import { useQuery } from "@tanstack/react-query";

import { productsQueries } from "@entities/products";

export const useGetProductById = (productId: string) => {
	return useQuery(productsQueries.productDetail({ productId }));
};
