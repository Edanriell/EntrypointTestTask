import { Children, ComponentProps, FC, isValidElement, ReactNode } from "react";

import {
	Sidebar,
	SidebarContent,
	SidebarFooter,
	SidebarHeader,
	SidebarRail
} from "@shared/ui/sidebar";

import { DashboardSidebarMainNavigation } from "./dashboard-sidebar-main-navigation";
import { DashboardSidebarUserNavigation } from "./dashboard-sidebar-user-navigation";
import { DashboardSidebarHeader } from "./dashboard-sidebar-header";
import { DashboardSidebarMainNavigationItem } from "./dashboard-sidebar-main-navigation-item";
import { DashboardSidebarUserNavigationItem } from "./dashboard-sidebar-user-navigation-item";
import { DashboardSidebarUserNavigationItemGroup } from "./dashboard-sidebar-user-navigation-item-group";
import { DashboardSidebarUserNavigationItemSeparator } from "./dashboard-sidebar-user-navigation-item-separator";

type DashboardSidebarProps = {
	children: ReactNode;
};

export const DashboardSidebarRoot: FC<DashboardSidebarProps> = ({
	children,
	...props
}: ComponentProps<typeof Sidebar>) => {
	let mainNavigation: ReactNode = null;
	let userNavigation: ReactNode = null;

	Children.forEach(children, (child) => {
		if (isValidElement(child)) {
			if (child.type === DashboardSidebarMainNavigation) {
				mainNavigation = child;
			} else if (child.type === DashboardSidebarUserNavigation) {
				userNavigation = child;
			}
		}
	});

	return (
		<Sidebar collapsible="icon" {...props}>
			<SidebarHeader>
				<DashboardSidebarHeader />
			</SidebarHeader>
			<SidebarContent>{mainNavigation}</SidebarContent>
			<SidebarFooter>{userNavigation}</SidebarFooter>
			<SidebarRail />
		</Sidebar>
	);
};

export const DashboardSidebar = Object.assign(DashboardSidebarRoot, {
	MainNavigation: DashboardSidebarMainNavigation,
	MainNavigationItem: DashboardSidebarMainNavigationItem,
	UserNavigation: DashboardSidebarUserNavigation,
	UserNavigationItem: DashboardSidebarUserNavigationItem,
	UserNavigationItemGroup: DashboardSidebarUserNavigationItemGroup,
	UserNavigationItemSeparator: DashboardSidebarUserNavigationItemSeparator
});
