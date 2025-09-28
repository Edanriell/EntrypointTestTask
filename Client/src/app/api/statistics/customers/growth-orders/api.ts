import { GetCustomerGrowthAndOrderVolumeResponse } from "@entities/statistics";

export async function getCustomerGrowthAndOrderVolume(): Promise<GetCustomerGrowthAndOrderVolumeResponse> {
	try {
		const res = await fetch("/api/statistics/customers/growth-orders", {
			method: "GET",
			cache: "no-store",
			next: { revalidate: 0 },
			headers: { "Content-Type": "application/json" }
		});

		if (!res.ok) {
			console.error("Failed to fetch customer growth and order volume:", {
				status: res.status,
				statusText: res.statusText,
				url: res.url
			});

			throw new Error(`Request failed with status ${res.status}`);
		}

		return await res.json();
	} catch (error) {
		console.error("Error fetching customer growth and order volume:", error);
		throw error;
	}
}
