import { GetOrdersAndRevenueTrendResponse } from "@entities/statistics";

export async function getOrdersAndRevenueTrend(): Promise<GetOrdersAndRevenueTrendResponse> {
	try {
		const res = await fetch("/api/statistics/orders/revenue-trend", {
			method: "GET",
			cache: "no-store",
			next: { revalidate: 0 },
			headers: { "Content-Type": "application/json" }
		});

		if (!res.ok) {
			console.error("Failed to fetch orders and revenue trend:", {
				status: res.status,
				statusText: res.statusText,
				url: res.url
			});

			throw new Error(`Request failed with status ${res.status}`);
		}

		return await res.json();
	} catch (error) {
		console.error("Error fetching orders and revenue trend:", error);
		throw error;
	}
}
