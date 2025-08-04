import { apiClient } from "@shared/api";

import type { CreateProductCommand } from "./create-product-command";
import type { CreateProductResponse } from "./create-product-response";

export const createProduct = async (data: CreateProductCommand): Promise<CreateProductResponse> => {
	return apiClient.post<CreateProductResponse>("/products", data);
};
