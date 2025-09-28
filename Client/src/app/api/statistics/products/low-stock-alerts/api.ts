import { GetLowStockAlertsResponse } from "@entities/statistics";

export async function getLowStockAlerts(): Promise<GetLowStockAlertsResponse> {
	try {
		const res = await fetch("/api/statistics/products/low-stock-alerts", {
			method: "GET",
			cache: "no-store",
			next: { revalidate: 0 },
			headers: { "Content-Type": "application/json" }
		});

		if (!res.ok) {
			console.error("Failed to fetch low stock alerts:", {
				status: res.status,
				statusText: res.statusText,
				url: res.url
			});

			throw new Error(`Request failed with status ${res.status}`);
		}

		return await res.json();
	} catch (error) {
		console.error("Error fetching low stock alerts:", error);
		throw error;
	}
}
