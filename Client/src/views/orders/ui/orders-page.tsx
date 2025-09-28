"use client";

import { FC } from "react";

import { Pagination } from "@widgets/pagination";

import { CreateOrder } from "@features/orders/create";
import { FiltersPanel } from "@features/orders/filters-panel";
import { Search } from "@features/orders/search";
import { SortOptions } from "@features/orders/sort-options";

import { useOrdersList } from "../api";

import { OrdersListSkeleton } from "./orders-list-skeleton";
import { OrdersNotFound } from "./orders-not-found";
import { OrdersList } from "./orders-list";
import { OrdersError } from "./orders-error";

export const OrdersPage: FC = () => {
	const {
		orders,
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
	} = useOrdersList({
		initialPageSize: 10,
		initialSortBy: "createdAt",
		initialSortDirection: "desc"
	});

	if (error) {
		return <OrdersError />;
	}

	return (
		<div className="flex flex-1 flex-col gap-4 p-4">
			<div className="flex items-center justify-between">
				<div>
					<h1 className="text-2xl font-bold">Orders</h1>
					<p className="text-muted-foreground">
						{totalCount ? `${totalCount} orders total` : "Manage your orders"}
					</p>
				</div>
			</div>
			<div className="flex flex-row gap-x-[10px] items-center mb-[20px]">
				<Search setFilters={setFilters} />
				<FiltersPanel setFilters={setFilters} resetFilters={resetFilters} />
				<SortOptions setSort={setSort} queryParams={queryParams} />
				<CreateOrder />
			</div>
			{isLoading && <OrdersListSkeleton />}
			{!isLoading && (
				<>
					{orders.length === 0 ? <OrdersNotFound /> : <OrdersList orders={orders} />}
					<Pagination
						entity={orders}
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
