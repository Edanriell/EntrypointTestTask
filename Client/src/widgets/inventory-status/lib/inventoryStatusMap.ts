import { InventoryStatus } from "@entities/statistics";

export const inventoryStatusMap = (inventoryStatus: InventoryStatus) => {
	const inventory = {
		"0": "In Stock",
		"1": "On Order",
		"2": "Out Of Stock",
		"3": "Low Stock"
	};

	return inventory[inventoryStatus];
};
