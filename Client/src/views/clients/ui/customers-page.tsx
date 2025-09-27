"use client";

import { FC, useState } from "react";

import { CreateCustomer } from "@features/users/create";

import { useCustomersList } from "../api";
import { Pagination } from "@widgets/pagination";
import { CustomersList } from "@views/clients/ui/customers-list";
import { CustomersNotFound } from "@views/clients/ui/customers-not-found";
import { CustomersListSkeleton } from "@views/clients/ui/customers-list-skeleton";
import { CustomersError } from "@views/clients/ui/customers-error";
import { SortOptions } from "@features/users/sort-options";
import { FiltersPanel } from "@features/users/filters-panel";
import { Search } from "@features/users/search";

export const CustomersPage: FC = () => {
	const [showFilters, setShowFilters] = useState<boolean>(false);
	const [searchTerm, setSearchTerm] = useState<string>("");
	const [countryFilter, setCountryFilter] = useState<string>("");
	const [cityFilter, setCityFilter] = useState<string>("");
	const [minSpent, setMinSpent] = useState<string>("");
	const [maxSpent, setMaxSpent] = useState<string>("");
	const [minOrders, setMinOrders] = useState<string>("");
	const [maxOrders, setMaxOrders] = useState<string>("");

	const {
		customers,
		isLoading,
		error,
		hasNextPage,
		hasPreviousPage,
		goToNextPage,
		goToPreviousPage,
		setSort,
		setFilters,
		resetFilters,
		queryParams,
		totalCount
	} = useCustomersList({
		initialPageSize: 10,
		initialSortBy: "createdAt",
		initialSortDirection: "desc"
	});

	const handleSearch = () => {
		setFilters({
			nameFilter: searchTerm || undefined,
			emailFilter: searchTerm || undefined,
			countryFilter: countryFilter || undefined,
			cityFilter: cityFilter || undefined,
			minTotalSpent: minSpent ? Number(minSpent) : undefined,
			maxTotalSpent: maxSpent ? Number(maxSpent) : undefined,
			minTotalOrders: minOrders ? Number(minOrders) : undefined,
			maxTotalOrders: maxOrders ? Number(maxOrders) : undefined
		});
	};

	const handleClearFilters = () => {
		setSearchTerm("");
		setCountryFilter("");
		setCityFilter("");
		setMinSpent("");
		setMaxSpent("");
		setMinOrders("");
		setMaxOrders("");
		resetFilters();
	};

	if (error) {
		return <CustomersError />;
	}

	return (
		<div className="flex flex-1 flex-col gap-4 p-4">
			<div className="flex items-center justify-between">
				<div>
					<h1 className="text-2xl font-bold">Customers</h1>
					<p className="text-muted-foreground">
						{totalCount ? `${totalCount} customers total` : "Manage your customers"}
					</p>
				</div>
			</div>
			<div className="flex flex-row gap-x-[10px] items-center mb-[20px]">
				<Search setFilters={setFilters} />
				<FiltersPanel setFilters={setFilters} resetFilters={resetFilters} />
				<SortOptions setSort={setSort} queryParams={queryParams} />
				<CreateCustomer />
			</div>
			{isLoading && <CustomersListSkeleton />}
			{!isLoading && (
				<>
					{customers.length === 0 ? (
						<CustomersNotFound />
					) : (
						<CustomersList customers={customers} />
					)}
					<Pagination
						entity={customers}
						totalCount={totalCount}
						hasNextPage={hasNextPage}
						hasPreviousPage={hasPreviousPage}
						goToNextPage={goToNextPage}
						goToPreviousPage={goToPreviousPage}
					/>
				</>
			)}
		</div>
	);
};
