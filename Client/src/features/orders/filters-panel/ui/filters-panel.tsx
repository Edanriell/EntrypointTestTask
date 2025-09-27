import { FC, useState } from "react";
import { Filter, X } from "lucide-react";

import { GetOrdersQuery } from "@entities/orders";

import { Popover, PopoverContent, PopoverTrigger } from "@shared/ui/popover";
import { Button } from "@shared/ui/button";
import { Label } from "@shared/ui/label";
import { Input } from "@shared/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@shared/ui/select";

type FiltersPanelProps = {
	setFilters: (filters: Partial<GetOrdersQuery>) => void;
	resetFilters: () => void;
};

export const FiltersPanel: FC<FiltersPanelProps> = ({ setFilters, resetFilters }) => {
	// Order filters
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

	const handleSearch = () => {
		setFilters({
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

	return (
		<Popover>
			<PopoverTrigger>
				<div className="items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium transition-all disabled:pointer-events-none disabled:opacity-50 [&_svg]:pointer-events-none [&_svg:not([class*='size-'])]:size-4 shrink-0 [&_svg]:shrink-0 outline-none focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px] aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 aria-invalid:border-destructive bg-primary text-primary-foreground shadow-xs hover:bg-primary/90 h-9 px-4 py-2 has-[>svg]:px-3 flex flex-row">
					<Filter className="h-4 w-4 mr-2" />
					<span>Show Filters</span>
				</div>
			</PopoverTrigger>
			<PopoverContent className="w-full">
				<div className="mb-6">
					<h4 className="text-sm font-medium mb-3">Basic Filters</h4>
					<div className="grid grid-cols-1 md:grid-cols-4 gap-4">
						<div className="md:col-span-2">
							<Label className="pb-[10px]" htmlFor="status">
								Order Status
							</Label>
							<Select
								value={statusFilter}
								onValueChange={(value) => setStatusFilter(value)}
							>
								<SelectTrigger className="w-full">
									<SelectValue placeholder="All statuses" />
								</SelectTrigger>
								<SelectContent>
									<SelectItem value="all">All statuses</SelectItem>
									<SelectItem value="Pending">Pending</SelectItem>
									<SelectItem value="Processing">Processing</SelectItem>
									<SelectItem value="Shipped">Shipped</SelectItem>
									<SelectItem value="Delivered">Delivered</SelectItem>
									<SelectItem value="Completed">Completed</SelectItem>
									<SelectItem value="Cancelled">Cancelled</SelectItem>
									<SelectItem value="Returned">Returned</SelectItem>
									<SelectItem value="Refunded">Refunded</SelectItem>
								</SelectContent>
							</Select>
						</div>
						<div className="md:col-span-2">
							<Label className="pb-[10px]" htmlFor="trackingNumber">
								Tracking Number
							</Label>
							<Input
								id="trackingNumber"
								placeholder="Filter by tracking number"
								value={trackingNumberFilter}
								onChange={(e) => setTrackingNumberFilter(e.target.value)}
							/>
						</div>
					</div>
				</div>
				<div className="mb-6">
					<h4 className="text-sm font-medium mb-3">Amount Filters</h4>
					<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
						<div>
							<Label className="pb-[10px]" htmlFor="minTotalAmount">
								Min Total Amount
							</Label>
							<Input
								id="minTotalAmount"
								type="number"
								placeholder="0"
								value={minTotalAmount}
								onChange={(e) => setMinTotalAmount(e.target.value)}
							/>
						</div>
						<div>
							<Label className="pb-[10px]" htmlFor="maxTotalAmount">
								Max Total Amount
							</Label>
							<Input
								id="maxTotalAmount"
								type="number"
								placeholder="10000"
								value={maxTotalAmount}
								onChange={(e) => setMaxTotalAmount(e.target.value)}
							/>
						</div>
						<div>
							<Label className="pb-[10px]" htmlFor="minOutstandingAmount">
								Min Outstanding
							</Label>
							<Input
								id="minOutstandingAmount"
								type="number"
								placeholder="0"
								value={minOutstandingAmount}
								onChange={(e) => setMinOutstandingAmount(e.target.value)}
							/>
						</div>
						<div>
							<Label className="pb-[10px]" htmlFor="maxOutstandingAmount">
								Max Outstanding
							</Label>
							<Input
								id="maxOutstandingAmount"
								type="number"
								placeholder="10000"
								value={maxOutstandingAmount}
								onChange={(e) => setMaxOutstandingAmount(e.target.value)}
							/>
						</div>
					</div>
				</div>
				<div className="mb-6">
					<h4 className="text-sm font-medium mb-3">Date Filters</h4>
					<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
						<div>
							<Label className="pb-[10px]" htmlFor="createdAfter">
								Created After
							</Label>
							<Input
								id="createdAfter"
								type="date"
								value={createdAfter}
								onChange={(e) => setCreatedAfter(e.target.value)}
							/>
						</div>
						<div>
							<Label className="pb-[10px]" htmlFor="createdBefore">
								Created Before
							</Label>
							<Input
								id="createdBefore"
								type="date"
								value={createdBefore}
								onChange={(e) => setCreatedBefore(e.target.value)}
							/>
						</div>
						<div>
							<Label className="pb-[10px]" htmlFor="confirmedAfter">
								Confirmed After
							</Label>
							<Input
								id="confirmedAfter"
								type="date"
								value={confirmedAfter}
								onChange={(e) => setConfirmedAfter(e.target.value)}
							/>
						</div>
						<div>
							<Label className="pb-[10px]" htmlFor="confirmedBefore">
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
							<Label className="pb-[10px]" htmlFor="shippedAfter">
								Shipped After
							</Label>
							<Input
								id="shippedAfter"
								type="date"
								value={shippedAfter}
								onChange={(e) => setShippedAfter(e.target.value)}
							/>
						</div>
						<div>
							<Label className="pb-[10px]" htmlFor="shippedBefore">
								Shipped Before
							</Label>
							<Input
								id="shippedBefore"
								type="date"
								value={shippedBefore}
								onChange={(e) => setShippedBefore(e.target.value)}
							/>
						</div>
						<div>
							<Label className="pb-[10px]" htmlFor="deliveredAfter">
								Delivered After
							</Label>
							<Input
								id="deliveredAfter"
								type="date"
								value={deliveredAfter}
								onChange={(e) => setDeliveredAfter(e.target.value)}
							/>
						</div>
						<div>
							<Label className="pb-[10px]" htmlFor="deliveredBefore">
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
				<div className="mb-6">
					<h4 className="text-sm font-medium mb-3">Client & Product Filters</h4>
					<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
						<div>
							<Label className="pb-[10px]" htmlFor="clientName">
								Client Name
							</Label>
							<Input
								id="clientName"
								placeholder="Filter by client name"
								value={clientNameFilter}
								onChange={(e) => setClientNameFilter(e.target.value)}
							/>
						</div>
						<div>
							<Label className="pb-[10px]" htmlFor="clientEmail">
								Client Email
							</Label>
							<Input
								id="clientEmail"
								placeholder="Filter by client email"
								value={clientEmailFilter}
								onChange={(e) => setClientEmailFilter(e.target.value)}
							/>
						</div>
						<div>
							<Label className="pb-[10px]" htmlFor="productName">
								Product Name
							</Label>
							<Input
								id="productName"
								placeholder="Filter by product name"
								value={productNameFilter}
								onChange={(e) => setProductNameFilter(e.target.value)}
							/>
						</div>
					</div>
				</div>
				<div className="mb-6">
					<h4 className="text-sm font-medium mb-3">Payment Filters</h4>
					<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
						<div>
							<Label className="pb-[10px]" htmlFor="hasPayment">
								Has Payment
							</Label>
							<Select
								value={hasPayment}
								onValueChange={(value) => setHasPayment(value)}
							>
								<SelectTrigger className="w-full">
									<SelectValue placeholder="All orders" />
								</SelectTrigger>
								<SelectContent>
									<SelectItem value="all">All orders</SelectItem>
									<SelectItem value="true">With Payment</SelectItem>
									<SelectItem value="false">Without Payment</SelectItem>
								</SelectContent>
							</Select>
						</div>
						<div>
							<Label className="pb-[10px]" htmlFor="isFullyPaid">
								Fully Paid
							</Label>
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
									<SelectItem value="false">Not Fully Paid</SelectItem>
								</SelectContent>
							</Select>
						</div>
						<div>
							<Label className="pb-[10px]" htmlFor="hasOutstandingBalance">
								Outstanding Balance
							</Label>
							<Select
								value={hasOutstandingBalance}
								onValueChange={(value) => setHasOutstandingBalance(value)}
							>
								<SelectTrigger className="w-full">
									<SelectValue placeholder="All orders" />
								</SelectTrigger>
								<SelectContent>
									<SelectItem value="all">All orders</SelectItem>
									<SelectItem value="true">Has Outstanding</SelectItem>
									<SelectItem value="false">No Outstanding</SelectItem>
								</SelectContent>
							</Select>
						</div>
						<div>
							<Label className="pb-[10px]" htmlFor="paymentStatus">
								Payment Status
							</Label>
							<Select
								value={paymentStatusFilter}
								onValueChange={(value) => setPaymentStatusFilter(value)}
							>
								<SelectTrigger className="w-full">
									<SelectValue placeholder="All statuses" />
								</SelectTrigger>
								<SelectContent>
									<SelectItem value="all">All statuses</SelectItem>
									<SelectItem value="Paid">Paid</SelectItem>
									<SelectItem value="Pending">Pending</SelectItem>
									<SelectItem value="Failed">Failed</SelectItem>
									<SelectItem value="Refunded">Refunded</SelectItem>
								</SelectContent>
							</Select>
						</div>
					</div>
				</div>
				<div className="grid grid-cols-2 gap-[16px] mt-4 relative">
					<Button onClick={handleSearch}>Apply Filters</Button>
					<Button variant="outline" onClick={handleClearFilters}>
						<X className="h-4 w-4 mr-[-4px]" />
						Clear All
					</Button>
				</div>
			</PopoverContent>
		</Popover>
	);
};
