import { FC, useState } from "react";
import { Filter, X } from "lucide-react";

import { GetCustomersQuery } from "@entities/users";

import { Popover, PopoverContent, PopoverTrigger } from "@shared/ui/popover";
import { Button } from "@shared/ui/button";
import { Label } from "@shared/ui/label";
import { Input } from "@shared/ui/input";

type FiltersPanelProps = {
	setFilters: (filters: Partial<GetCustomersQuery>) => void;
	resetFilters: () => void;
};

export const FiltersPanel: FC<FiltersPanelProps> = ({ setFilters, resetFilters }) => {
	const [emailFilter, setEmailFilter] = useState<string>("");
	const [countryFilter, setCountryFilter] = useState<string>("");
	const [cityFilter, setCityFilter] = useState<string>("");
	const [minSpent, setMinSpent] = useState<string>("");
	const [maxSpent, setMaxSpent] = useState<string>("");
	const [minOrders, setMinOrders] = useState<string>("");
	const [maxOrders, setMaxOrders] = useState<string>("");

	const handleSearch = () => {
		setFilters({
			emailFilter: emailFilter || undefined,
			countryFilter: countryFilter || undefined,
			cityFilter: cityFilter || undefined,
			minTotalSpent: minSpent ? Number(minSpent) : undefined,
			maxTotalSpent: maxSpent ? Number(maxSpent) : undefined,
			minTotalOrders: minOrders ? Number(minOrders) : undefined,
			maxTotalOrders: maxOrders ? Number(maxOrders) : undefined
		});
	};

	const handleClearFilters = () => {
		setCountryFilter("");
		setCityFilter("");
		setMinSpent("");
		setMaxSpent("");
		setMinOrders("");
		setMaxOrders("");
		setEmailFilter("");
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
				<div className="grid grid-cols-1 md:grid-cols-2 gap-4">
					<div>
						<Label className="pb-[10px]" htmlFor="country">
							Country
						</Label>
						<Input
							id="country"
							placeholder="Filter by country"
							value={countryFilter}
							onChange={(e) => setCountryFilter(e.target.value)}
						/>
					</div>
					<div>
						<Label className="pb-[10px]" htmlFor="city">
							City
						</Label>
						<Input
							id="city"
							placeholder="Filter by city"
							value={cityFilter}
							onChange={(e) => setCityFilter(e.target.value)}
						/>
					</div>
					<div>
						<Label className="pb-[10px]" htmlFor="minSpent">
							Min Total Spent
						</Label>
						<Input
							id="minSpent"
							type="number"
							placeholder="0"
							value={minSpent}
							onChange={(e) => setMinSpent(e.target.value)}
						/>
					</div>
					<div>
						<Label className="pb-[10px]" htmlFor="maxSpent">
							Max Total Spent
						</Label>
						<Input
							id="maxSpent"
							type="number"
							placeholder="1000"
							value={maxSpent}
							onChange={(e) => setMaxSpent(e.target.value)}
						/>
					</div>
					<div>
						<Label className="pb-[10px]" htmlFor="minOrders">
							Min Total Orders
						</Label>
						<Input
							id="minOrders"
							type="number"
							placeholder="0"
							value={minOrders}
							onChange={(e) => setMinOrders(e.target.value)}
						/>
					</div>
					<div>
						<Label className="pb-[10px]" htmlFor="maxOrders">
							Max Total Orders
						</Label>
						<Input
							id="maxOrders"
							type="number"
							placeholder="100"
							value={maxOrders}
							onChange={(e) => setMaxOrders(e.target.value)}
						/>
					</div>
					<div className="md:col-span-2">
						<Label className="pb-[10px]" htmlFor="email">
							Email address
						</Label>
						<Input
							id="email"
							type="text"
							value={emailFilter}
							onChange={(e) => setEmailFilter(e.target.value)}
						/>
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
