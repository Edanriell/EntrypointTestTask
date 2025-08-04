"use client";

import { FC, useState } from "react";
import { ChevronDown, ChevronLeft, ChevronRight, ChevronUp, Filter, Search, X } from "lucide-react";

import { AuthGuard } from "@features/authentication/general";

import { OrderRowCard } from "@entities/orders";

import { Button } from "@shared/ui/button";
import { Input } from "@shared/ui/input";
import { Label } from "@shared/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@shared/ui/select";
import { Card, CardContent, CardHeader, CardTitle } from "@shared/ui/card";
import { Separator } from "@shared/ui/separator";
import { Spinner } from "@shared/ui/spinner";

import { useOrdersList } from "../api";
import { EditOrder } from "@features/orders/edit";
import { DeleteOrder } from "@features/orders/delete";
import { StartProcessing } from "@features/orders/start-processing";
import { Ship } from "@features/orders/ship";
import { Return } from "@features/orders/return";
import { CancelOrder } from "@features/orders/cancel";
import { CreateOrder } from "@features/orders/create";
import { AddPayment } from "@features/payments/add";
import { ProcessPayment } from "@features/payments/process";
import { ConfirmOrder } from "@features/orders/confirm";
import { MarkReadyForShipment } from "@features/orders/mark-ready-for-shipment";
import { MarkOutForDelivery } from "@features/orders/mark-out-for-delivery";
import { MarkAsDelivered } from "@features/orders/mark-as-delivered";
import { Complete } from "@features/orders/complete";
import { Refund } from "@features/payments/refund";

