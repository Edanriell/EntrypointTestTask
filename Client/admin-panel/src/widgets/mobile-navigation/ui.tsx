import { FC, ReactNode } from "react";
import Link from "next/link";

import { Sheet, SheetContent, SheetTrigger } from "@shared/ui/sheet";
import { Button } from "@shared/ui/button";
import {
	Home,
	LayoutDashboard,
	LineChart,
	Package,
	PanelLeft,
	Settings,
	ShoppingCart,
	Users2
} from "lucide-react";

type MobileNavLinks = Array<{
	linkName: string;
	linkUrl: string;
	LinkIcon: ReactNode;
}>;

const mobileNavLinks: MobileNavLinks = [
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
	},
	{
		linkName: "Settings",
		linkUrl: "/settings",
		LinkIcon: <Settings className="h-5 w-5" />
	}
];

export const MobileNavigation: FC = () => {
	return (
		<Sheet>
			<SheetTrigger asChild>
				<Button size="icon" variant="outline" className="sm:hidden">
					<PanelLeft className="h-5 w-5" />
					<span className="sr-only">Toggle Menu</span>
				</Button>
			</SheetTrigger>
			<SheetContent side="left" className="sm:max-w-xs">
				<nav className="grid gap-6 text-lg font-medium">
					<Link
						href="/"
						className="group flex h-10 w-10 shrink-0 items-center justify-center gap-2 rounded-full bg-primary text-lg font-semibold text-primary-foreground md:text-base"
					>
						<LayoutDashboard className="h-5 w-5 transition-all group-hover:scale-110" />
						<span className="sr-only">Admin Panel</span>
					</Link>
					{mobileNavLinks.map(({ linkName, linkUrl, LinkIcon }, index) => (
						<Link
							key={index + "-" + linkName}
							href={linkUrl}
							className="flex items-center gap-4 px-2.5 text-muted-foreground hover:text-foreground"
						>
							{LinkIcon}
							{linkName}
						</Link>
					))}
				</nav>
			</SheetContent>
		</Sheet>
	);
};
