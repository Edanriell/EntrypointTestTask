"use client";

import { FC, useState } from "react";

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
	// TODO
	// When creating an order we have small issue
	// We need make in select virtual lists for performance
	// Ui problem when cancelled order before confirming we must see nothing
	// Api problem !
	// When paying in different currency we get 500 instead we must throw an error
	// When we cancell order all funds must be refunded if they exist !
	const [showFilters, setShowFilters] = useState<boolean>(false);
	const [searchTerm, setSearchTerm] = useState<string>("");

	// Order filters
	const [orderNumberFilter, setOrderNumberFilter] = useState<string>("");
	const [statusFilter, setStatusFilter] = useState<string>("");
	const [minTotalAmount, setMinTotalAmount] = useState<string>("");
	const [maxTotalAmount, setMaxTotalAmount] = useState<string>("");
	const [trackingNumberFilter, setTrackingNumberFilter] = useState<string>("");
	const [minOutstandingAmount, setMinOutstandingAmount] = useState<string>("");
	const [maxOutstandingAmount, setMaxOutstandingAmount] = useState<string>("");

	// Date filters
	const [createdAfter, setCreatedAfter] = useState<string>("");
	const [createdBefore, setCreatedBefore] = useState<string>("");
	const [confirmedAfter, setConfirmedAfter] = useState<string>("");
	const [confirmedBefore, setConfirmedBefore] = useState<string>("");
	const [shippedAfter, setShippedAfter] = useState<string>("");
	const [shippedBefore, setShippedBefore] = useState<string>("");
	const [deliveredAfter, setDeliveredAfter] = useState<string>("");
	const [deliveredBefore, setDeliveredBefore] = useState<string>("");
	const [estimatedDeliveryAfter, setEstimatedDeliveryAfter] = useState<string>("");
	const [estimatedDeliveryBefore, setEstimatedDeliveryBefore] = useState<string>("");

	// Boolean filters
	const [hasPayment, setHasPayment] = useState<string>("");
	const [isFullyPaid, setIsFullyPaid] = useState<string>("");
	const [hasOutstandingBalance, setHasOutstandingBalance] = useState<string>("");

	// Product/Client filters
	const [productNameFilter, setProductNameFilter] = useState<string>("");
	const [clientNameFilter, setClientNameFilter] = useState<string>("");
	const [clientEmailFilter, setClientEmailFilter] = useState<string>("");
	const [paymentStatusFilter, setPaymentStatusFilter] = useState<string>("");

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

	const handleSearch = () => {
		setFilters({
			orderNumberFilter: searchTerm || undefined,
			statusFilter: statusFilter === "all" || !statusFilter ? undefined : statusFilter,
			minTotalAmount: minTotalAmount ? Number(minTotalAmount) : undefined,
			maxTotalAmount: maxTotalAmount ? Number(maxTotalAmount) : undefined,
			trackingNumberFilter: trackingNumberFilter || undefined,
			minOutstandingAmount: minOutstandingAmount ? Number(minOutstandingAmount) : undefined,
			maxOutstandingAmount: maxOutstandingAmount ? Number(maxOutstandingAmount) : undefined,
			createdAfter: createdAfter || undefined,
			createdBefore: createdBefore || undefined,
			confirmedAfter: confirmedAfter || undefined,
			confirmedBefore: confirmedBefore || undefined,
			shippedAfter: shippedAfter || undefined,
			shippedBefore: shippedBefore || undefined,
			deliveredAfter: deliveredAfter || undefined,
			deliveredBefore: deliveredBefore || undefined,
			estimatedDeliveryAfter: estimatedDeliveryAfter || undefined,
			estimatedDeliveryBefore: estimatedDeliveryBefore || undefined,
			hasPayment: hasPayment === "all" || !hasPayment ? undefined : hasPayment === "true",
			isFullyPaid: isFullyPaid === "all" || !isFullyPaid ? undefined : isFullyPaid === "true",
			hasOutstandingBalance:
				hasOutstandingBalance === "all" || !hasOutstandingBalance
					? undefined
					: hasOutstandingBalance === "true",
			productNameFilter: productNameFilter || undefined,
			clientNameFilter: clientNameFilter || undefined,
			clientEmailFilter: clientEmailFilter || undefined,
			paymentStatusFilter:
				paymentStatusFilter === "all" || !paymentStatusFilter
					? undefined
					: paymentStatusFilter
		});
	};

	const handleClearFilters = () => {
		setSearchTerm("");
		setOrderNumberFilter("");
		setStatusFilter("");
		setMinTotalAmount("");
		setMaxTotalAmount("");
		setTrackingNumberFilter("");
		setMinOutstandingAmount("");
		setMaxOutstandingAmount("");
		setCreatedAfter("");
		setCreatedBefore("");
		setConfirmedAfter("");
		setConfirmedBefore("");
		setShippedAfter("");
		setShippedBefore("");
		setDeliveredAfter("");
		setDeliveredBefore("");
		setEstimatedDeliveryAfter("");
		setEstimatedDeliveryBefore("");
		setHasPayment("");
		setIsFullyPaid("");
		setHasOutstandingBalance("");
		setProductNameFilter("");
		setClientNameFilter("");
		setClientEmailFilter("");
		setPaymentStatusFilter("");
		resetFilters();
	};

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
