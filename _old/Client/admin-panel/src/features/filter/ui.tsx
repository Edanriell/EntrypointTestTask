import { FC, Fragment } from "react";

import {
	DropdownMenu,
	DropdownMenuCheckboxItem,
	DropdownMenuContent,
	DropdownMenuLabel,
	DropdownMenuSeparator,
	DropdownMenuTrigger
} from "@shared/ui/dropdown";
import { Button } from "@shared/ui/button";
import { toCamelCase } from "@shared/lib";

type FilterProps = {
	filterButtonName: string;
	filterGroups: Array<{
		groupName: string;
		groupFilters: Array<{
			filterName: string;
		}>;
	}>;
	currentlyActiveFilter: string | null;
	onFilterClick: (filterName: string) => void;
};

export const Filter: FC<FilterProps> = ({
	filterButtonName,
	filterGroups,
	currentlyActiveFilter,
	onFilterClick
}) => {
	return (
		<DropdownMenu>
			<DropdownMenuTrigger asChild>
				<Button variant="outline">{filterButtonName}</Button>
			</DropdownMenuTrigger>
			<DropdownMenuContent className="w-56">
				{filterGroups.map(({ groupName, groupFilters }, index) => (
					<Fragment key={index + "-" + groupName}>
						<DropdownMenuLabel>{groupName}</DropdownMenuLabel>
						<DropdownMenuSeparator />
						{groupFilters.map(({ filterName }, index) => (
							<DropdownMenuCheckboxItem
								key={index + "-" + filterName}
								onClick={() => onFilterClick(toCamelCase(filterName))}
								checked={currentlyActiveFilter === toCamelCase(filterName) ? true : false}
							>
								{filterName}
							</DropdownMenuCheckboxItem>
						))}
					</Fragment>
				))}
			</DropdownMenuContent>
		</DropdownMenu>
	);
};
