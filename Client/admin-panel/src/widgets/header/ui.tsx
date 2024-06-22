"use client";

import { FC, ReactNode } from "react";
import Link from "next/link";
import {
	CircleUser,
	Home,
	LayoutDashboard,
	LineChart,
	Package,
	PanelLeft,
	ShoppingCart,
	Users2,
	Settings
} from "lucide-react";
import { signOut } from "next-auth/react";

import {
	Button,
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuLabel,
	DropdownMenuSeparator,
	DropdownMenuTrigger,
	Sheet,
	SheetContent,
	SheetTrigger
} from "@shared/ui";

type HeaderProps = {
	children?: ReactNode;
};

export const Header: FC<HeaderProps> = ({ children }) => {
	const handleSignOutClick = async () => {
		await signOut();
	};

	return (
		<header className="sticky top-0 z-30 flex h-14 items-center gap-4 border-b bg-background px-4 sm:static sm:h-auto sm:border-0 sm:bg-transparent sm:px-6">
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
						<Link
							href="/dashboard"
							className="flex items-center gap-4 px-2.5 text-muted-foreground hover:text-foreground"
						>
							<Home className="h-5 w-5" />
							Home
						</Link>
						<Link
							href="/dashboard/orders"
							className="flex items-center gap-4 px-2.5 text-muted-foreground hover:text-foreground"
						>
							<ShoppingCart className="h-5 w-5" />
							Orders
						</Link>
						<Link
							href="/dashboard/products"
							className="flex items-center gap-4 px-2.5 text-muted-foreground hover:text-foreground"
						>
							<Package className="h-5 w-5" />
							Products
						</Link>
						<Link
							href="/dashboard/customers"
							className="flex items-center gap-4 px-2.5 text-muted-foreground hover:text-foreground"
						>
							<Users2 className="h-5 w-5" />
							Customers
						</Link>
						<Link
							href="/dashboard/analytics"
							className="flex items-center gap-4 px-2.5 text-muted-foreground hover:text-foreground"
						>
							<LineChart className="h-5 w-5" />
							Analytics
						</Link>
						<Link
							href="/settings"
							className="flex items-center gap-4 px-2.5 text-muted-foreground hover:text-foreground"
						>
							<Settings className="h-5 w-5" />
							Settings
						</Link>
					</nav>
				</SheetContent>
			</Sheet>
			{children}
			<div className={"ml-auto"}>
				<DropdownMenu>
					<DropdownMenuTrigger asChild>
						<Button variant="secondary" size="icon" className="rounded-full">
							<CircleUser className="h-5 w-5" />
							<span className="sr-only">Toggle user menu</span>
						</Button>
					</DropdownMenuTrigger>
					<DropdownMenuContent align="end">
						<DropdownMenuLabel>My Account</DropdownMenuLabel>
						<DropdownMenuSeparator />
						<DropdownMenuItem>
							<Link href="/settings">Settings</Link>
						</DropdownMenuItem>
						<DropdownMenuSeparator />
						<DropdownMenuItem>
							<button onClick={handleSignOutClick}>Logout</button>
						</DropdownMenuItem>
					</DropdownMenuContent>
				</DropdownMenu>
			</div>
		</header>
	);
};
