import { GetRecentOrdersResponse } from "@entities/statistics";

export async function getRecentOrders(): Promise<GetRecentOrdersResponse> {
	try {
		const res = await fetch("/api/statistics/orders/recent", {
			method: "GET",
			cache: "no-store",
			next: { revalidate: 0 },
			headers: { "Content-Type": "application/json" }
		});

		if (!res.ok) {
			console.error("Failed to fetch recent orders:", {
				status: res.status,
				statusText: res.statusText,
				url: res.url
			});

			throw new Error(`Request failed with status ${res.status}`);
		}

		return await res.json();
	} catch (error) {
		console.error("Error fetching recent orders:", error);
		throw error;
	}
}
