import { Search as SearchIcon } from "lucide-react";
import { FC, useState } from "react";
import { AnimatePresence, motion } from "motion/react";

import { GetProductsQuery } from "@entities/products";

import { Input } from "@shared/ui/input";
import { Button } from "@shared/ui/button";

type SearchProps = {
	setFilters: (filters: Partial<GetProductsQuery>) => void;
};

export const Search: FC<SearchProps> = ({ setFilters }) => {
	const [searchTerm, setSearchTerm] = useState<string>("");

	const handleSearch = () => {
		setFilters({
			nameFilter: searchTerm || undefined
		});
	};

	const handleClearSearchTerm = () => {
		setSearchTerm("");
		setFilters({
			nameFilter: undefined
		});
	};

	return (
		<div className="flex flex-row gap-2 basis-[100%] mr-[20px] relative">
			<motion.div layout className="flex-1 relative z-2 w-full">
				<SearchIcon className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
				<Input
					placeholder="Search products by name..."
					value={searchTerm}
					onChange={(e) => setSearchTerm(e.target.value)}
					className="pl-9"
					onKeyDown={(e) => e.key === "Enter" && handleSearch()}
				/>
			</motion.div>
			<AnimatePresence mode={"popLayout"}>
				{searchTerm && (
					<motion.div
						layoutId="searchControls"
						className="flex flex-row rounded-[8px] overflow-hidden"
					>
						<motion.div key="searchControls" layoutId="search">
							<Button
								className="rounded-tr-none rounded-br-none"
								onClick={handleClearSearchTerm}
							>
								Clear
							</Button>
						</motion.div>
						<motion.div>
							<Button
								className="rounded-tl-none rounded-bl-none"
								onClick={handleSearch}
							>
								Search
							</Button>
						</motion.div>
					</motion.div>
				)}
				{!searchTerm && (
					<motion.div key="searchControls" layoutId="searchControls">
						<motion.div layoutId="search">
							<Button className="z-2" onClick={handleSearch}>
								Search
							</Button>
						</motion.div>
					</motion.div>
				)}
			</AnimatePresence>
		</div>
	);
};
