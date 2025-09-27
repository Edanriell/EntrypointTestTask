import { InventoryStatus } from "../../model";

export type GetInventoryStatusResponse = {
	inventorySummary: Array<ProductInventory>;
};

type ProductInventory = {
	inventoryStatus: InventoryStatus;
	count: number;
};
