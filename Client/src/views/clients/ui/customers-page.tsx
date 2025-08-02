"use client";

import { FC, useState } from "react";
import { ChevronDown, ChevronLeft, ChevronRight, ChevronUp, Filter, Search, X } from "lucide-react";

import { CreateCustomer } from "@features/users/create";
import { EditUser } from "@features/users/edit";
import { DeleteUser } from "@features/users/delete";
import { AuthGuard } from "@features/authentication/general";

import { Customer, CustomerRowCard } from "@entities/users";

import { Button } from "@shared/ui/button";
import { Input } from "@shared/ui/input";
import { Label } from "@shared/ui/label";
import { Card, CardContent, CardHeader, CardTitle } from "@shared/ui/card";
import { Separator } from "@shared/ui/separator";
import { Spinner } from "@shared/ui/spinner";

import { useCustomersList } from "../api";

export const ClientsPage: FC = () => {
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
		initialSortBy: "CreatedOnUtc",
		initialSortDirection: "DESC"
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

	if (error) {
		return (
			<AuthGuard>
				<div className="flex flex-1 flex-col gap-4 p-4">
					<Card>
						<CardContent className="pt-6">
							<div className="text-center text-red-500">
								Error loading customers. Please try again.
							</div>
						</CardContent>
					</Card>
				</div>
			</AuthGuard>
		);
	}

	return (
		<AuthGuard>
			<div className="flex flex-1 flex-col gap-4 p-4">
				{/* Header */}
				<div className="flex items-center justify-between">
					<div>
						<h1 className="text-2xl font-bold">Customers</h1>
						<p className="text-muted-foreground">
							{totalCount ? `${totalCount} customers total` : "Manage your customers"}
						</p>
					</div>
					<CreateCustomer />
				</div>

				{/* Search and Filters */}
				<Card>
					<CardHeader>
						<div className="flex items-center justify-between">
							<CardTitle className="text-lg">Search & Filters</CardTitle>
							<Button
								variant="outline"
								size="sm"
								onClick={() => setShowFilters(!showFilters)}
							>
								<Filter className="h-4 w-4 mr-2" />
								{showFilters ? "Hide Filters" : "Show Filters"}
							</Button>
						</div>
					</CardHeader>
					<CardContent>
						{/* Search Bar */}
						<div className="flex gap-2 mb-4">
							<div className="flex-1 relative">
								<Search className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
								<Input
									placeholder="Search by name or email..."
									value={searchTerm}
									onChange={(e) => setSearchTerm(e.target.value)}
									className="pl-9"
									onKeyDown={(e) => e.key === "Enter" && handleSearch()}
								/>
							</div>
							<Button onClick={handleSearch}>Search</Button>
						</div>

						{/* Advanced Filters */}
						{showFilters && (
							<>
								<Separator className="mb-4" />
								<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
									<div>
										<Label htmlFor="country">Country</Label>
										<Input
											id="country"
											placeholder="Filter by country"
											value={countryFilter}
											onChange={(e) => setCountryFilter(e.target.value)}
										/>
									</div>
									<div>
										<Label htmlFor="city">City</Label>
										<Input
											id="city"
											placeholder="Filter by city"
											value={cityFilter}
											onChange={(e) => setCityFilter(e.target.value)}
										/>
									</div>
									<div>
										<Label htmlFor="minSpent">Min Total Spent</Label>
										<Input
											id="minSpent"
											type="number"
											placeholder="0"
											value={minSpent}
											onChange={(e) => setMinSpent(e.target.value)}
										/>
									</div>
									<div>
										<Label htmlFor="maxSpent">Max Total Spent</Label>
										<Input
											id="maxSpent"
											type="number"
											placeholder="1000"
											value={maxSpent}
											onChange={(e) => setMaxSpent(e.target.value)}
										/>
									</div>
									<div>
										<Label htmlFor="minOrders">Min Total Orders</Label>
										<Input
											id="minOrders"
											type="number"
											placeholder="0"
											value={minOrders}
											onChange={(e) => setMinOrders(e.target.value)}
										/>
									</div>
									<div>
										<Label htmlFor="maxOrders">Max Total Orders</Label>
										<Input
											id="maxOrders"
											type="number"
											placeholder="100"
											value={maxOrders}
											onChange={(e) => setMaxOrders(e.target.value)}
										/>
									</div>
								</div>
								<div className="flex gap-2 mt-4">
									<Button onClick={handleSearch}>Apply Filters</Button>
									<Button variant="outline" onClick={handleClearFilters}>
										<X className="h-4 w-4 mr-2" />
										Clear All
									</Button>
								</div>
							</>
						)}
					</CardContent>
				</Card>

				{/* Sorting Controls */}
				<Card>
					<CardHeader>
						<CardTitle className="text-lg">Sort By</CardTitle>
					</CardHeader>
					<CardContent>
						<div className="flex flex-wrap gap-2">
							<Button
								variant={queryParams.sortBy === "fullName" ? "default" : "outline"}
								size="sm"
								onClick={() => handleSortChange("fullName")}
								className="flex items-center gap-1"
							>
								Name
								{getSortIcon("fullName")}
							</Button>
							<Button
								variant={
									queryParams.sortBy === "totalSpent" ? "default" : "outline"
								}
								size="sm"
								onClick={() => handleSortChange("totalSpent")}
								className="flex items-center gap-1"
							>
								Total Spent
								{getSortIcon("totalSpent")}
							</Button>
							<Button
								variant={
									queryParams.sortBy === "totalOrders" ? "default" : "outline"
								}
								size="sm"
								onClick={() => handleSortChange("totalOrders")}
								className="flex items-center gap-1"
							>
								Total Orders
								{getSortIcon("totalOrders")}
							</Button>
							<Button
								variant={
									queryParams.sortBy === "createdOnUtc" ? "default" : "outline"
								}
								size="sm"
								onClick={() => handleSortChange("createdOnUtc")}
								className="flex items-center gap-1"
							>
								Created Date
								{getSortIcon("createdOnUtc")}
							</Button>
						</div>
					</CardContent>
				</Card>

				{/* Loading State */}
				{isLoading && (
					<Card>
						<CardContent className="pt-6">
							<div className="flex items-center justify-center py-8">
								<Spinner />
								<span className="ml-2">Loading customers...</span>
							</div>
						</CardContent>
					</Card>
				)}

				{/* Customers List */}
				{!isLoading && (
					<>
						{customers.length === 0 ? (
							<Card>
								<CardContent className="pt-6">
									<div className="text-center text-muted-foreground py-8">
										No customers found. Try adjusting your search or filters.
									</div>
								</CardContent>
							</Card>
						) : (
							<div className="space-y-4">
								{customers.map((customer: Customer) => (
									<CustomerRowCard key={customer.id} customer={customer}>
										<CustomerRowCard.ManagementActions>
											<EditUser userId={customer.id} />
											<DeleteUser
												userId={customer.id}
												userName={customer.fullName}
											/>
										</CustomerRowCard.ManagementActions>
									</CustomerRowCard>
								))}
							</div>
						)}

						{/* Pagination */}
						<Card>
							<CardContent className="pt-6">
								<div className="flex items-center justify-between">
									<div className="text-sm text-muted-foreground">
										Showing {customers.length} customers
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
					</>
				)}
			</div>
		</AuthGuard>
	);
};
