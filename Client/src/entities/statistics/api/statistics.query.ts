import { ApiError } from "@shared/lib/handlers";
import { keepPreviousData, queryOptions } from "@tanstack/react-query";

// NextJs route handlers, we are using them for performance reasons (They execute in parallel)
import { getTotalOrders } from "@app/api/statistics/orders/total/api";
import { getActiveProducts } from "@app/api/statistics/products/active/api";
import { getTotalCustomers } from "@app/api/statistics/customers/total/api";
import { getMonthlyRevenue } from "@app/api/statistics/revenue/monthly/api";
import { getOrdersAndRevenueTrend } from "@app/api/statistics/orders/revenue-trend/api";
import { getCustomerGrowthAndOrderVolume } from "@app/api/statistics/customers/growth-orders/api";
import { getInventoryStatus } from "@app/api/statistics/inventory/status/api";
import { getLowStockAlerts } from "@app/api/statistics/products/low-stock-alerts/api";
import { getOrderStatusDistribution } from "@app/api/statistics/orders/status/api";
import { getRecentOrders } from "@app/api/statistics/orders/recent/api";
import { getTopSellingProducts } from "@app/api/statistics/products/top-selling/api";

export const statisticsQueries = {
	all: () => ["statistics"] as const,

	activeProducts: () =>
		queryOptions({
			queryKey: [...statisticsQueries.all(), "activeProducts"],
			// queryFn: () => getActiveProducts(),
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
			// queryFn: () => getTotalOrders()
			queryFn: getTotalOrders,
			staleTime: 2 * 60 * 1000,
			retry: retryHandler,
			retryDelay
		})
};

// Cleaner ?
const MAX_RETRIES = 3;

function retryHandler(failureCount: number, error: unknown): boolean {
	const apiError = error as ApiError | undefined;
	const status = apiError?.response?.status;

	if (status && status >= 400 && status < 500) {
		return status === 429 && failureCount < MAX_RETRIES;
	}

	return failureCount < MAX_RETRIES;
}

function retryDelay(attemptIndex: number): number {
	return Math.min(1000 * 2 ** attemptIndex, 30_000);
}
