import { FC } from "react";
import { GalleryVerticalEnd } from "lucide-react";
import Link from "next/link";

import { SidebarMenuButton } from "@shared/ui/sidebar";

export const DashboardSidebarHeader: FC = () => {
	return (
		<Link href="/">
			<SidebarMenuButton
				size="lg"
				className="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground"
			>
				<div className="bg-sidebar-primary text-sidebar-primary-foreground flex aspect-square size-8 items-center justify-center rounded-lg">
					<GalleryVerticalEnd className="size-5" />
				</div>
				<div className="grid flex-1 text-left text-sm leading-tight">
					<span className="truncate font-medium">Dashboard</span>
					<span className="truncate text-xs">Enterprise</span>
				</div>
			</SidebarMenuButton>
		</Link>
	);
};
