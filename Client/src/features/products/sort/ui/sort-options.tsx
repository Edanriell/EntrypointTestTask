import { FC } from "react";
import { ChevronDown, ChevronUp } from "lucide-react";

import { GetProductsQuery } from "@entities/products";

import { Button } from "@shared/ui/button";
import { Popover, PopoverContent, PopoverTrigger } from "@shared/ui/popover";

type SortOptionsProps = {
	queryParams: GetProductsQuery;
	setSort: (field: string, direction: "asc" | "desc") => void;
};

export const SortOptions: FC<SortOptionsProps> = ({ queryParams, setSort }) => {
	const handleSortChange = (field: string) => {
		const isCurrentField = queryParams.sortBy === field;
		const newDirection = isCurrentField && queryParams.sortDirection === "asc" ? "desc" : "asc";
		setSort(field, newDirection);
	};

	const getSortIcon = (field: string) => {
		if (queryParams.sortBy === field) {
			return queryParams.sortDirection === "asc" ? (
				<ChevronUp className="h-4 w-4" />
			) : (
				<ChevronDown className="h-4 w-4" />
			);
		}
		return null;
	};

	return (
		<Popover>
			<PopoverTrigger>
				<div className="items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium transition-all disabled:pointer-events-none disabled:opacity-50 [&_svg]:pointer-events-none [&_svg:not([class*='size-'])]:size-4 shrink-0 [&_svg]:shrink-0 outline-none focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px] aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 aria-invalid:border-destructive bg-primary text-primary-foreground shadow-xs hover:bg-primary/90 h-9 px-4 py-2 has-[>svg]:px-3 flex flex-row">
					<div className="w-[24px]">
						<ChevronUp className="mb-[-4px]" />
						<ChevronDown className="mt-[-4px]" />
					</div>
					<span>Sort By</span>
				</div>
			</PopoverTrigger>
			<PopoverContent className="w-full">
				<div className="grid grid-cols-1 md:grid-cols-2 gap-4">
					<Button
						variant={queryParams.sortBy === "name" ? "default" : "outline"}
						size="sm"
						onClick={() => handleSortChange("name")}
						className="flex items-center gap-1"
					>
						Name
						{getSortIcon("name")}
					</Button>
					<Button
						variant={queryParams.sortBy === "price" ? "default" : "outline"}
						size="sm"
						onClick={() => handleSortChange("price")}
						className="flex items-center gap-1"
					>
						Price
						{getSortIcon("price")}
					</Button>
					<Button
						variant={queryParams.sortBy === "totalStock" ? "default" : "outline"}
						size="sm"
						onClick={() => handleSortChange("totalStock")}
						className="flex items-center gap-1"
					>
						Stock
						{getSortIcon("totalStock")}
					</Button>
					<Button
						variant={queryParams.sortBy === "createdAt" ? "default" : "outline"}
						size="sm"
						onClick={() => handleSortChange("createdAt")}
						className="flex items-center gap-1"
					>
						Created Date
						{getSortIcon("createdAt")}
					</Button>
				</div>
			</PopoverContent>
		</Popover>
	);
};
