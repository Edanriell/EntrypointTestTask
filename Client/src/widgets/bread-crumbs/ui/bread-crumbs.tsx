"use client";

import { Fragment } from "react";

import { usePathname } from "next/navigation";
import Link from "next/link";

import {
	Breadcrumb,
	BreadcrumbItem,
	BreadcrumbLink,
	BreadcrumbList,
	BreadcrumbPage,
	BreadcrumbSeparator
} from "@shared/ui/breadcrumb";

export const BreadCrumbs = () => {
	const pathname = usePathname();

	// Split pathname into segments
	const segments = pathname.split("/").filter((segment) => segment.length > 0);

	// Build breadcrumb paths for each segment
	const breadcrumbs = segments.map((segment, index) => {
		const href = "/" + segments.slice(0, index + 1).join("/");
		const label = segment.charAt(0).toUpperCase() + segment.slice(1);
		return { href, label, isLast: index === segments.length - 1 };
	});

	return (
		<Breadcrumb>
			<BreadcrumbList>
				<BreadcrumbItem>
					<BreadcrumbLink asChild>
						<Link href="/">Home</Link>
					</BreadcrumbLink>
				</BreadcrumbItem>
				{breadcrumbs.map((crumb, index) => (
					<Fragment key={crumb.href}>
						<BreadcrumbSeparator />
						<BreadcrumbItem>
							{crumb.isLast ? (
								<BreadcrumbPage>{crumb.label}</BreadcrumbPage>
							) : (
								<BreadcrumbLink asChild>
									<Link href={crumb.href}>{crumb.label}</Link>
								</BreadcrumbLink>
							)}
						</BreadcrumbItem>
					</Fragment>
				))}
			</BreadcrumbList>
		</Breadcrumb>
	);
};
