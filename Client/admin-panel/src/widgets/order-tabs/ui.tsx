"use client";

import { FC, useEffect, useState } from "react";
import { useInView } from "react-intersection-observer";
import { useInfiniteQuery } from "@tanstack/react-query";
import { useSession } from "next-auth/react";

import { OrderStatus } from "@entities/orders/model";
import { getPaginatedSortedAndFilteredOrders } from "@entities/orders/api";

import { Search } from "@features/search";
import { Filter } from "@features/filter";
import { Sort } from "@features/sort";

import { OrdersTable } from "@widgets/oders-table";

import { Tabs, TabsContent, TabsList, TabsTrigger } from "@shared/ui/tabs";
import { useDebounce } from "@shared/lib/hooks/useDebounce";
import { sanitizeInput } from "@shared/lib";

export type sortingMethod = {
	activeSortingMethodName: string;
	sortColumn: string;
	sortOrder: string;
};

type OrdersTabList = Array<{
	tabName: string;
	tabDescription: string;
	tabTrigger: {
		classes?: string;
		onTabClick: (tabName: string) => void;
	};
	orderStatus: OrderStatus;
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

type OrdersTabsTriggerProps = {
	tabName: string;
	classes?: string;
	onTabClick: (tabName: string) => void;
};

const OrdersTabsTrigger: FC<OrdersTabsTriggerProps> = ({ tabName, classes, onTabClick }) => {
	return (
		<TabsTrigger onClick={() => onTabClick(tabName)} className={classes} value={tabName}>
			{tabName}
		</TabsTrigger>
	);
};

type OrdersTabContentProps = {
	tabName: string;
	tabDescription: string;
	currentActiveTab: string;
	currentActiveSortingMethod: {
		activeSortingMethodName: string;
		sortColumn: string;
		sortOrder: string;
	} | null;
	currentActiveSearchFilter: string | null;
	searchTerm: string | null;
	setSearchTerm: (searchTerm: string | null) => void;
	setIsDataRefetched: (isRefetched: boolean) => void;
	isDataRefetched: boolean;
	orderStatus: OrderStatus;
};

const OrdersTabContent: FC<OrdersTabContentProps> = ({
	tabName,
	currentActiveTab,
	currentActiveSortingMethod,
	searchTerm,
	setSearchTerm,
	tabDescription,
	currentActiveSearchFilter,
	setIsDataRefetched,
	isDataRefetched,
	orderStatus
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
		queryKey: [
			tabName,
			userId,
			accessToken,
			searchTerm,
			orderStatus,
			currentActiveSortingMethod?.sortColumn,
			currentActiveSortingMethod?.sortOrder,
			currentActiveSearchFilter
		],
		queryFn: ({ pageParam }) =>
			getPaginatedSortedAndFilteredOrders({
				accessToken,
				pageParam,
				pageSize: 6,
				orderStatus: orderStatus,
				sortColumn: currentActiveSortingMethod?.sortColumn,
				sortOrder: currentActiveSortingMethod?.sortOrder,
				customerName: currentActiveSearchFilter === "customerName" ? searchTerm : null,
				customerSurname: currentActiveSearchFilter === "customerSurname" ? searchTerm : null,
				customerEmail: currentActiveSearchFilter === "customerEmail" ? searchTerm : null,
				productName: currentActiveSearchFilter === "productName" ? searchTerm : null,
				orderShipAddress: currentActiveSearchFilter === "orderShipAddress" ? searchTerm : null
			}),
		initialPageParam: 0,
		getNextPageParam: (lastPage, allPages) => (lastPage.length === 6 ? allPages.length : undefined),
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
			currentActiveSortingMethod?.activeSortingMethodName &&
			currentActiveTab === tabName &&
			!isDataRefetched
		) {
			refetchOrders();
			setIsDataRefetched(true);
		}
	}, [currentActiveSortingMethod?.activeSortingMethodName]);

	useEffect(() => {
		if (searchTerm && searchTerm.length > 0 && currentActiveTab === tabName && !isDataRefetched) {
			setIsDataRefetched(true);
			refetchOrders();
		} else if (searchTerm === "" && currentActiveTab === tabName && !isDataRefetched) {
			setIsDataRefetched(true);
			setSearchTerm(null);
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

export const OrderTabs: FC = () => {
	const [activeTab, setActiveTab] = useState<string>("created");
	const [activeSortingMethod, setActiveSortingMethod] = useState<sortingMethod | null>(null);
	const [activeSearchFilter, setActiveSearchFilter] = useState<string | null>("customerName");
	const [isDataRefetched, setIsDataRefetched] = useState<boolean>(false);
	const [searchTerm, setSearchTerm] = useState<string | null>(null);
	const debouncedSearchTerm = useDebounce(searchTerm, 600);

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

	const ordersTabList: OrdersTabList = [
		{
			tabName: "created",
			tabDescription: "Complete list of all orders.",
			tabTrigger: {
				classes: "block",
				onTabClick: handleTabClick
			},
			orderStatus: OrderStatus.Created,
			currentActiveTab: activeTab,
			currentActiveSortingMethod: activeSortingMethod,
			currentActiveSearchFilter: activeSearchFilter,
			searchTerm: debouncedSearchTerm,
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
			orderStatus: OrderStatus.PendingForPayment,
			currentActiveTab: activeTab,
			currentActiveSortingMethod: activeSortingMethod,
			currentActiveSearchFilter: activeSearchFilter,
			searchTerm: debouncedSearchTerm,
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
			orderStatus: OrderStatus.Paid,
			currentActiveTab: activeTab,
			currentActiveSortingMethod: activeSortingMethod,
			currentActiveSearchFilter: activeSearchFilter,
			searchTerm: debouncedSearchTerm,
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
			orderStatus: OrderStatus.InTransit,
			currentActiveTab: activeTab,
			currentActiveSortingMethod: activeSortingMethod,
			currentActiveSearchFilter: activeSearchFilter,
			searchTerm: debouncedSearchTerm,
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
			orderStatus: OrderStatus.Delivered,
			currentActiveTab: activeTab,
			currentActiveSortingMethod: activeSortingMethod,
			currentActiveSearchFilter: activeSearchFilter,
			searchTerm: debouncedSearchTerm,
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
			orderStatus: OrderStatus.Cancelled,
			currentActiveTab: activeTab,
			currentActiveSortingMethod: activeSortingMethod,
			currentActiveSearchFilter: activeSearchFilter,
			searchTerm: debouncedSearchTerm,
			setIsDataRefetched: setIsDataRefetched,
			isDataRefetched: isDataRefetched
		}
	];

	return (
		<Tabs defaultValue="created">
			<div className="flex items-center">
				<TabsList>
					{ordersTabList.map(({ tabName, tabTrigger: { classes, onTabClick } }, index) => (
						<OrdersTabsTrigger
							key={index + "-" + tabName}
							tabName={tabName}
							classes={classes}
							onTabClick={onTabClick}
						/>
					))}
				</TabsList>
				<div className="ml-auto flex items-center gap-2">
					<Search onSearchInputChange={handleSearchInputChange} />
					<Filter
						filterGroups={[
							{
								groupName: "Search orders by",
								groupFilters: [
									{ filterName: "Customer name" },
									{ filterName: "Customer surname" },
									{ filterName: "Customer email" },
									{ filterName: "Product name" },
									{ filterName: "Order ship address" }
								]
							}
						]}
						filterButtonName="Search By"
						currentlyActiveFilter={activeSearchFilter}
						onFilterClick={setActiveSearchFilter}
					/>
					<Sort
						sortMethodGroups={[
							{
								groupName: "Sort by creation date",
								groupSortMethods: [
									{
										methodName: "Newest",
										activeSortingMethodName: "CreationDateDesc",
										sortColumn: "CreatedAt",
										sortOrder: "DESC"
									},
									{
										methodName: "Oldest",
										activeSortingMethodName: "CreationDateAsc",
										sortColumn: "CreatedAt",
										sortOrder: "ASC"
									}
								]
							}
						]}
						sortButtonName="Sort"
						currentlyActiveSortingMethod={activeSortingMethod}
						onSortMethodClick={handleSortClick}
					/>
				</div>
			</div>
			{ordersTabList.map(
				(
					{
						tabName,
						tabDescription,
						currentActiveTab,
						currentActiveSearchFilter,
						currentActiveSortingMethod,
						searchTerm,
						setIsDataRefetched,
						isDataRefetched,
						orderStatus
					},
					index
				) => (
					<OrdersTabContent
						key={index + "-" + tabName + "-" + orderStatus}
						tabName={tabName}
						tabDescription={tabDescription}
						currentActiveTab={currentActiveTab}
						currentActiveSortingMethod={currentActiveSortingMethod}
						currentActiveSearchFilter={currentActiveSearchFilter}
						searchTerm={searchTerm}
						setSearchTerm={setSearchTerm}
						setIsDataRefetched={setIsDataRefetched}
						isDataRefetched={isDataRefetched}
						orderStatus={orderStatus}
					/>
				)
			)}
		</Tabs>
	);
};
