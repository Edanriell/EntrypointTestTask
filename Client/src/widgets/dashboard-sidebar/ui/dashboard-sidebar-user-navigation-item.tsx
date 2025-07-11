import type { FC, ReactNode } from "react";
import type { LucideIcon } from "lucide-react";

import { DropdownMenuItem } from "@shared/ui/dropdown-menu";

type DashboardSidebarUserNavigationItemProps = {
	children: ReactNode;
	Icon: LucideIcon;
};

export const DashboardSidebarUserNavigationItem: FC<DashboardSidebarUserNavigationItemProps> = ({
	children,
	Icon
}) => {
	return (
		<DropdownMenuItem>
			{Icon && <Icon />}
			{children}
		</DropdownMenuItem>
	);
};
