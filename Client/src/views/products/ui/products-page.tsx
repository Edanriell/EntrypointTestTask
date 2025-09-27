"use client";

import { FC } from "react";

import { CreateProduct } from "@features/products/create";

import { Pagination } from "@widgets/pagination";

import { SortOptions } from "@features/products/sort/ui/sort-options";
import { FiltersPanel } from "@features/products/filters";
import { Search } from "@features/products/search/ui/search";

import { useProductsList } from "../api";

import { ProductsError } from "./products-error";
import { ProductsList } from "./products-list";
import { ProductsNotFound } from "./products-not-found";
import { ProductsListSkeleton } from "./products-list-skeleton";

export const ProductsPage: FC = () => {
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

	if (error) {
		return <ProductsError />;
	}

	return (
		<div className="flex flex-1 flex-col gap-4 p-4">
			<div className="flex items-center justify-between">
				<div>
					<h1 className="text-2xl font-bold">Products</h1>
					<p className="text-muted-foreground">
						{totalCount ? `${totalCount} products total` : "Manage your products"}
					</p>
				</div>
			</div>
			<div className="flex flex-row gap-x-[10px] items-center mb-[20px]">
				<Search setFilters={setFilters} />
				<FiltersPanel setFilters={setFilters} resetFilters={resetFilters} />
				<SortOptions setSort={setSort} queryParams={queryParams} />
				<CreateProduct />
			</div>
			{isLoading && <ProductsListSkeleton />}
			{!isLoading && (
				<>
					{products.length === 0 ? (
						<ProductsNotFound />
					) : (
						<ProductsList products={products} />
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
	);
};
