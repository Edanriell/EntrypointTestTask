export const mapCurrencyToDisplay = (currency: string): string => {
	switch (currency?.toUpperCase()) {
		case "USD":
			return "USD";
		case "EUR":
			return "EUR";
		default:
			return currency || "";
	}
};
