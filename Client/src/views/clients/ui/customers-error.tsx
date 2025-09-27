import { OctagonAlert } from "lucide-react";

import { Card, CardContent } from "@shared/ui/card";

export const CustomersError = () => {
	return (
		<div className="flex flex-1 flex-col gap-4 p-4">
			<Card className="h-[100%] flex items-center justify-center">
				<CardContent>
					<OctagonAlert className="ml-[auto] mr-[auto] block text-red-500 w-[60px] h-[60px] mb-[20px]" />
					<div className="text-center text-red-500">
						Error loading customers. Please try again.
					</div>
				</CardContent>
			</Card>
		</div>
	);
};
