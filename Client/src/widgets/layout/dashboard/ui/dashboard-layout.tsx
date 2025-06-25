import { FC, ReactNode } from "react";

import { SidebarProvider } from "@shared/ui/sidebar";

import { Sidebar } from "./sidebar";
import { Content } from "./content";

type DashboardLayoutComponents = {
	Content: typeof Content;
	Sidebar: typeof Sidebar;
};

type DashboardLayoutProps = {
	children: ReactNode;
};

type DashboardLayout = FC<DashboardLayoutProps> & DashboardLayoutComponents;

export const DashboardLayout: DashboardLayout = ({ children }) => {
	return <SidebarProvider>{children}</SidebarProvider>;
};

DashboardLayout.Content = Content;
DashboardLayout.Sidebar = Sidebar;
