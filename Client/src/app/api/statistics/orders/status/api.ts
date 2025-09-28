import { GetOrderStatusDistributionResponse } from "@entities/statistics";

export async function getOrderStatusDistribution(): Promise<GetOrderStatusDistributionResponse> {
	try {
		const res = await fetch("/api/statistics/orders/status", {
			method: "GET",
			cache: "no-store",
			next: { revalidate: 0 },
			headers: { "Content-Type": "application/json" }
		});

		if (!res.ok) {
			console.error("Failed to fetch order status distribution:", {
				status: res.status,
				statusText: res.statusText,
				url: res.url
			});

			throw new Error(`Request failed with status ${res.status}`);
		}

		return await res.json();
	} catch (error) {
		console.error("Error fetching order status distribution:", error);
		throw error;
	}
}
