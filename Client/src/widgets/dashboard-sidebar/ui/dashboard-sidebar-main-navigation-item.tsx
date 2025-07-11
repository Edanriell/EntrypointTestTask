import type { FC, ReactNode } from "react";
import Link from "next/link";
import type { LucideIcon } from "lucide-react";

import { SidebarMenuButton, SidebarMenuItem } from "@shared/ui/sidebar";
import { Collapsible, CollapsibleTrigger } from "@shared/ui/collapsible";

type DashboardSidebarMainNavigationItemProps = {
	href: string;
	Icon?: LucideIcon;
	children: ReactNode;
};

export const DashboardSidebarMainNavigationItem: FC<DashboardSidebarMainNavigationItemProps> = ({
	href,
	Icon,
	children
}) => {
	return (
		<Collapsible asChild className="group/collapsible">
			<SidebarMenuItem>
				<CollapsibleTrigger asChild>
					<Link href={href}>
						<SidebarMenuButton tooltip={children?.toString()}>
							{Icon && <Icon />}
							<span>{children}</span>
						</SidebarMenuButton>
					</Link>
				</CollapsibleTrigger>
			</SidebarMenuItem>
		</Collapsible>
	);
};
