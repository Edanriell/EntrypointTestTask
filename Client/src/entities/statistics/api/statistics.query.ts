import { ApiError } from "@shared/lib/handlers";
import { keepPreviousData, queryOptions } from "@tanstack/react-query";
import {
	getActiveProducts,
	getCustomerGrowthAndOrderVolume,
	getInventoryStatus,
	getLowStockAlerts,
	getMonthlyRevenue,
	getOrdersAndRevenueTrend,
	getOrderStatusDistribution,
	getRecentOrders,
	getTopSellingProducts,
	getTotalCustomers,
	getTotalOrders
} from "@entities/statistics";

const MAX_RETRIES = 3;

// v5 retry handler signature: (failureCount, error) => boolean
function retryHandler(failureCount: number, error: unknown): boolean {
	const apiError = error as ApiError | undefined;
	const status = apiError?.response?.status;

	// Do not retry client errors (4xx) except 429 (rate limit).
	if (status && status >= 400 && status < 500) {
		return status === 429 && failureCount < MAX_RETRIES;
	}

	// Retry on network / server errors up to MAX_RETRIES
	return failureCount < MAX_RETRIES;
}

function retryDelay(attemptIndex: number): number {
	// Exponential backoff capped at 30s
	return Math.min(1000 * 2 ** attemptIndex, 30_000);
}

export const statisticsQueries = {
	all: () => ["statistics"] as const,

	activeProducts: () =>
		queryOptions({
			queryKey: [...statisticsQueries.all(), "activeProducts"],
			queryFn: () => getActiveProducts(),
			staleTime: 2 * 60 * 1000,
			retry: retryHandler,
			retryDelay
		}),

	customerGrowthAndOrderVolume: () =>
		queryOptions({
			queryKey: [...statisticsQueries.all(), "customerGrowthAndOrderVolume"],
			queryFn: () => getCustomerGrowthAndOrderVolume(),
			staleTime: 5 * 60 * 1000,
			retry: retryHandler,
			retryDelay
		}),

	inventoryStatus: () =>
		queryOptions({
			queryKey: [...statisticsQueries.all(), "inventoryStatus"],
			queryFn: () => getInventoryStatus(),
			staleTime: 5 * 60 * 1000,
			retry: retryHandler,
			retryDelay
		}),

	lowStockAlerts: () =>
		queryOptions({
			queryKey: [...statisticsQueries.all(), "lowStockAlerts"],
			queryFn: () => getLowStockAlerts(),
			staleTime: 60 * 1000,
			retry: retryHandler,
			retryDelay
		}),

	monthlyRevenue: () =>
		queryOptions({
			queryKey: [...statisticsQueries.all(), "monthlyRevenue"],
			queryFn: () => getMonthlyRevenue(),
			staleTime: 2 * 60 * 1000,
			retry: retryHandler,
			retryDelay
		}),

	ordersAndRevenueTrend: () =>
		queryOptions({
			queryKey: [...statisticsQueries.all(), "ordersAndRevenueTrend"],
			queryFn: () => getOrdersAndRevenueTrend(),
			staleTime: 5 * 60 * 1000,
			retry: retryHandler,
			retryDelay
		}),

	orderStatusDistribution: () =>
		queryOptions({
			queryKey: [...statisticsQueries.all(), "orderStatusDistribution"],
			queryFn: () => getOrderStatusDistribution(),
			staleTime: 10 * 60 * 1000,
			retry: retryHandler,
			retryDelay
		}),

	recentOrders: () =>
		queryOptions({
			queryKey: [...statisticsQueries.all(), "recentOrders"],
			queryFn: () => getRecentOrders(),
			// show previous list while fetching new page -> use keepPreviousData
			placeholderData: keepPreviousData,
			staleTime: 30 * 1000,
			retry: retryHandler,
			retryDelay
		}),

	topSellingProducts: () =>
		queryOptions({
			queryKey: [...statisticsQueries.all(), "topSellingProducts"],
			queryFn: () => getTopSellingProducts(),
			staleTime: 5 * 60 * 1000,
			retry: retryHandler,
			retryDelay
		}),

	totalCustomers: () =>
		queryOptions({
			queryKey: [...statisticsQueries.all(), "totalCustomers"],
			queryFn: () => getTotalCustomers(),
			staleTime: 2 * 60 * 1000,
			retry: retryHandler,
			retryDelay
		}),

	totalOrders: () =>
		queryOptions({
			queryKey: [...statisticsQueries.all(), "totalOrders"],
			queryFn: () => getTotalOrders(),
			staleTime: 2 * 60 * 1000,
			retry: retryHandler,
			retryDelay
		})
};
