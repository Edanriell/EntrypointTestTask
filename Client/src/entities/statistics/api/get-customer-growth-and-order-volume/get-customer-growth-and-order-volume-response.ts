import { Month } from "../../model";

export type GetCustomerGrowthAndOrderVolumeResponse = {
	trend: Array<CustomerGrowthOrders>;
};

type CustomerGrowthOrders = {
	month: Month;
	totalCustomers: number;
	totalOrders: number;
};
