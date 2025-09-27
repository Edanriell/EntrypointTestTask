import { FC } from "react";

import { Card, CardContent } from "@shared/ui/card";

export const OrdersNotFound: FC = () => {
	return (
		<Card>
			<CardContent className="pt-6">
				<div className="text-center text-muted-foreground py-8">
					No orders found. Try adjusting your search or filters.
				</div>
			</CardContent>
		</Card>
	);
};
