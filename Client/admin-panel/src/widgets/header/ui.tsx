"use client";

import { FC, Fragment } from "react";
import Link from "next/link";

import { MobileNavigation } from "@widgets/mobile-navigation";
import { UserProfile } from "@widgets/user-profile";

import {
	Breadcrumb,
	BreadcrumbLink,
	BreadcrumbList,
	BreadcrumbPage,
	BreadcrumbSeparator
} from "@shared/ui/breadcrumb";
import { useBreadcrumbs } from "@shared/lib/hooks";

export const Header: FC = () => {
	const breadcrumbs = useBreadcrumbs();

	return (
		<header className="sticky top-0 z-30 flex h-14 items-center gap-4 border-b bg-background px-4 sm:static sm:h-auto sm:border-0 sm:bg-transparent sm:px-6">
			<MobileNavigation />
			<Breadcrumb className="hidden md:flex items-center justify-center">
				<BreadcrumbList>
					{breadcrumbs.map((breadcrumb, index) => (
						<Fragment key={breadcrumb.href}>
							{index < breadcrumbs.length - 1 ? (
								<Fragment>
									<BreadcrumbLink asChild>
										<Link href={breadcrumb.href}>{breadcrumb.name}</Link>
									</BreadcrumbLink>
									<BreadcrumbSeparator className="flex items-center justify-center" />
								</Fragment>
							) : (
								<BreadcrumbPage>{breadcrumb.name}</BreadcrumbPage>
							)}
						</Fragment>
					))}
				</BreadcrumbList>
			</Breadcrumb>
			<div className={"ml-auto"}>
				<UserProfile />
			</div>
		</header>
	);
};
