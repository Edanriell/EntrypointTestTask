import { Currency } from "@entities/products";

export const currencyComparator = {
	currency: (apiCurrency: string, formCurrency: Currency): boolean => {
		// return CURRENCY_MAPPING[apiCurrency] === formCurrency;
		return apiCurrency === Currency[formCurrency];
	}
};
