import type { Metadata } from "next";
import { FC, ReactNode } from "react";

import { DashboardLayout } from "@widgets/layouts/dashboard-layout";

export const metadata: Metadata = {
	title: "Admin Panel Dashboard",
	description: "Admin Panel Dashboard"
};

type LayoutProps = {
	children: ReactNode;
};

const Layout: FC<LayoutProps> = ({ children }) => {
	return <DashboardLayout>{children}</DashboardLayout>;
};

export default Layout;
