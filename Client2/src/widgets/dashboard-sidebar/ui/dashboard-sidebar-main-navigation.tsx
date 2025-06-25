import Link from "next/link";
import { type LucideIcon } from "lucide-react";

import { Collapsible, CollapsibleTrigger } from "@shared/ui/collapsible";
import {
	SidebarGroup,
	SidebarGroupLabel,
	SidebarMenu,
	SidebarMenuButton,
	SidebarMenuItem
} from "@shared/ui/sidebar";

export function DashboardSidebarMainNavigation({
	items
}: {
	items: {
		title: string;
		url: string;
		icon?: LucideIcon;
	}[];
}) {
	return (
		<SidebarGroup>
			<SidebarGroupLabel>Navigation</SidebarGroupLabel>
			<SidebarMenu>
				{items.map((item) => (
					<Collapsible key={item.title} asChild className="group/collapsible">
						<SidebarMenuItem>
							<CollapsibleTrigger asChild>
								<Link href={item.url}>
									<SidebarMenuButton tooltip={item.title}>
										{item.icon && <item.icon />}
										<span>{item.title}</span>
									</SidebarMenuButton>
								</Link>
							</CollapsibleTrigger>
						</SidebarMenuItem>
					</Collapsible>
				))}
			</SidebarMenu>
		</SidebarGroup>
	);
}
