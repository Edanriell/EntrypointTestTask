import { apiClient } from "@shared/api";

import { GetProductByIdResponse } from "./get-product-by-id-response";
import { GetProductByIdQuery } from "./get-product-by-id-query";

export const getProductById = async ({
	productId
}: GetProductByIdQuery): Promise<GetProductByIdResponse> => {
	return apiClient.get<GetProductByIdResponse>(`/products/${productId}`);
};
