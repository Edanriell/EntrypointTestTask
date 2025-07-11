import type { FC, ReactNode } from "react";

import { DropdownMenuGroup } from "@shared/ui/dropdown-menu";

type DashboardSidebarUserNavigationItemGroupProps = {
	children: ReactNode;
};

export const DashboardSidebarUserNavigationItemGroup: FC<
	DashboardSidebarUserNavigationItemGroupProps
> = ({ children }) => {
	return <DropdownMenuGroup>{children}</DropdownMenuGroup>;
};