export const OrdersPage: FC = () => {
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
								Error loading orders. Please try again.
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
						<h1 className="text-2xl font-bold">Orders</h1>
						<p className="text-muted-foreground">
							{totalCount ? `${totalCount} orders total` : "Manage your orders"}
						</p>
					</div>
					<CreateOrder />
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
									placeholder="Search orders by order number..."
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

								{/* Basic Filters */}
								<div className="mb-6">
									<h4 className="text-sm font-medium mb-3">Basic Filters</h4>
									<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
										<div>
											<Label htmlFor="orderNumber">Order Number</Label>
											<Input
												id="orderNumber"
												placeholder="Filter by order number"
												value={orderNumberFilter}
												onChange={(e) =>
													setOrderNumberFilter(e.target.value)
												}
											/>
										</div>
										<div>
											<Label htmlFor="status">Order Status</Label>
											<Select
												value={statusFilter}
												onValueChange={(value) => setStatusFilter(value)}
											>
												<SelectTrigger className="w-full">
													<SelectValue placeholder="All statuses" />
												</SelectTrigger>
												<SelectContent>
													<SelectItem value="all">
														All statuses
													</SelectItem>
													<SelectItem value="Pending">Pending</SelectItem>
													<SelectItem value="Processing">
														Processing
													</SelectItem>
													<SelectItem value="Shipped">Shipped</SelectItem>
													<SelectItem value="Delivered">
														Delivered
													</SelectItem>
													<SelectItem value="Completed">
														Completed
													</SelectItem>
													<SelectItem value="Cancelled">
														Cancelled
													</SelectItem>
													<SelectItem value="Returned">
														Returned
													</SelectItem>
													<SelectItem value="Refunded">
														Refunded
													</SelectItem>
												</SelectContent>
											</Select>
										</div>
										<div>
											<Label htmlFor="trackingNumber">Tracking Number</Label>
											<Input
												id="trackingNumber"
												placeholder="Filter by tracking number"
												value={trackingNumberFilter}
												onChange={(e) =>
													setTrackingNumberFilter(e.target.value)
												}
											/>
										</div>
									</div>
								</div>

								{/* Amount Filters */}
								<div className="mb-6">
									<h4 className="text-sm font-medium mb-3">Amount Filters</h4>
									<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
										<div>
											<Label htmlFor="minTotalAmount">Min Total Amount</Label>
											<Input
												id="minTotalAmount"
												type="number"
												placeholder="0"
												value={minTotalAmount}
												onChange={(e) => setMinTotalAmount(e.target.value)}
											/>
										</div>
										<div>
											<Label htmlFor="maxTotalAmount">Max Total Amount</Label>
											<Input
												id="maxTotalAmount"
												type="number"
												placeholder="10000"
												value={maxTotalAmount}
												onChange={(e) => setMaxTotalAmount(e.target.value)}
											/>
										</div>
										<div>
											<Label htmlFor="minOutstandingAmount">
												Min Outstanding
											</Label>
											<Input
												id="minOutstandingAmount"
												type="number"
												placeholder="0"
												value={minOutstandingAmount}
												onChange={(e) =>
													setMinOutstandingAmount(e.target.value)
												}
											/>
										</div>
										<div>
											<Label htmlFor="maxOutstandingAmount">
												Max Outstanding
											</Label>
											<Input
												id="maxOutstandingAmount"
												type="number"
												placeholder="10000"
												value={maxOutstandingAmount}
												onChange={(e) =>
													setMaxOutstandingAmount(e.target.value)
												}
											/>
										</div>
									</div>
								</div>

								{/* Date Filters */}
								<div className="mb-6">
									<h4 className="text-sm font-medium mb-3">Date Filters</h4>
									<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
										<div>
											<Label htmlFor="createdAfter">Created After</Label>
											<Input
												id="createdAfter"
												type="date"
												value={createdAfter}
												onChange={(e) => setCreatedAfter(e.target.value)}
											/>
										</div>
										<div>
											<Label htmlFor="createdBefore">Created Before</Label>
											<Input
												id="createdBefore"
												type="date"
												value={createdBefore}
												onChange={(e) => setCreatedBefore(e.target.value)}
											/>
										</div>
										<div>
											<Label htmlFor="confirmedAfter">Confirmed After</Label>
											<Input
												id="confirmedAfter"
												type="date"
												value={confirmedAfter}
												onChange={(e) => setConfirmedAfter(e.target.value)}
											/>
										</div>
										<div>
											<Label htmlFor="confirmedBefore">
												Confirmed Before
											</Label>
											<Input
												id="confirmedBefore"
												type="date"
												value={confirmedBefore}
												onChange={(e) => setConfirmedBefore(e.target.value)}
											/>
										</div>
										<div>
											<Label htmlFor="shippedAfter">Shipped After</Label>
											<Input
												id="shippedAfter"
												type="date"
												value={shippedAfter}
												onChange={(e) => setShippedAfter(e.target.value)}
											/>
										</div>
										<div>
											<Label htmlFor="shippedBefore">Shipped Before</Label>
											<Input
												id="shippedBefore"
												type="date"
												value={shippedBefore}
												onChange={(e) => setShippedBefore(e.target.value)}
											/>
										</div>
										<div>
											<Label htmlFor="deliveredAfter">Delivered After</Label>
											<Input
												id="deliveredAfter"
												type="date"
												value={deliveredAfter}
												onChange={(e) => setDeliveredAfter(e.target.value)}
											/>
										</div>
										<div>
											<Label htmlFor="deliveredBefore">
												Delivered Before
											</Label>
											<Input
												id="deliveredBefore"
												type="date"
												value={deliveredBefore}
												onChange={(e) => setDeliveredBefore(e.target.value)}
											/>
										</div>
									</div>
								</div>

								{/* Client & Product Filters */}
								<div className="mb-6">
									<h4 className="text-sm font-medium mb-3">
										Client & Product Filters
									</h4>
									<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
										<div>
											<Label htmlFor="clientName">Client Name</Label>
											<Input
												id="clientName"
												placeholder="Filter by client name"
												value={clientNameFilter}
												onChange={(e) =>
													setClientNameFilter(e.target.value)
												}
											/>
										</div>
										<div>
											<Label htmlFor="clientEmail">Client Email</Label>
											<Input
												id="clientEmail"
												placeholder="Filter by client email"
												value={clientEmailFilter}
												onChange={(e) =>
													setClientEmailFilter(e.target.value)
												}
											/>
										</div>
										<div>
											<Label htmlFor="productName">Product Name</Label>
											<Input
												id="productName"
												placeholder="Filter by product name"
												value={productNameFilter}
												onChange={(e) =>
													setProductNameFilter(e.target.value)
												}
											/>
										</div>
									</div>
								</div>

								{/* Boolean Filters */}
								<div className="mb-6">
									<h4 className="text-sm font-medium mb-3">Payment Filters</h4>
									<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
										<div>
											<Label htmlFor="hasPayment">Has Payment</Label>
											<Select
												value={hasPayment}
												onValueChange={(value) => setHasPayment(value)}
											>
												<SelectTrigger className="w-full">
													<SelectValue placeholder="All orders" />
												</SelectTrigger>
												<SelectContent>
													<SelectItem value="all">All orders</SelectItem>
													<SelectItem value="true">
														With Payment
													</SelectItem>
													<SelectItem value="false">
														Without Payment
													</SelectItem>
												</SelectContent>
											</Select>
										</div>
										<div>
											<Label htmlFor="isFullyPaid">Fully Paid</Label>
											<Select
												value={isFullyPaid}
												onValueChange={(value) => setIsFullyPaid(value)}
											>
												<SelectTrigger className="w-full">
													<SelectValue placeholder="All orders" />
												</SelectTrigger>
												<SelectContent>
													<SelectItem value="all">All orders</SelectItem>
													<SelectItem value="true">Fully Paid</SelectItem>
													<SelectItem value="false">
														Not Fully Paid
													</SelectItem>
												</SelectContent>
											</Select>
										</div>
										<div>
											<Label htmlFor="hasOutstandingBalance">
												Outstanding Balance
											</Label>
											<Select
												value={hasOutstandingBalance}
												onValueChange={(value) =>
													setHasOutstandingBalance(value)
												}
											>
												<SelectTrigger className="w-full">
													<SelectValue placeholder="All orders" />
												</SelectTrigger>
												<SelectContent>
													<SelectItem value="all">All orders</SelectItem>
													<SelectItem value="true">
														Has Outstanding
													</SelectItem>
													<SelectItem value="false">
														No Outstanding
													</SelectItem>
												</SelectContent>
											</Select>
										</div>
										<div>
											<Label htmlFor="paymentStatus">Payment Status</Label>
											<Select
												value={paymentStatusFilter}
												onValueChange={(value) =>
													setPaymentStatusFilter(value)
												}
											>
												<SelectTrigger className="w-full">
													<SelectValue placeholder="All statuses" />
												</SelectTrigger>
												<SelectContent>
													<SelectItem value="all">
														All statuses
													</SelectItem>
													<SelectItem value="Paid">Paid</SelectItem>
													<SelectItem value="Pending">Pending</SelectItem>
													<SelectItem value="Failed">Failed</SelectItem>
													<SelectItem value="Refunded">
														Refunded
													</SelectItem>
												</SelectContent>
											</Select>
										</div>
									</div>
								</div>

								<div className="flex gap-2">
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
								variant={
									queryParams.sortBy === "orderNumber" ? "default" : "outline"
								}
								size="sm"
								onClick={() => handleSortChange("orderNumber")}
								className="flex items-center gap-1"
							>
								Order Number
								{getSortIcon("orderNumber")}
							</Button>
							<Button
								variant={
									queryParams.sortBy === "totalAmount" ? "default" : "outline"
								}
								size="sm"
								onClick={() => handleSortChange("totalAmount")}
								className="flex items-center gap-1"
							>
								Total Amount
								{getSortIcon("totalAmount")}
							</Button>
							<Button
								variant={
									queryParams.sortBy === "outstandingAmount"
										? "default"
										: "outline"
								}
								size="sm"
								onClick={() => handleSortChange("outstandingAmount")}
								className="flex items-center gap-1"
							>
								Outstanding
								{getSortIcon("outstandingAmount")}
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
							<Button
								variant={
									queryParams.sortBy === "confirmedAt" ? "default" : "outline"
								}
								size="sm"
								onClick={() => handleSortChange("confirmedAt")}
								className="flex items-center gap-1"
							>
								Confirmed Date
								{getSortIcon("confirmedAt")}
							</Button>
							<Button
								variant={queryParams.sortBy === "shippedAt" ? "default" : "outline"}
								size="sm"
								onClick={() => handleSortChange("shippedAt")}
								className="flex items-center gap-1"
							>
								Shipped Date
								{getSortIcon("shippedAt")}
							</Button>
							<Button
								variant={
									queryParams.sortBy === "deliveredAt" ? "default" : "outline"
								}
								size="sm"
								onClick={() => handleSortChange("deliveredAt")}
								className="flex items-center gap-1"
							>
								Delivered Date
								{getSortIcon("deliveredAt")}
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
								<span className="ml-2">Loading orders...</span>
							</div>
						</CardContent>
					</Card>
				)}

				{/* Orders List */}
				{!isLoading && (
					<>
						{orders.length === 0 ? (
							<Card>
								<CardContent className="pt-6">
									<div className="text-center text-muted-foreground py-8">
										No orders found. Try adjusting your search or filters.
									</div>
								</CardContent>
							</Card>
						) : (
							<div className="space-y-4">
								{orders.map((order) => (
									<OrderRowCard key={order.id} order={order}>
										<OrderRowCard.ManagementActions>
											<EditOrder orderId={order.id} />
											<DeleteOrder
												orderId={order.id}
												orderNumber={order.orderNumber}
											/>
										</OrderRowCard.ManagementActions>
										<OrderRowCard.ContextActions>
											<ConfirmOrder
												orderId={order.id}
												orderNumber={order.orderNumber}
												isFullyPaid={order.paidAmount >= order.totalAmount}
											/>
											<MarkReadyForShipment
												orderId={order.id}
												orderNumber={order.orderNumber}
												orderStatus={order.status}
											/>
											<ProcessPayment
												orderId={order.id}
												orderNumber={order.orderNumber}
											/>
											<StartProcessing
												orderId={order.id}
												orderNumber={order.orderNumber}
												orderStatus={order.status}
											/>
											<Refund
												orderId={order.id}
												orderNumber={order.orderNumber}
												orderStatus={order.status}
												paidAmount={order.paidAmount}
												currency={order.currency}
											/>
											<MarkOutForDelivery
												orderId={order.id}
												orderNumber={order.orderNumber}
												orderStatus={order.status}
											/>
											<AddPayment
												orderId={order.id}
												orderNumber={order.orderNumber}
												outstandingAmount={order.outstandingAmount}
												orderCurrency={order.currency}
												orderStatus={order.status}
											/>
											<Return
												orderId={order.id}
												orderNumber={order.orderNumber}
												orderStatus={order.status}
											/>
											<Ship
												orderId={order.id}
												orderNumber={order.orderNumber}
												orderStatus={order.status}
											/>
											<Complete
												orderId={order.id}
												orderNumber={order.orderNumber}
												orderStatus={order.status}
											/>
											<CancelOrder
												orderId={order.id}
												orderNumber={order.orderNumber}
												orderStatus={order.status}
											/>
											<MarkAsDelivered
												orderId={order.id}
												orderNumber={order.orderNumber}
												orderStatus={order.status}
											/>
										</OrderRowCard.ContextActions>
									</OrderRowCard>
								))}
							</div>
						)}

						{/* Pagination */}
						<Card>
							<CardContent className="pt-6">
								<div className="flex items-center justify-between">
									<div className="text-sm text-muted-foreground">
										Showing {orders.length} orders
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
