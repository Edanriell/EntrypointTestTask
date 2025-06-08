import { FC } from "react";

import { TabsTrigger } from "@shared/ui/tabs";
import { toCamelCase } from "@shared/lib";

type OrderTabsTriggerProps = {
	tabName: string;
	classes?: string;
	onTabClick: (tabName: string) => void;
};

export const OrderTabsTrigger: FC<OrderTabsTriggerProps> = ({ tabName, classes, onTabClick }) => {
	return (
		<TabsTrigger
			onClick={() => onTabClick(toCamelCase(tabName))}
			className={classes}
			value={toCamelCase(tabName)}
		>
			{tabName}
		</TabsTrigger>
	);
};
