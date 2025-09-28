import { GetMonthlyRevenueResponse } from "@entities/statistics";

export async function getMonthlyRevenue(): Promise<GetMonthlyRevenueResponse> {
	try {
		const res = await fetch("/api/statistics/revenue/monthly", {
			method: "GET",
			cache: "no-store",
			next: { revalidate: 0 },
			headers: { "Content-Type": "application/json" }
		});

		if (!res.ok) {
			console.error("Failed to fetch monthly revenue:", {
				status: res.status,
				statusText: res.statusText,
				url: res.url
			});

			throw new Error(`Request failed with status ${res.status}`);
		}

		return await res.json();
	} catch (error) {
		console.error("Error fetching monthly revenue:", error);
		throw error;
	}
}
