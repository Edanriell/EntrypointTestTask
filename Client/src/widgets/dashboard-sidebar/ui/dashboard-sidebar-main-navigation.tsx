import type { FC, ReactNode } from "react";

import { SidebarGroup, SidebarGroupLabel, SidebarMenu } from "@shared/ui/sidebar";

type DashboardSidebarMainNavigationProps = {
	children: ReactNode;
};

export const DashboardSidebarMainNavigation: FC<DashboardSidebarMainNavigationProps> = ({
	children
}) => {
	return (
		<SidebarGroup>
			<SidebarGroupLabel>Navigation</SidebarGroupLabel>
			<SidebarMenu>{children}</SidebarMenu>
		</SidebarGroup>
	);
};
