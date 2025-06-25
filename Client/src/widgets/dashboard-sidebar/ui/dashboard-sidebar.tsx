"use client";

import * as React from "react";
import { Box, ClipboardList, Users } from "lucide-react";

import { DashboardSidebarMainNavigation } from "./dashboard-sidebar-main-navigation";
import { DashboardSidebarUserNavigation } from "./dashboard-sidebar-user-navigation";
import { DashboardSidebarHeader } from "./dashboard-sidebar-header";

import {
	Sidebar,
	SidebarContent,
	SidebarFooter,
	SidebarHeader,
	SidebarRail
} from "@shared/ui/sidebar";

// This is sample data.
const data = {
	user: {
		name: "shadcn",
		email: "m@example.com",
		avatar: "/avatars/shadcn.jpg"
	}
};

const mainNavItems = [
	{
		title: "Clients",
		url: "clients",
		icon: Users
	},
	{
		title: "Orders",
		url: "orders",
		icon: ClipboardList
	},
	{
		title: "Products",
		url: "products",
		icon: Box
	}
];

export function DashboardSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {
	return (
		<Sidebar collapsible="icon" {...props}>
			<SidebarHeader>
				<DashboardSidebarHeader />
			</SidebarHeader>
			<SidebarContent>
				<DashboardSidebarMainNavigation items={mainNavItems} />
			</SidebarContent>
			<SidebarFooter>
				<DashboardSidebarUserNavigation user={data.user} />
			</SidebarFooter>
			<SidebarRail />
		</Sidebar>
	);
}
