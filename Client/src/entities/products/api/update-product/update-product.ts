import { apiClient } from "@shared/api";

import type {
	UpdateProductPriceRequest,
	UpdateProductRequest,
	UpdateProductReservedStockRequest,
	UpdateProductStockRequest
} from "../model";

export const updateProduct = async (
	productId: string,
	productData: UpdateProductRequest
): Promise<void> => {
	return apiClient.put<void>(`/products/${productId}`, productData);
};

export const updateProductPrice = async (
	productId: string,
	productData: UpdateProductPriceRequest
): Promise<void> => {
	return apiClient.patch<void>(`/products/${productId}/price`, productData);
};

export const updateProductStock = async (
	productId: string,
	productData: UpdateProductStockRequest
): Promise<void> => {
	return apiClient.patch<void>(`/products/${productId}/stock`, productData);
};

export const updateProductReservedStock = async (
	productId: string,
	productData: UpdateProductReservedStockRequest
): Promise<void> => {
	return apiClient.patch<void>(`/products/${productId}/reserved-stock`, productData);
};
