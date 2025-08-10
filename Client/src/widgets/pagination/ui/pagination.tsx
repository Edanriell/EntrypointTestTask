import { ChevronLeft, ChevronRight } from "lucide-react";

import { Card, CardContent } from "@shared/ui/card";
import { Button } from "@shared/ui/button";

type PaginationProps<T> = {
	entity: Array<T>;
	totalCount?: number;
	hasPreviousPage: boolean;
	hasNextPage: boolean;
	goToPreviousPage: () => void;
	goToNextPage: () => void;
};

export const Pagination = <T,>({
	entity,
	totalCount,
	goToPreviousPage,
	hasPreviousPage,
	goToNextPage,
	hasNextPage
}: PaginationProps<T>) => {
	return (
		<Card>
			<CardContent className="pt-6">
				<div className="flex items-center justify-between">
					<div className="text-sm text-muted-foreground">
						Showing {entity.length} products
						{totalCount && ` of ${totalCount} total`}
					</div>
					<div className="flex items-center gap-2">
						<Button
							variant="outline"
							size="sm"
							onClick={goToPreviousPage}
							disabled={!hasPreviousPage}
						>
							<ChevronLeft className="h-4 w-4 mr-1" />
							Previous
						</Button>
						<Button
							variant="outline"
							size="sm"
							onClick={goToNextPage}
							disabled={!hasNextPage}
						>
							Next
							<ChevronRight className="h-4 w-4 ml-1" />
						</Button>
					</div>
				</div>
			</CardContent>
		</Card>
	);
};
