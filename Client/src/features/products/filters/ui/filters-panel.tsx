import { FC, useState } from "react";
import { Filter, X } from "lucide-react";

import { GetProductsQuery, ProductStatus } from "@entities/products";

import { Popover, PopoverContent, PopoverTrigger } from "@shared/ui/popover";
import { Button } from "@shared/ui/button";
import { Label } from "@shared/ui/label";
import { Input } from "@shared/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@shared/ui/select";

type FiltersPanelProps = {
	setFilters: (filters: Partial<GetProductsQuery>) => void;
	resetFilters: () => void;
};

export const FiltersPanel: FC<FiltersPanelProps> = ({ setFilters, resetFilters }) => {
	const [descriptionFilter, setDescriptionFilter] = useState<string>("");
	const [minPrice, setMinPrice] = useState<string>("");
	const [maxPrice, setMaxPrice] = useState<string>("");
	const [minStock, setMinStock] = useState<string>("");
	const [maxStock, setMaxStock] = useState<string>("");
	const [statusFilter, setStatusFilter] = useState<string>("");
	const [hasStock, setHasStock] = useState<string>("");
	const [isReserved, setIsReserved] = useState<string>("");

	const handleSearch = () => {
		setFilters({
			// nameFilter: searchTerm || undefined,
			descriptionFilter: descriptionFilter || undefined,
			minPrice: minPrice ? Number(minPrice) : undefined,
			maxPrice: maxPrice ? Number(maxPrice) : undefined,
			minStock: minStock ? Number(minStock) : undefined,
			maxStock: maxStock ? Number(maxStock) : undefined,
			statusFilter: statusFilter === "all" || !statusFilter ? undefined : statusFilter,
			hasLowStock: hasStock === "low" ? true : undefined,
			hasStock:
				hasStock === "all" || hasStock === "low" || !hasStock
					? undefined
					: hasStock === "true",
			isReserved: isReserved === "all" || !isReserved ? undefined : isReserved === "true"
		});
	};

	const handleClearFilters = () => {
		// setSearchTerm("");
		setDescriptionFilter("");
		setMinPrice("");
		setMaxPrice("");
		setMinStock("");
		setMaxStock("");
		setStatusFilter("");
		setHasStock("");
		setIsReserved("");
		resetFilters();
	};

	return (
		<Popover>
			<PopoverTrigger>
				<div className="items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium transition-all disabled:pointer-events-none disabled:opacity-50 [&_svg]:pointer-events-none [&_svg:not([class*='size-'])]:size-4 shrink-0 [&_svg]:shrink-0 outline-none focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px] aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 aria-invalid:border-destructive bg-primary text-primary-foreground shadow-xs hover:bg-primary/90 h-9 px-4 py-2 has-[>svg]:px-3 flex flex-row">
					<Filter className="h-4 w-4 mr-2" />
					<span>Show Filters</span>
				</div>
			</PopoverTrigger>
			<PopoverContent className="w-full">
				<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
					<div>
						<Label className="pb-[10px]" htmlFor="description">
							Description
						</Label>
						<Input
							id="description"
							placeholder="Filter by description"
							value={descriptionFilter}
							onChange={(e) => setDescriptionFilter(e.target.value)}
						/>
					</div>
					<div>
						<Label className="pb-[10px]" htmlFor="minPrice">
							Min Price
						</Label>
						<Input
							id="minPrice"
							type="number"
							placeholder="0"
							value={minPrice}
							onChange={(e) => setMinPrice(e.target.value)}
						/>
					</div>
					<div>
						<Label className="pb-[10px]" htmlFor="maxPrice">
							Max Price
						</Label>
						<Input
							id="maxPrice"
							type="number"
							placeholder="1000"
							value={maxPrice}
							onChange={(e) => setMaxPrice(e.target.value)}
						/>
					</div>
					<div>
						<Label className="pb-[10px]" htmlFor="minStock">
							Min Stock
						</Label>
						<Input
							id="minStock"
							type="number"
							placeholder="0"
							value={minStock}
							onChange={(e) => setMinStock(e.target.value)}
						/>
					</div>
					<div>
						<Label className="pb-[10px]" htmlFor="maxStock">
							Max Stock
						</Label>
						<Input
							id="maxStock"
							type="number"
							placeholder="100"
							value={maxStock}
							onChange={(e) => setMaxStock(e.target.value)}
						/>
					</div>
					<div>
						<Label className="pb-[10px]" htmlFor="status">
							Status
						</Label>
						<Select
							value={statusFilter}
							onValueChange={(value) => setStatusFilter(value)}
						>
							<SelectTrigger className="w-full">
								<SelectValue placeholder="All statuses" />
							</SelectTrigger>
							<SelectContent>
								<SelectItem value="all">All statuses</SelectItem>
								<SelectItem value={ProductStatus.Available}>Available</SelectItem>
								<SelectItem value={ProductStatus.OutOfStock}>
									Out of Stock
								</SelectItem>
								<SelectItem value={ProductStatus.Discontinued}>
									Discontinued
								</SelectItem>
								<SelectItem value={ProductStatus.Deleted}>Deleted</SelectItem>
							</SelectContent>
						</Select>
					</div>
					<div>
						<Label className="pb-[10px]" htmlFor="hasStock">
							Has Stock
						</Label>
						<Select value={hasStock} onValueChange={(value) => setHasStock(value)}>
							<SelectTrigger className="w-full">
								<SelectValue placeholder="All products" />
							</SelectTrigger>
							<SelectContent>
								<SelectItem value="all">All products</SelectItem>
								<SelectItem value="true">In Stock</SelectItem>
								<SelectItem value="false">Out of Stock</SelectItem>
								<SelectItem value="low">Low Stock</SelectItem>
							</SelectContent>
						</Select>
					</div>
					<div>
						<Label className="pb-[10px]" htmlFor="isReserved">
							Reserved
						</Label>
						<Select value={isReserved} onValueChange={(value) => setIsReserved(value)}>
							<SelectTrigger className="w-full">
								<SelectValue placeholder="All products" />
							</SelectTrigger>
							<SelectContent>
								<SelectItem value="all">All products</SelectItem>
								<SelectItem value="true">Reserved</SelectItem>
								<SelectItem value="false">Not Reserved</SelectItem>
							</SelectContent>
						</Select>
					</div>
				</div>
				<div className="grid grid-cols-2 gap-[16px] mt-4 relative">
					<Button onClick={handleSearch}>Apply Filters</Button>
					<Button variant="outline" onClick={handleClearFilters}>
						<X className="h-4 w-4 mr-[-4px]" />
						Clear All
					</Button>
				</div>
			</PopoverContent>
		</Popover>
	);
};
