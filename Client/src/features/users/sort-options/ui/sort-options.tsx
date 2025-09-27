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
						variant={queryParams.sortBy === "firstName" ? "default" : "outline"}
						size="sm"
						onClick={() => handleSortChange("firstName")}
						className="flex items-center gap-1"
					>
						Name
						{getSortIcon("firstName")}
					</Button>
					<Button
						variant={queryParams.sortBy === "lastName" ? "default" : "outline"}
						size="sm"
						onClick={() => handleSortChange("lastName")}
						className="flex items-center gap-1"
					>
						Surname
						{getSortIcon("lastName")}
					</Button>
					<Button
						variant={queryParams.sortBy === "totalSpent" ? "default" : "outline"}
						size="sm"
						onClick={() => handleSortChange("totalSpent")}
						className="flex items-center gap-1"
					>
						Total Spent
						{getSortIcon("totalSpent")}
					</Button>
					<Button
						variant={queryParams.sortBy === "totalOrders" ? "default" : "outline"}
						size="sm"
						onClick={() => handleSortChange("totalOrders")}
						className="flex items-center gap-1"
					>
						Total Orders
						{getSortIcon("totalOrders")}
					</Button>
					<Button
						variant={queryParams.sortBy === "createdOnUtc" ? "default" : "outline"}
						size="sm"
						onClick={() => handleSortChange("createdOnUtc")}
						className="flex items-center gap-1 md:col-span-2"
					>
						Created Date
						{getSortIcon("createdOnUtc")}
					</Button>
				</div>
			</PopoverContent>
		</Popover>
	);
};
