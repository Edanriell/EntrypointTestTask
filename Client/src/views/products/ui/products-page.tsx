"use client";

import { FC, useState } from "react";
import { ChevronDown, ChevronUp, Filter, Search, X } from "lucide-react";

import { CreateProduct } from "@features/products/create";
import { EditProduct } from "@features/products/edit";
import { DeleteProduct } from "@features/products/delete";
import { UpdateProductPrice } from "@features/products/update-price";
import { UpdateProductStock } from "@features/products/update-stock";
import { ProductReservedStock } from "@features/products/reserved-stock";
import { AuthGuard } from "@features/authentication/general";

import { ProductRowCard } from "@entities/products";
import { ProductStatus } from "@entities/products/model";

import { Button } from "@shared/ui/button";
import { Input } from "@shared/ui/input";
import { Label } from "@shared/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@shared/ui/select";
import { Card, CardContent, CardHeader, CardTitle } from "@shared/ui/card";
import { Separator } from "@shared/ui/separator";
import { Spinner } from "@shared/ui/spinner";

import { useProductsList } from "../api";
import { DiscountProduct } from "@features/products/discount-product/ui";
import { RestoreProduct } from "@features/products/restore-product";
import { Pagination } from "@widgets/pagination";

export const ProductsPage: FC = () => {
	// TODO
	// Add new filter has stock low stock !
	// Edit product stock works like 337 will add if we type -20 will remove
	// Create three separate inputs
	// For add stock, remove stock by default 0
	// And current stock input must be inactive ! Api can stay the same
	// Decompose Filters and search and sort by separate widgets
	// Sorting by Stock is working incorrectly !
	const [showFilters, setShowFilters] = useState<boolean>(false);
	const [searchTerm, setSearchTerm] = useState<string>("");
	const [descriptionFilter, setDescriptionFilter] = useState<string>("");
	const [minPrice, setMinPrice] = useState<string>("");
	const [maxPrice, setMaxPrice] = useState<string>("");
	const [minStock, setMinStock] = useState<string>("");
	const [maxStock, setMaxStock] = useState<string>("");
	const [statusFilter, setStatusFilter] = useState<string>("");
	const [hasStock, setHasStock] = useState<string>("");
	const [isReserved, setIsReserved] = useState<string>("");

	const {
		products,
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
	} = useProductsList({
		initialPageSize: 10,
		initialSortBy: "createdAt",
		initialSortDirection: "desc"
	});

	const handleSearch = () => {
		setFilters({
			nameFilter: searchTerm || undefined,
			descriptionFilter: descriptionFilter || undefined,
			minPrice: minPrice ? Number(minPrice) : undefined,
			maxPrice: maxPrice ? Number(maxPrice) : undefined,
			minStock: minStock ? Number(minStock) : undefined,
			maxStock: maxStock ? Number(maxStock) : undefined,
			statusFilter: statusFilter === "all" || !statusFilter ? undefined : statusFilter,
			hasStock: hasStock === "all" || !hasStock ? undefined : hasStock === "true",
			isReserved: isReserved === "all" || !isReserved ? undefined : isReserved === "true"
		});
	};

	const handleClearFilters = () => {
		setSearchTerm("");
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
								Error loading products. Please try again.
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
						<h1 className="text-2xl font-bold">Products</h1>
						<p className="text-muted-foreground">
							{totalCount ? `${totalCount} products total` : "Manage your products"}
						</p>
					</div>
					<CreateProduct />
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
									placeholder="Search products by name..."
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
										<Label htmlFor="description">Description</Label>
										<Input
											id="description"
											placeholder="Filter by description"
											value={descriptionFilter}
											onChange={(e) => setDescriptionFilter(e.target.value)}
										/>
									</div>
									<div>
										<Label htmlFor="minPrice">Min Price</Label>
										<Input
											id="minPrice"
											type="number"
											placeholder="0"
											value={minPrice}
											onChange={(e) => setMinPrice(e.target.value)}
										/>
									</div>
									<div>
										<Label htmlFor="maxPrice">Max Price</Label>
										<Input
											id="maxPrice"
											type="number"
											placeholder="1000"
											value={maxPrice}
											onChange={(e) => setMaxPrice(e.target.value)}
										/>
									</div>
									<div>
										<Label htmlFor="minStock">Min Stock</Label>
										<Input
											id="minStock"
											type="number"
											placeholder="0"
											value={minStock}
											onChange={(e) => setMinStock(e.target.value)}
										/>
									</div>
									<div>
										<Label htmlFor="maxStock">Max Stock</Label>
										<Input
											id="maxStock"
											type="number"
											placeholder="100"
											value={maxStock}
											onChange={(e) => setMaxStock(e.target.value)}
										/>
									</div>
									<div>
										<Label htmlFor="status">Status</Label>
										<Select
											value={statusFilter}
											onValueChange={(value) => setStatusFilter(value)}
										>
											<SelectTrigger className="w-full">
												<SelectValue placeholder="All statuses" />
											</SelectTrigger>
											<SelectContent>
												<SelectItem value="all">All statuses</SelectItem>
												<SelectItem value={ProductStatus.Available}>
													Available
												</SelectItem>
												<SelectItem value={ProductStatus.OutOfStock}>
													Out of Stock
												</SelectItem>
												<SelectItem value={ProductStatus.Discontinued}>
													Discontinued
												</SelectItem>
												<SelectItem value={ProductStatus.Deleted}>
													Deleted
												</SelectItem>
											</SelectContent>
										</Select>
									</div>
									<div>
										<Label htmlFor="hasStock">Has Stock</Label>
										<Select
											value={hasStock}
											onValueChange={(value) => setHasStock(value)}
										>
											<SelectTrigger className="w-full">
												<SelectValue placeholder="All products" />
											</SelectTrigger>
											<SelectContent>
												<SelectItem value="all">All products</SelectItem>
												<SelectItem value="true">In Stock</SelectItem>
												<SelectItem value="false">Out of Stock</SelectItem>
											</SelectContent>
										</Select>
									</div>
									<div>
										<Label htmlFor="isReserved">Reserved</Label>
										<Select
											value={isReserved}
											onValueChange={(value) => setIsReserved(value)}
										>
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
								variant={
									queryParams.sortBy === "totalStock" ? "default" : "outline"
								}
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
					</CardContent>
				</Card>

				{/* Loading State */}
				{isLoading && (
					<Card>
						<CardContent className="pt-6">
							<div className="flex items-center justify-center py-8">
								<Spinner />
								<span className="ml-2">Loading products...</span>
							</div>
						</CardContent>
					</Card>
				)}

				{/* Products List */}
				{!isLoading && (
					<>
						{products.length === 0 ? (
							<Card>
								<CardContent className="pt-6">
									<div className="text-center text-muted-foreground py-8">
										No products found. Try adjusting your search or filters.
									</div>
								</CardContent>
							</Card>
						) : (
							<div className="space-y-4">
								{products.map((product) => (
									<ProductRowCard key={product.id} product={product}>
										{product.status !== ProductStatus.Deleted && (
											<ProductRowCard.ManagementActions>
												<EditProduct productId={product.id} />
												<DeleteProduct
													productId={product.id}
													productName={product.name}
												/>
											</ProductRowCard.ManagementActions>
										)}
										<ProductRowCard.QuickActions>
											{product.status !== ProductStatus.Deleted && (
												<>
													<UpdateProductPrice productId={product.id} />
													<UpdateProductStock productId={product.id} />
													<ProductReservedStock productId={product.id} />
													<DiscountProduct productId={product.id} />
												</>
											)}
											{product.status === ProductStatus.Deleted && (
												<RestoreProduct productId={product.id} />
											)}
										</ProductRowCard.QuickActions>
									</ProductRowCard>
								))}
							</div>
						)}

						<Pagination
							entity={products}
							totalCount={totalCount}
							hasNextPage={hasNextPage}
							hasPreviousPage={hasPreviousPage}
							goToNextPage={goToNextPage}
							goToPreviousPage={goToPreviousPage}
						/>
					</>
				)}
			</div>
		</AuthGuard>
	);
};
