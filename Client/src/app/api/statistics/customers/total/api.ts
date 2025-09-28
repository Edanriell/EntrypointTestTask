import { GetTotalCustomersResponse } from "@entities/statistics";

export async function getTotalCustomers(): Promise<GetTotalCustomersResponse> {
	try {
		const res = await fetch("/api/statistics/customers/total", {
			method: "GET",
			cache: "no-store",
			next: { revalidate: 0 },
			headers: { "Content-Type": "application/json" }
		});

		if (!res.ok) {
			console.error("Failed to fetch total customers:", {
				status: res.status,
				statusText: res.statusText,
				url: res.url
			});

			throw new Error(`Request failed with status ${res.status}`);
		}

		return await res.json();
	} catch (error) {
		console.error("Error fetching total customers:", error);
		throw error;
	}
}
