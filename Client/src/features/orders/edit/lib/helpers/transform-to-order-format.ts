// Helper function to transform form data back to order data format
// This is necessary for our optimistic update, because we send data in format
// {
// street?: string;
// city?: string;
// zipCode?: string;
// country?: string;
// info?: string;
// }
// But data held in by a query is in whole string format
// "country, city, street, zipCode"
export const transformToOrderFormat = (updatedOrderData: any, oldData: GetOrderByIdResponse) => {
	const result = { ...oldData };

	// If we have address fields, reconstruct the shippingAddress string
	const hasAddressFields = ["street", "city", "country", "zipCode"].some((field) =>
		updatedOrderData.hasOwnProperty(field)
	);

	if (hasAddressFields) {
		// Parse current address or use empty defaults
		const parseAddressString = (addressString: string) => {
			const parts = addressString.split(", ");
			return parts.length >= 4
				? {
						street: parts[0] || "",
						city: parts[1] || "",
						country: parts[2] || "",
						zipCode: parts[3] || ""
					}
				: {
						street: addressString,
						city: "",
						country: "",
						zipCode: ""
					};
		};

		const currentAddress = oldData.shippingAddress
			? parseAddressString(oldData.shippingAddress)
			: { street: "", city: "", country: "", zipCode: "" };

		// Merge with updated fields
		const updatedAddress = {
			street: updatedOrderData.street ?? currentAddress.street,
			city: updatedOrderData.city ?? currentAddress.city,
			country: updatedOrderData.country ?? currentAddress.country,
			zipCode: updatedOrderData.zipCode ?? currentAddress.zipCode
		};

		// Reconstruct the address string
		result.shippingAddress = `${updatedAddress.street}, ${updatedAddress.city}, ${updatedAddress.country}, ${updatedAddress.zipCode}`;
	}

	// Handle other fields (like info)
	if (updatedOrderData.info !== undefined) {
		result.info = updatedOrderData.info;
	}

	return result;
};
