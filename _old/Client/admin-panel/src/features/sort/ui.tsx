import { FC, Fragment } from "react";
import { ListFilter } from "lucide-react";

import {
	DropdownMenu,
	DropdownMenuCheckboxItem,
	DropdownMenuContent,
	DropdownMenuLabel,
	DropdownMenuSeparator,
	DropdownMenuTrigger
} from "@shared/ui/dropdown";
import { Button } from "@shared/ui/button";

type SortProps = {
	sortButtonName: string;
	sortMethodGroups: Array<{
		groupName: string;
		groupSortMethods: Array<{
			methodName: string;
			uniqueMethodName: string;
			sortColumn: string;
			sortOrder: string;
		}>;
	}>;
	currentlyActiveSortingMethod: {
		uniqueMethodName: string;
		sortColumn: string;
		sortOrder: string;
	} | null;
	onSortMethodClick: (sortMethod: {
		uniqueMethodName: string;
		sortColumn: string;
		sortOrder: string;
	}) => void;
};

export const Sort: FC<SortProps> = ({
	sortButtonName,
	sortMethodGroups,
	currentlyActiveSortingMethod,
	onSortMethodClick
}) => {
	return (
		<DropdownMenu>
			<DropdownMenuTrigger asChild>
				<Button variant="outline" size="sm" className="h-10 gap-1 text-sm">
					<ListFilter className="h-3.5 w-3.5" />
					<span className="sr-only sm:not-sr-only">{sortButtonName}</span>
				</Button>
			</DropdownMenuTrigger>
			<DropdownMenuContent align="end">
				{sortMethodGroups.map(({ groupName, groupSortMethods }, index) => (
					<Fragment key={index + "-" + groupName}>
						<DropdownMenuLabel>{groupName}</DropdownMenuLabel>
						<DropdownMenuSeparator />
						{groupSortMethods.map(
							({ uniqueMethodName, sortColumn, sortOrder, methodName }, index) => (
								<DropdownMenuCheckboxItem
									key={index + "-" + methodName + "-" + sortColumn + "-" + sortOrder}
									onClick={() =>
										onSortMethodClick({
											uniqueMethodName: uniqueMethodName,
											sortColumn: sortColumn,
											sortOrder: sortOrder
										})
									}
									checked={currentlyActiveSortingMethod?.uniqueMethodName === uniqueMethodName}
								>
									{methodName}
								</DropdownMenuCheckboxItem>
							)
						)}
					</Fragment>
				))}
			</DropdownMenuContent>
		</DropdownMenu>
	);
};
