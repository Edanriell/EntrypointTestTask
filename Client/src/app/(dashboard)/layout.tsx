import { ReactNode } from "react";
import { BadgeCheck, Box, ClipboardList, LogOut, Users } from "lucide-react";
import { getServerSession } from "next-auth";

import { DashboardLayout } from "@widgets/layout/dashboard";
import type { UserData } from "@widgets/dashboard-sidebar";
import { DashboardSidebar } from "@widgets/dashboard-sidebar";

import { SignOut } from "@features/authentication/sign-out";

type LayoutProps = {
	children: ReactNode;
};

export default async function Layout({ children }: LayoutProps) {
	const {
		user: { name, email, avatar }
	} = (await getServerSession()) as { user: UserData };

	return (
		<DashboardLayout>
			<DashboardLayout.Sidebar>
				<DashboardSidebar>
					<DashboardSidebar.MainNavigation>
						<DashboardSidebar.MainNavigationItem Icon={Users} href="clients">
							Clients
						</DashboardSidebar.MainNavigationItem>
						<DashboardSidebar.MainNavigationItem Icon={ClipboardList} href="orders">
							Orders
						</DashboardSidebar.MainNavigationItem>
						<DashboardSidebar.MainNavigationItem Icon={Box} href="products">
							Products
						</DashboardSidebar.MainNavigationItem>
					</DashboardSidebar.MainNavigation>
					<DashboardSidebar.UserNavigation user={{ name, email, avatar }}>
						<DashboardSidebar.UserNavigationItemSeparator />
						<DashboardSidebar.UserNavigationItemGroup>
							<DashboardSidebar.UserNavigationItem Icon={BadgeCheck}>
								Settings
							</DashboardSidebar.UserNavigationItem>
						</DashboardSidebar.UserNavigationItemGroup>
						<DashboardSidebar.UserNavigationItemSeparator />
						<DashboardSidebar.UserNavigationItem Icon={LogOut}>
							<SignOut />
						</DashboardSidebar.UserNavigationItem>
					</DashboardSidebar.UserNavigation>
				</DashboardSidebar>
			</DashboardLayout.Sidebar>
			<DashboardLayout.Content>{children}</DashboardLayout.Content>
		</DashboardLayout>
	);
}
