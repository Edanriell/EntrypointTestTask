import { Currency } from "@entities/products";

import { CURRENCY_MAPPING } from "../../model";

export const currencyComparator = {
	currency: (apiCurrency: string, formCurrency: Currency): boolean => {
		return CURRENCY_MAPPING[apiCurrency] === Currency[formCurrency];
	}
};
