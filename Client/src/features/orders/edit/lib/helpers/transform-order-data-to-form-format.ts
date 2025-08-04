import { parseAddressString } from "./parse-address-string";

// Transform order data to match form data structure
// Converts: { country: "Germany" } (partial form changes)
// Plus: "3873 Gunnar Village, London, France, 20974" (current address)
// Into: "3873 Gunnar Village, London, Germany, 20974" (updated address string)
export const transformOrderDataToFormFormat = (orderData: any): EditOrderFormData => {
	let addressData: Partial<EditOrderFormData> = {
		street: "",
		city: "",
		country: "",
		zipCode: ""
	};

	// Handle different address formats
	if (typeof orderData.shippingAddress === "string") {
		addressData = parseAddressString(orderData.shippingAddress);
	} else if (orderData.shippingAddress && typeof orderData.shippingAddress === "object") {
		addressData = {
			street: orderData.shippingAddress.street || "",
			city: orderData.shippingAddress.city || "",
			country: orderData.shippingAddress.country || "",
			zipCode: orderData.shippingAddress.zipCode || ""
		};
	}

	return {
		street: addressData.street || "",
		city: addressData.city || "",
		country: addressData.country || "",
		zipCode: addressData.zipCode || "",
		info: orderData.info || ""
	};
};
