import { apiClient } from "@shared/api";

import { CreateProductRequest } from "../model";

export const createProduct = async (data: CreateProductRequest): Promise<string> => {
	return apiClient.post<string>("/products", data);
};
