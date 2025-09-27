export const orderStatusDataMap = (orderStatus: string) => {
	const orderStatuses: { [key: string]: string } = {
		"0": "Pending",
		"1": "Confirmed",
		"2": "Processing",
		"3": "ReadyForShipment",
		"4": "Shipped",
		"5": "OutForDelivery",
		"6": "Delivered",
		"7": "Completed",
		"8": "Cancelled",
		"9": "Returned",
		"10": "Failed"
	};

	return orderStatuses[orderStatus];
};
