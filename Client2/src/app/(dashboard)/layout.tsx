import { ReactNode } from "react";

import { DashboardLayout } from "@widgets/layout/dashboard";
import { DashboardSidebar } from "@widgets/dashboard-sidebar";

type LayoutProps = {
	children: ReactNode;
};

export default function Layout({ children }: LayoutProps) {
	return (
		<DashboardLayout>
			<DashboardLayout.Sidebar>
				<DashboardSidebar />
			</DashboardLayout.Sidebar>
			<DashboardLayout.Content>{children}</DashboardLayout.Content>
		</DashboardLayout>
	);
}
