"use client";

import { ChangeEvent, FC, useEffect, useState } from "react";
import { ListFilter, Search as SearchIcon } from "lucide-react";
import { useInView } from "react-intersection-observer";
import { useInfiniteQuery } from "@tanstack/react-query";
import { useSession } from "next-auth/react";

import { OrderStatus } from "@entities/orders/model";
import { getPaginatedSortedAndFilteredOrders } from "@entities/orders/api";

import { OrdersTable } from "@widgets/oders-table";

import { Tabs, TabsContent, TabsList, TabsTrigger } from "@shared/ui/tabs";
import {
	DropdownMenu,
	DropdownMenuCheckboxItem,
	DropdownMenuContent,
	DropdownMenuLabel,
	DropdownMenuSeparator,
	DropdownMenuTrigger
} from "@shared/ui/dropdown";
import { Button } from "@shared/ui/button";
import { Input } from "@shared/ui/input";
import { useDebounce } from "@shared/lib/hooks/useDebounce";
import { sanitizeInput } from "@shared/lib";

export type sortingMethod = {
	activeSortingMethodName: string;
	sortColumn: string;
	sortOrder: string;
};

export const OrderTabs: FC = () => {
	const [activeTab, setActiveTab] = useState<string>("created");
	const [activeSortingMethod, setActiveSortingMethod] = useState<sortingMethod | null>(null);
	const [activeSearchFilter, setActiveSearchFilter] = useState<string | null>("customerName");
	const [isDataRefetched, setIsDataRefetched] = useState<boolean>(false);
	const [searchTerm, setSearchTerm] = useState<string | null>(null);
	let debouncedSearchTerm = useDebounce(searchTerm, 600);

	const { data: session } = useSession();
	const userId = (session as any)?.user?.id;
	const accessToken = (session as any)?.accessToken;

	const {
		data: allOrders,
		error: allOrdersError,
		isPending: isAllOrdersPending,
		isError: isAllOrdersError,
		fetchNextPage: fetchNextAllOrdersPage,
		hasNextPage: hasAllOrdersNextPage,
		isFetchingNextPage: isAllOrdersFetchingNextPage,
		status: allOrdersStatus,
		refetch: refetchAllOrders
	} = useInfiniteQuery({
		queryKey: ["getPaginatedSortedAndFilteredAllOrders", userId],
		queryFn: ({ pageParam }) =>
			getPaginatedSortedAndFilteredOrders({
				accessToken,
				pageParam,
				pageSize: 6,
				sortColumn: activeSortingMethod?.sortColumn,
				sortOrder: activeSortingMethod?.sortOrder,
				customerName: activeSearchFilter === "customerName" ? debouncedSearchTerm : null,
				customerSurname: activeSearchFilter === "customerSurname" ? debouncedSearchTerm : null,
				customerEmail: activeSearchFilter === "customerEmail" ? debouncedSearchTerm : null,
				productName: activeSearchFilter === "productName" ? debouncedSearchTerm : null,
				orderShipAddress: activeSearchFilter === "orderShipAddress" ? debouncedSearchTerm : null
			}),
		initialPageParam: 0,
		getNextPageParam: (lastPage, allPages) => (lastPage.length === 6 ? allPages.length : undefined),
		enabled: !!userId && !!accessToken && activeTab === "created"
	});

	const {
		data: pendingForPaymentOrders,
		error: pendingForPaymentOrdersError,
		isPending: isPendingForPaymentOrdersPending,
		isError: isPendingForPaymentOrdersError,
		fetchNextPage: fetchNextPendingForPaymentOrdersPage,
		hasNextPage: hasPendingForPaymentOrdersNextPage,
		isFetchingNextPage: isPendingForPaymentOrdersFetchingNextPage,
		status: pendingForPaymentOrdersStatus,
		refetch: refetchPendingForPaymentOrders
	} = useInfiniteQuery({
		queryKey: ["getPaginatedSortedAndFilteredPendingForPaymentOrders", userId],
		queryFn: ({ pageParam }) =>
			getPaginatedSortedAndFilteredOrders({
				accessToken,
				pageParam,
				pageSize: 6,
				orderStatus: OrderStatus.PendingForPayment,
				sortColumn: activeSortingMethod?.sortColumn,
				sortOrder: activeSortingMethod?.sortOrder,
				customerName: activeSearchFilter === "customerName" ? debouncedSearchTerm : null,
				customerSurname: activeSearchFilter === "customerSurname" ? debouncedSearchTerm : null,
				customerEmail: activeSearchFilter === "customerEmail" ? debouncedSearchTerm : null,
				productName: activeSearchFilter === "productName" ? debouncedSearchTerm : null,
				orderShipAddress: activeSearchFilter === "orderShipAddress" ? debouncedSearchTerm : null
			}),
		initialPageParam: 0,
		getNextPageParam: (lastPage, allPages) => (lastPage.length === 6 ? allPages.length : undefined),
		enabled: !!userId && !!accessToken && activeTab === "pendingforpayment"
	});

	const {
		data: paidOrders,
		error: paidOrdersError,
		isPending: isPaidOrdersPending,
		isError: isPaidOrdersError,
		fetchNextPage: fetchNextPaidOrdersPage,
		hasNextPage: hasPaidOrdersNextPage,
		isFetchingNextPage: isPaidOrdersFetchingNextPage,
		status: paidOrdersStatus,
		refetch: refetchPaidOrders
	} = useInfiniteQuery({
		queryKey: ["getPaginatedSortedAndFilteredPaidOrders", userId],
		queryFn: ({ pageParam }) =>
			getPaginatedSortedAndFilteredOrders({
				accessToken,
				pageParam,
				pageSize: 6,
				orderStatus: OrderStatus.Paid,
				sortColumn: activeSortingMethod?.sortColumn,
				sortOrder: activeSortingMethod?.sortOrder,
				customerName: activeSearchFilter === "customerName" ? debouncedSearchTerm : null,
				customerSurname: activeSearchFilter === "customerSurname" ? debouncedSearchTerm : null,
				customerEmail: activeSearchFilter === "customerEmail" ? debouncedSearchTerm : null,
				productName: activeSearchFilter === "productName" ? debouncedSearchTerm : null,
				orderShipAddress: activeSearchFilter === "orderShipAddress" ? debouncedSearchTerm : null
			}),
		initialPageParam: 0,
		getNextPageParam: (lastPage, allPages) => (lastPage.length === 6 ? allPages.length : undefined),
		enabled: !!userId && !!accessToken && activeTab === "paid"
	});

	const {
		data: inTransitOrders,
		error: inTransitOrdersError,
		isPending: isInTransitOrdersPending,
		isError: isInTransitOrdersError,
		fetchNextPage: fetchNextInTransitOrdersPage,
		hasNextPage: hasInTransitOrdersNextPage,
		isFetchingNextPage: isInTransitOrdersFetchingNextPage,
		status: inTransitOrdersStatus,
		refetch: refetchIntransitOrders
	} = useInfiniteQuery({
		queryKey: ["getPaginatedSortedAndFilteredInTransitOrders", userId],
		queryFn: ({ pageParam }) =>
			getPaginatedSortedAndFilteredOrders({
				accessToken,
				pageParam,
				pageSize: 6,
				orderStatus: OrderStatus.InTransit,
				sortColumn: activeSortingMethod?.sortColumn,
				sortOrder: activeSortingMethod?.sortOrder,
				customerName: activeSearchFilter === "customerName" ? debouncedSearchTerm : null,
				customerSurname: activeSearchFilter === "customerSurname" ? debouncedSearchTerm : null,
				customerEmail: activeSearchFilter === "customerEmail" ? debouncedSearchTerm : null,
				productName: activeSearchFilter === "productName" ? debouncedSearchTerm : null,
				orderShipAddress: activeSearchFilter === "orderShipAddress" ? debouncedSearchTerm : null
			}),
		initialPageParam: 0,
		getNextPageParam: (lastPage, allPages) => (lastPage.length === 6 ? allPages.length : undefined),
		enabled: !!userId && !!accessToken && activeTab === "intransit"
	});

	const {
		data: deliveredOrders,
		error: deliveredOrdersError,
		isPending: isDeliveredOrdersPending,
		isError: isDeliveredOrdersError,
		fetchNextPage: fetchNextDeliveredOrdersPage,
		hasNextPage: hasDeliveredOrdersNextPage,
		isFetchingNextPage: isDeliveredOrdersFetchingNextPage,
		status: deliveredOrdersStatus,
		refetch: refetchDeliveredOrders
	} = useInfiniteQuery({
		queryKey: ["getPaginatedSortedAndFilteredDeliveredOrders", userId],
		queryFn: ({ pageParam }) =>
			getPaginatedSortedAndFilteredOrders({
				accessToken,
				pageParam,
				pageSize: 6,
				orderStatus: OrderStatus.Delivered,
				sortColumn: activeSortingMethod?.sortColumn,
				sortOrder: activeSortingMethod?.sortOrder,
				customerName: activeSearchFilter === "customerName" ? debouncedSearchTerm : null,
				customerSurname: activeSearchFilter === "customerSurname" ? debouncedSearchTerm : null,
				customerEmail: activeSearchFilter === "customerEmail" ? debouncedSearchTerm : null,
				productName: activeSearchFilter === "productName" ? debouncedSearchTerm : null,
				orderShipAddress: activeSearchFilter === "orderShipAddress" ? debouncedSearchTerm : null
			}),
		initialPageParam: 0,
		getNextPageParam: (lastPage, allPages) => (lastPage.length === 6 ? allPages.length : undefined),
		enabled: !!userId && !!accessToken && activeTab === "delivered"
	});

	const {
		data: cancelledOrders,
		error: cancelledOrdersError,
		isPending: isCancelledOrdersPending,
		isError: isCancelledOrdersError,
		fetchNextPage: fetchNextCancelledOrdersPage,
		hasNextPage: hasCancelledOrdersNextPage,
		isFetchingNextPage: isCancelledOrdersFetchingNextPage,
		status: cancelledOrdersStatus,
		refetch: refetchCancelledOrders
	} = useInfiniteQuery({
		queryKey: ["getPaginatedSortedAndFilteredCancelledOrders", userId],
		queryFn: ({ pageParam }) =>
			getPaginatedSortedAndFilteredOrders({
				accessToken,
				pageParam,
				pageSize: 6,
				orderStatus: OrderStatus.Cancelled,
				sortColumn: activeSortingMethod?.sortColumn,
				sortOrder: activeSortingMethod?.sortOrder,
				customerName: activeSearchFilter === "customerName" ? debouncedSearchTerm : null,
				customerSurname: activeSearchFilter === "customerSurname" ? debouncedSearchTerm : null,
				customerEmail: activeSearchFilter === "customerEmail" ? debouncedSearchTerm : null,
				productName: activeSearchFilter === "productName" ? debouncedSearchTerm : null,
				orderShipAddress: activeSearchFilter === "orderShipAddress" ? debouncedSearchTerm : null
			}),
		initialPageParam: 0,
		getNextPageParam: (lastPage, allPages) => (lastPage.length === 6 ? allPages.length : undefined),
		enabled: !!userId && !!accessToken && activeTab === "cancelled"
	});

	const { ref: allOrdersTableLastRowRef, inView: isLastAllOrdersTableRowInView } = useInView();
	const {
		ref: pendingForPaymentOrdersTableLastRowRef,
		inView: isLastPendingForPaymentOrdersTableRowInView
	} = useInView();
	const { ref: paidOrdersTableLastRowRef, inView: isLastPaidOrdersTableRowInView } = useInView();
	const { ref: inTransitOrdersTableLastRowRef, inView: isLastInTransitOrdersTableRowInView } =
		useInView();
	const { ref: deliveredOrdersTableLastRowRef, inView: isLastDeliveredOrdersTableRowInView } =
		useInView();
	const { ref: cancelledOrdersTableLastRowRef, inView: isLastCancelledOrdersTableRowInView } =
		useInView();

	useEffect(() => {
		if (isLastAllOrdersTableRowInView && hasAllOrdersNextPage) {
			fetchNextAllOrdersPage();
		}
	}, [isLastAllOrdersTableRowInView, hasAllOrdersNextPage]);

	useEffect(() => {
		if (activeSortingMethod && activeTab === "created" && !isDataRefetched) {
			refetchAllOrders();
			setIsDataRefetched(true);
		}
	}, [activeSortingMethod]);

	useEffect(() => {
		if (
			debouncedSearchTerm &&
			debouncedSearchTerm.trim().length > 0 &&
			activeTab === "created" &&
			!isDataRefetched
		) {
			setIsDataRefetched(true);
			refetchAllOrders();
		} else if (debouncedSearchTerm === "" && activeTab === "created" && !isDataRefetched) {
			setIsDataRefetched(true);
			debouncedSearchTerm = null;
			refetchAllOrders();
		}
	}, [debouncedSearchTerm]);

	useEffect(() => {
		if (isLastPendingForPaymentOrdersTableRowInView && hasPendingForPaymentOrdersNextPage) {
			fetchNextPendingForPaymentOrdersPage();
		}
	}, [isLastPendingForPaymentOrdersTableRowInView, hasPendingForPaymentOrdersNextPage]);

	useEffect(() => {
		if (activeSortingMethod && activeTab === "pendingforpayment" && !isDataRefetched) {
			refetchPendingForPaymentOrders();
			setIsDataRefetched(true);
		}
	}, [activeSortingMethod]);

	useEffect(() => {
		if (
			debouncedSearchTerm &&
			debouncedSearchTerm.trim().length > 0 &&
			activeTab === "pendingforpayment" &&
			!isDataRefetched
		) {
			setIsDataRefetched(true);
			refetchPendingForPaymentOrders();
		} else if (
			debouncedSearchTerm === "" &&
			activeTab === "pendingforpayment" &&
			!isDataRefetched
		) {
			setIsDataRefetched(true);
			debouncedSearchTerm = null;
			refetchPendingForPaymentOrders();
		}
	}, [debouncedSearchTerm]);

	useEffect(() => {
		if (hasPaidOrdersNextPage && isLastPaidOrdersTableRowInView) {
			fetchNextPaidOrdersPage();
		}
	}, [hasPaidOrdersNextPage, isLastPaidOrdersTableRowInView]);

	useEffect(() => {
		if (activeSortingMethod && activeTab === "paid" && !isDataRefetched) {
			refetchPaidOrders();
			setIsDataRefetched(true);
		}
	}, [activeSortingMethod]);

	useEffect(() => {
		if (
			debouncedSearchTerm &&
			debouncedSearchTerm.trim().length > 0 &&
			activeTab === "paid" &&
			!isDataRefetched
		) {
			setIsDataRefetched(true);
			refetchPaidOrders();
		} else if (debouncedSearchTerm === "" && activeTab === "paid" && !isDataRefetched) {
			setIsDataRefetched(true);
			debouncedSearchTerm = null;
			refetchPaidOrders();
		}
	}, [debouncedSearchTerm]);

	useEffect(() => {
		if (activeSortingMethod && activeTab === "intransit" && !isDataRefetched) {
			refetchIntransitOrders();
			setIsDataRefetched(true);
		} else if (hasInTransitOrdersNextPage && isLastInTransitOrdersTableRowInView) {
			fetchNextInTransitOrdersPage();
		}
	}, [
		isLastInTransitOrdersTableRowInView,
		fetchNextInTransitOrdersPage,
		hasInTransitOrdersNextPage,
		activeSortingMethod,
		activeTab,
		isDataRefetched
	]);

	useEffect(() => {
		if (hasDeliveredOrdersNextPage && isLastDeliveredOrdersTableRowInView) {
			fetchNextDeliveredOrdersPage();
		}
	}, [hasDeliveredOrdersNextPage, isLastDeliveredOrdersTableRowInView]);

	useEffect(() => {
		if (activeSortingMethod && activeTab === "delivered" && !isDataRefetched) {
			refetchDeliveredOrders();
			setIsDataRefetched(true);
		}
	}, [activeSortingMethod]);

	useEffect(() => {
		if (
			debouncedSearchTerm &&
			debouncedSearchTerm.trim().length > 0 &&
			activeTab === "delivered" &&
			!isDataRefetched
		) {
			setIsDataRefetched(true);
			refetchDeliveredOrders();
		} else if (debouncedSearchTerm === "" && activeTab === "delivered" && !isDataRefetched) {
			setIsDataRefetched(true);
			debouncedSearchTerm = null;
			refetchDeliveredOrders();
		}
	}, [debouncedSearchTerm]);

	useEffect(() => {
		if (hasCancelledOrdersNextPage && isLastCancelledOrdersTableRowInView) {
			fetchNextCancelledOrdersPage();
		}
	}, [hasCancelledOrdersNextPage, isLastCancelledOrdersTableRowInView]);

	useEffect(() => {
		if (activeSortingMethod && activeTab === "cancelled" && !isDataRefetched) {
			refetchCancelledOrders();
			setIsDataRefetched(true);
		}
	}, [activeSortingMethod]);

	useEffect(() => {
		if (
			debouncedSearchTerm &&
			debouncedSearchTerm.trim().length > 0 &&
			activeTab === "cancelled" &&
			!isDataRefetched
		) {
			setIsDataRefetched(true);
			refetchCancelledOrders();
		} else if (debouncedSearchTerm === "" && activeTab === "cancelled" && !isDataRefetched) {
			setIsDataRefetched(true);
			debouncedSearchTerm = null;
			refetchCancelledOrders();
		}
	}, [debouncedSearchTerm]);

	const handleTabClick = (tabName: string) => {
		setActiveTab(tabName);
	};

	const handleSortClick = (sortType: any) => {
		setIsDataRefetched(false);
		setActiveSortingMethod({
			activeSortingMethodName: sortType.name,
			sortColumn: sortType.sortColumn,
			sortOrder: sortType.sortOrder
		});
	};

	const handleSearchInputChange = (event: any) => {
		setIsDataRefetched(false);
		setSearchTerm(sanitizeInput(event.target.value));
	};

	// TODO
	// Search can  be moved to separate component
	// We can do  hoc in search like withSanitize
	// Also filter logic can be moved out
	// Tab triggers and tabs must be separate component
	// useQueryInfinity  separate hook
	// TODO

	type TabTriggerProps = {
		tabName: string;
		classes: string;
		onTabClick: (tabName: string) => void;
	};

	const TabTrigger: FC<TabTriggerProps> = ({ tabName, classes, onTabClick }) => {
		return (
			<TabsTrigger onClick={() => onTabClick(tabName)} className={classes} value={tabName}>
				{tabName}
			</TabsTrigger>
		);
	};

	type TabList = Array<{
		tabName: string;
		tabDescription: string;
		tabTrigger: {
			classes?: string;
			onTabClick: (tabName: string) => void;
		};
		currentActiveTab: string;
		currentActiveSortingMethod: {
			activeSortingMethodName: string;
			sortColumn: string;
			sortOrder: string;
		} | null;
		currentActiveSearchFilter: string | null;
		searchTerm: string | null;
		setIsDataRefetched: (isRefetched: boolean) => void;
		isDataRefetched: boolean;
	}>;

	const tabList: TabList = [
		{
			tabName: "created",
			tabDescription: "Complete list of all orders.",
			tabTrigger: {
				classes: "block",
				onTabClick: handleTabClick
			},
			currentActiveTab: activeTab,
			currentActiveSortingMethod: activeSortingMethod,
			currentActiveSearchFilter: activeSearchFilter,
			searchTerm: searchTerm,
			setIsDataRefetched: setIsDataRefetched,
			isDataRefetched: isDataRefetched
		},
		{
			tabName: "pendingforpayment",
			tabDescription: "List of all orders pending for payment.",
			tabTrigger: {
				classes: "hidden min-[1200px]:block",
				onTabClick: handleTabClick
			},
			currentActiveTab: activeTab,
			currentActiveSortingMethod: activeSortingMethod,
			currentActiveSearchFilter: activeSearchFilter,
			searchTerm: searchTerm,
			setIsDataRefetched: setIsDataRefetched,
			isDataRefetched: isDataRefetched
		},
		{
			tabName: "paid",
			tabDescription: "Complete list of paid orders.",
			tabTrigger: {
				classes: "hidden min-[520px]:block",
				onTabClick: handleTabClick
			},
			currentActiveTab: activeTab,
			currentActiveSortingMethod: activeSortingMethod,
			currentActiveSearchFilter: activeSearchFilter,
			searchTerm: searchTerm,
			setIsDataRefetched: setIsDataRefetched,
			isDataRefetched: isDataRefetched
		},
		{
			tabName: "intransit",
			tabDescription: "List of all orders in transit.",
			tabTrigger: {
				classes: "hidden min-[920px]:block",
				onTabClick: handleTabClick
			},
			currentActiveTab: activeTab,
			currentActiveSortingMethod: activeSortingMethod,
			currentActiveSearchFilter: activeSearchFilter,
			searchTerm: searchTerm,
			setIsDataRefetched: setIsDataRefetched,
			isDataRefetched: isDataRefetched
		},
		{
			tabName: "delivered",
			tabDescription: "List of all delivered orders.",
			tabTrigger: {
				classes: "hidden min-[768px]:block",
				onTabClick: handleTabClick
			},
			currentActiveTab: activeTab,
			currentActiveSortingMethod: activeSortingMethod,
			currentActiveSearchFilter: activeSearchFilter,
			searchTerm: searchTerm,
			setIsDataRefetched: setIsDataRefetched,
			isDataRefetched: isDataRefetched
		},
		{
			tabName: "cancelled",
			tabDescription: "List of all cancelled orders.",
			tabTrigger: {
				classes: "hidden min-[600px]:block",
				onTabClick: handleTabClick
			},
			currentActiveTab: activeTab,
			currentActiveSortingMethod: activeSortingMethod,
			currentActiveSearchFilter: activeSearchFilter,
			searchTerm: searchTerm,
			setIsDataRefetched: setIsDataRefetched,
			isDataRefetched: isDataRefetched
		}
	];

	type TabContentProps = {
		tabName: string;
		tabDescription: string;
		currentActiveTab: string;
		currentActiveSortingMethod: {
			methodName: string;
			sortColumn: string;
			sortOrder: string;
		};
		currentActiveSearchFilter: string;
		searchTerm: string | null;
		setIsDataRefetched: (isRefetched: boolean) => void;
		isDataRefetched: boolean;
	};

	const TabContent: FC<TabContentProps> = ({
		tabName,
		currentActiveTab,
		currentActiveSortingMethod,
		searchTerm,
		tabDescription,
		currentActiveSearchFilter,
		setIsDataRefetched,
		isDataRefetched
	}) => {
		const { data: session } = useSession();
		const userId = (session as any)?.user?.id;
		const accessToken = (session as any)?.accessToken;

		const {
			data: orders,
			error: ordersError,
			isPending: isOrdersPending,
			isError: isOrdersError,
			fetchNextPage: fetchNextOrdersPage,
			hasNextPage: hasOrdersNextPage,
			isFetchingNextPage: isOrdersFetchingNextPage,
			status: ordersStatus,
			refetch: refetchOrders
		} = useInfiniteQuery({
			queryKey: ["getOrders", userId],
			queryFn: ({ pageParam }) =>
				getPaginatedSortedAndFilteredOrders({
					accessToken,
					pageParam,
					pageSize: 6,
					sortColumn: currentActiveSortingMethod?.sortColumn,
					sortOrder: currentActiveSortingMethod?.sortOrder,
					customerName: currentActiveSearchFilter === "customerName" ? searchTerm : null,
					customerSurname: currentActiveSearchFilter === "customerSurname" ? searchTerm : null,
					customerEmail: currentActiveSearchFilter === "customerEmail" ? searchTerm : null,
					productName: currentActiveSearchFilter === "productName" ? searchTerm : null,
					orderShipAddress: currentActiveSearchFilter === "orderShipAddress" ? searchTerm : null
				}),
			initialPageParam: 0,
			getNextPageParam: (lastPage, allPages) =>
				lastPage.length === 6 ? allPages.length : undefined,
			enabled: !!userId && !!accessToken && currentActiveTab === tabName
		});

		const { ref: ordersTableLastRowRef, inView: isLastOrdersTableRowInView } = useInView();

		useEffect(() => {
			if (isLastOrdersTableRowInView && hasOrdersNextPage) {
				fetchNextOrdersPage();
			}
		}, [isLastOrdersTableRowInView, hasOrdersNextPage]);

		useEffect(() => {
			if (
				currentActiveSortingMethod.methodName &&
				currentActiveTab === tabName &&
				!isDataRefetched
			) {
				refetchOrders();
				setIsDataRefetched(true);
			}
		}, [currentActiveSortingMethod.methodName]);

		useEffect(() => {
			if (searchTerm && searchTerm.length > 0 && currentActiveTab === tabName && !isDataRefetched) {
				setIsDataRefetched(true);
				refetchOrders();
			} else if (searchTerm === "" && currentActiveTab === tabName && !isDataRefetched) {
				setIsDataRefetched(true);
				searchTerm = null;
				refetchOrders();
			}
		}, [searchTerm]);

		return (
			<TabsContent value={tabName}>
				<OrdersTable
					ordersTableLastRowRef={ordersTableLastRowRef}
					data={orders}
					error={ordersError}
					isPending={isOrdersPending}
					isError={isOrdersError}
					isFetchingNextPage={isOrdersFetchingNextPage}
					hasNextPage={hasOrdersNextPage}
					status={ordersStatus}
					description={tabDescription}
				/>
			</TabsContent>
		);
	};

	type SearchProps = {
		onInputChange: (event: ChangeEvent<HTMLInputElement>) => void;
	};

	const Search: FC<SearchProps> = ({ onInputChange }) => {
		return (
			<div className="relative ml-auto flex-1 md:grow-0">
				<SearchIcon className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
				<Input
					onChange={(event) => onInputChange(event)}
					type="search"
					placeholder="Search..."
					className="w-full rounded-lg bg-background pl-8 md:w-[200px] lg:w-[320px]"
				/>
			</div>
		);
	};

	type Filter = {
		filterOptions: Array<{
			filterName: string;
			filterDescription: string;
		}>;
		currentlyActiveFilterOption: string;
		onFilterOptionClick: (filterName: string) => void;
	};

	const Filter = () => {
		return (
			<DropdownMenu>
				<DropdownMenuTrigger asChild>
					<Button variant="outline">Search By</Button>
				</DropdownMenuTrigger>
				<DropdownMenuContent className="w-56">
					<DropdownMenuLabel>Search orders by</DropdownMenuLabel>
					<DropdownMenuSeparator />
					<DropdownMenuCheckboxItem
						onClick={() => setActiveSearchFilter("customerName")}
						checked={activeSearchFilter === "customerName" ? true : false}
					>
						Customer name
					</DropdownMenuCheckboxItem>
					<DropdownMenuCheckboxItem
						onClick={() => setActiveSearchFilter("customerSurname")}
						checked={activeSearchFilter === "customerSurname" ? true : false}
					>
						Customer surname
					</DropdownMenuCheckboxItem>
					<DropdownMenuCheckboxItem
						onClick={() => setActiveSearchFilter("customerEmail")}
						checked={activeSearchFilter === "customerEmail" ? true : false}
					>
						Customer email
					</DropdownMenuCheckboxItem>
					<DropdownMenuCheckboxItem
						onClick={() => setActiveSearchFilter("productName")}
						checked={activeSearchFilter === "productName" ? true : false}
					>
						Product name
					</DropdownMenuCheckboxItem>
					<DropdownMenuCheckboxItem
						onClick={() => setActiveSearchFilter("orderShipAddress")}
						checked={activeSearchFilter === "orderShipAddress" ? true : false}
					>
						Order ship address
					</DropdownMenuCheckboxItem>
				</DropdownMenuContent>
			</DropdownMenu>
		);
	};

	type Sort = {
		sortOptions: Array<>;
		onSortOptionClick: () => void;
	};

	const Sort = () => {
		return (
			<DropdownMenu>
				<DropdownMenuTrigger asChild>
					<Button variant="outline" size="sm" className="h-10 gap-1 text-sm">
						<ListFilter className="h-3.5 w-3.5" />
						<span className="sr-only sm:not-sr-only">Sort</span>
					</Button>
				</DropdownMenuTrigger>
				<DropdownMenuContent align="end">
					<DropdownMenuLabel>Sort by creation date</DropdownMenuLabel>
					<DropdownMenuSeparator />
					<DropdownMenuCheckboxItem
						onClick={() =>
							handleSortClick({
								name: "CreationDateDesc",
								sortColumn: "CreatedAt",
								sortOrder: "DESC"
							})
						}
						checked={activeSortingMethod?.activeSortingMethodName === "CreationDateDesc"}
					>
						Newest
					</DropdownMenuCheckboxItem>
					<DropdownMenuCheckboxItem
						onClick={() =>
							handleSortClick({
								name: "CreationDateAsc",
								sortColumn: "CreatedAt",
								sortOrder: "ASC"
							})
						}
						checked={activeSortingMethod?.activeSortingMethodName === "CreationDateAsc"}
					>
						Oldest
					</DropdownMenuCheckboxItem>
				</DropdownMenuContent>
			</DropdownMenu>
		);
	};

	return (
		<Tabs defaultValue="created">
			<div className="flex items-center">
				<TabsList>
					<TabsTrigger onClick={() => handleTabClick("created")} className="block" value="created">
						Created
					</TabsTrigger>
					<TabsTrigger
						onClick={() => handleTabClick("pendingforpayment")}
						className="hidden min-[1200px]:block"
						value="pendingforpayment"
					>
						Pending for payment
					</TabsTrigger>
					<TabsTrigger
						onClick={() => handleTabClick("paid")}
						className="hidden min-[520px]:block"
						value="paid"
					>
						Paid
					</TabsTrigger>
					<TabsTrigger
						onClick={() => handleTabClick("intransit")}
						className="hidden min-[920px]:block"
						value="intransit"
					>
						In transit
					</TabsTrigger>
					<TabsTrigger
						onClick={() => handleTabClick("delivered")}
						className="hidden min-[768px]:block"
						value="delivered"
					>
						Delivered
					</TabsTrigger>
					<TabsTrigger
						onClick={() => handleTabClick("cancelled")}
						className="hidden min-[600px]:block"
						value="cancelled"
					>
						Cancelled
					</TabsTrigger>
				</TabsList>
				<div className="ml-auto flex items-center gap-2">
					<div className="relative ml-auto flex-1 md:grow-0">
						<SearchIcon className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
						<Input
							onChange={(event) => handleSearchInputChange(event)}
							type="search"
							placeholder="Search..."
							className="w-full rounded-lg bg-background pl-8 md:w-[200px] lg:w-[320px]"
						/>
					</div>
					<DropdownMenu>
						<DropdownMenuTrigger asChild>
							<Button variant="outline">Search By</Button>
						</DropdownMenuTrigger>
						<DropdownMenuContent className="w-56">
							<DropdownMenuLabel>Search orders by</DropdownMenuLabel>
							<DropdownMenuSeparator />
							<DropdownMenuCheckboxItem
								onClick={() => setActiveSearchFilter("customerName")}
								checked={activeSearchFilter === "customerName" ? true : false}
							>
								Customer name
							</DropdownMenuCheckboxItem>
							<DropdownMenuCheckboxItem
								onClick={() => setActiveSearchFilter("customerSurname")}
								checked={activeSearchFilter === "customerSurname" ? true : false}
							>
								Customer surname
							</DropdownMenuCheckboxItem>
							<DropdownMenuCheckboxItem
								onClick={() => setActiveSearchFilter("customerEmail")}
								checked={activeSearchFilter === "customerEmail" ? true : false}
							>
								Customer email
							</DropdownMenuCheckboxItem>
							<DropdownMenuCheckboxItem
								onClick={() => setActiveSearchFilter("productName")}
								checked={activeSearchFilter === "productName" ? true : false}
							>
								Product name
							</DropdownMenuCheckboxItem>
							<DropdownMenuCheckboxItem
								onClick={() => setActiveSearchFilter("orderShipAddress")}
								checked={activeSearchFilter === "orderShipAddress" ? true : false}
							>
								Order ship address
							</DropdownMenuCheckboxItem>
						</DropdownMenuContent>
					</DropdownMenu>
					<DropdownMenu>
						<DropdownMenuTrigger asChild>
							<Button variant="outline" size="sm" className="h-10 gap-1 text-sm">
								<ListFilter className="h-3.5 w-3.5" />
								<span className="sr-only sm:not-sr-only">Sort</span>
							</Button>
						</DropdownMenuTrigger>
						<DropdownMenuContent align="end">
							<DropdownMenuLabel>Sort by creation date</DropdownMenuLabel>
							<DropdownMenuSeparator />
							<DropdownMenuCheckboxItem
								onClick={() =>
									handleSortClick({
										name: "CreationDateDesc",
										sortColumn: "CreatedAt",
										sortOrder: "DESC"
									})
								}
								checked={activeSortingMethod?.activeSortingMethodName === "CreationDateDesc"}
							>
								Newest
							</DropdownMenuCheckboxItem>
							<DropdownMenuCheckboxItem
								onClick={() =>
									handleSortClick({
										name: "CreationDateAsc",
										sortColumn: "CreatedAt",
										sortOrder: "ASC"
									})
								}
								checked={activeSortingMethod?.activeSortingMethodName === "CreationDateAsc"}
							>
								Oldest
							</DropdownMenuCheckboxItem>
						</DropdownMenuContent>
					</DropdownMenu>
				</div>
			</div>
			<TabsContent value="created">
				<OrdersTable
					ordersTableLastRowRef={allOrdersTableLastRowRef}
					data={allOrders}
					error={allOrdersError}
					isPending={isAllOrdersPending}
					isError={isAllOrdersError}
					isFetchingNextPage={isAllOrdersFetchingNextPage}
					hasNextPage={hasAllOrdersNextPage}
					status={allOrdersStatus}
					description={"Complete list of all orders."}
				/>
			</TabsContent>
			<TabsContent value="pendingforpayment">
				<OrdersTable
					ordersTableLastRowRef={pendingForPaymentOrdersTableLastRowRef}
					data={pendingForPaymentOrders}
					error={pendingForPaymentOrdersError}
					isPending={isPendingForPaymentOrdersPending}
					isError={isPendingForPaymentOrdersError}
					isFetchingNextPage={isPendingForPaymentOrdersFetchingNextPage}
					hasNextPage={hasPendingForPaymentOrdersNextPage}
					status={pendingForPaymentOrdersStatus}
					description={"List of all orders pending for payment."}
				/>
			</TabsContent>
			<TabsContent value="paid">
				<OrdersTable
					ordersTableLastRowRef={paidOrdersTableLastRowRef}
					data={paidOrders}
					error={paidOrdersError}
					isPending={isPaidOrdersPending}
					isError={isPaidOrdersError}
					isFetchingNextPage={isPaidOrdersFetchingNextPage}
					hasNextPage={hasPaidOrdersNextPage}
					status={paidOrdersStatus}
					description={"Complete list of paid orders."}
				/>
			</TabsContent>
			<TabsContent value="intransit">
				<OrdersTable
					ordersTableLastRowRef={inTransitOrdersTableLastRowRef}
					data={inTransitOrders}
					error={inTransitOrdersError}
					isPending={isInTransitOrdersPending}
					isError={isInTransitOrdersError}
					isFetchingNextPage={isInTransitOrdersFetchingNextPage}
					hasNextPage={hasInTransitOrdersNextPage}
					status={inTransitOrdersStatus}
					description={"List of all orders in transit."}
				/>
			</TabsContent>
			<TabsContent value="delivered">
				<OrdersTable
					ordersTableLastRowRef={deliveredOrdersTableLastRowRef}
					data={deliveredOrders}
					error={deliveredOrdersError}
					isPending={isDeliveredOrdersPending}
					isError={isDeliveredOrdersError}
					isFetchingNextPage={isDeliveredOrdersFetchingNextPage}
					hasNextPage={hasDeliveredOrdersNextPage}
					status={deliveredOrdersStatus}
					description={"List of all delivered orders."}
				/>
			</TabsContent>
			<TabsContent value="cancelled">
				<OrdersTable
					ordersTableLastRowRef={cancelledOrdersTableLastRowRef}
					data={cancelledOrders}
					error={cancelledOrdersError}
					isPending={isCancelledOrdersPending}
					isError={isCancelledOrdersError}
					isFetchingNextPage={isCancelledOrdersFetchingNextPage}
					hasNextPage={hasCancelledOrdersNextPage}
					status={cancelledOrdersStatus}
					description={"List of all cancelled orders."}
				/>
			</TabsContent>
		</Tabs>
	);
};
