import { ChangeEvent, FC } from "react";
import { Search as SearchIcon } from "lucide-react";

import { Input } from "@shared/ui/input";

type SearchProps = {
	onSearchInputChange: (event: ChangeEvent<HTMLInputElement>) => void;
};

export const Search: FC<SearchProps> = ({ onSearchInputChange }) => {
	return (
		<div className="relative ml-auto flex-1 md:grow-0">
			<SearchIcon className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
			<Input
				onChange={(event) => onSearchInputChange(event)}
				type="search"
				placeholder="Search..."
				className="w-full rounded-lg bg-background pl-8 md:w-[200px] lg:w-[320px]"
			/>
		</div>
	);
};
