import { Currency } from "@entities/payments";

export const mapCurrencyToEnum = (currency: string): Currency => {
	switch (currency?.toUpperCase()) {
		case "USD":
			return "Usd" as Currency;
		case "EUR":
			return "Eur" as Currency;
		default:
			return "Usd" as Currency; // Default fallback
	}
};
