import { FC, ReactNode } from "react";
import Link from "next/link";
import {
	Home,
	LayoutDashboard,
	LineChart,
	Package,
	Settings,
	ShoppingCart,
	Users2
} from "lucide-react";

import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@shared/ui/tooltip";

type NavLinks = Array<{
	linkName: string;
	linkUrl: string;
	LinkIcon: ReactNode;
}>;

const navLinks: NavLinks = [
	{
		linkName: "Home",
		linkUrl: "/dashboard",
		LinkIcon: <Home className="h-5 w-5" />
	},
	{
		linkName: "Orders",
		linkUrl: "/dashboard/orders",
		LinkIcon: <ShoppingCart className="h-5 w-5" />
	},
	{
		linkName: "Products",
		linkUrl: "/dashboard/products",
		LinkIcon: <Package className="h-5 w-5" />
	},
	{
		linkName: "Customers",
		linkUrl: "/dashboard/customers",
		LinkIcon: <Users2 className="h-5 w-5" />
	},
	{
		linkName: "Analytics",
		linkUrl: "/dashboard/analytics",
		LinkIcon: <LineChart className="h-5 w-5" />
	}
];

export const Sidebar: FC = () => {
	return (
		<aside className="fixed inset-y-0 left-0 z-10 hidden w-14 flex-col border-r bg-background sm:flex">
			<nav className="flex flex-col items-center gap-4 px-2 py-4">
				<Link
					href="/"
					className="group flex h-9 w-9 shrink-0 items-center justify-center gap-2 rounded-full bg-primary text-lg font-semibold text-primary-foreground md:h-8 md:w-8 md:text-base"
				>
					<LayoutDashboard className="h-4 w-4 transition-all group-hover:scale-110" />
					<span className="sr-only">Admin Panel</span>
				</Link>
				{navLinks.map(({ linkName, linkUrl, LinkIcon }, index) => (
					<TooltipProvider key={index + "-" + linkName}>
						<Tooltip>
							<TooltipTrigger asChild>
								<Link
									href={linkUrl}
									className="flex h-9 w-9 items-center justify-center rounded-lg text-muted-foreground transition-colors hover:text-foreground md:h-8 md:w-8"
								>
									{LinkIcon}
									<span className="sr-only">{linkName}</span>
								</Link>
							</TooltipTrigger>
							<TooltipContent side="right">{linkName}</TooltipContent>
						</Tooltip>
					</TooltipProvider>
				))}
			</nav>
			<nav className="mt-auto flex flex-col items-center gap-4 px-2 py-4">
				<TooltipProvider>
					<Tooltip>
						<TooltipTrigger asChild>
							<Link
								href="/settings"
								className="flex h-9 w-9 items-center justify-center rounded-lg text-muted-foreground transition-colors hover:text-foreground md:h-8 md:w-8"
							>
								<Settings className="h-5 w-5" />
								<span className="sr-only">Settings</span>
							</Link>
						</TooltipTrigger>
						<TooltipContent side="right">Settings</TooltipContent>
					</Tooltip>
				</TooltipProvider>
			</nav>
		</aside>
	);
};
