import Link from "next/link";

import { Card, CardDescription, CardFooter, CardHeader, CardTitle } from "@shared/ui/card";

export const CreateOrder = () => {
	return (
		<Card className="sm:col-span-2" x-chunk="dashboard-05-chunk-0">
			<CardHeader className="pb-3">
				<CardTitle>Orders</CardTitle>
				<CardDescription className="max-w-lg text-balance leading-relaxed">
					Manage all orders in store and create new ones efficiently.
				</CardDescription>
			</CardHeader>
			<CardFooter>
				<Link
					className="inline-flex items-center justify-center whitespace-nowrap rounded-md text-sm font-medium ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50 bg-primary text-primary-foreground hover:bg-primary/90 h-10 px-4 py-2"
					href="/dashboard/orders/create"
				>
					Create New Order
				</Link>
			</CardFooter>
		</Card>
	);
};
