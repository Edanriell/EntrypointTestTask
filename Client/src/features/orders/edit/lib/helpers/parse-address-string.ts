// Helper function to parse address string into form format
// Converts: "3873 Gunnar Village, London, France, 20974"
// Into: { street: "3873 Gunnar Village", city: "London", country: "France", zipCode: "20974" }
export const parseAddressString = (addressString: string): Partial<EditOrderFormData> => {
	const parts = addressString.split(", ");

	if (parts.length >= 4) {
		return {
			street: parts[0] || "",
			city: parts[1] || "",
			country: parts[2] || "",
			zipCode: parts[3] || ""
		};
	}

	// Fallback for unexpected format
	return {
		street: addressString,
		city: "",
		country: "",
		zipCode: ""
	};
};
