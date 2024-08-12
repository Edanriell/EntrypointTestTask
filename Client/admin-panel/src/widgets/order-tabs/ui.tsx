"use client";

import { ChangeEvent, FC, useState } from "react";

import { OrderStatus } from "@entities/orders/model";

import { Search } from "@features/search";
import { Filter } from "@features/filter";
import { Sort } from "@features/sort";

import { Tabs, TabsList } from "@shared/ui/tabs";
import { useDebounce } from "@shared/lib/hooks/useDebounce";
import { sanitizeInput, toCamelCase } from "@shared/lib";

import { OrderTabsContent } from "./ui/order-tabs-content";
import { OrderTabsTrigger } from "./ui/order-tabs-trigger";

export type SortingMethod = {
	uniqueMethodName: string;
	sortColumn: string;
	sortOrder: string;
};

type OrderTab = {
	currentActiveTab: string;
	currentActiveSortingMethod: SortingMethod | null;
	currentActiveSearchFilter: string | null;
	searchTerm: string | null;
	setIsDataRefetched: (isRefetched: boolean) => void;
	isDataRefetched: boolean;
};

type OrdersTabList = Array<
	{
		tabName: string;
		tabDescription: string;
		tabTrigger: {
			classes?: string;
			onTabClick: (tabName: string) => void;
		};
		orderStatus: OrderStatus;
	} & OrderTab
>;

export const OrderTabs: FC = () => {
	const [activeTab, setActiveTab] = useState<string>("created");
	const [activeSortingMethod, setActiveSortingMethod] = useState<SortingMethod | null>(null);
	const [activeSearchFilter, setActiveSearchFilter] = useState<string | null>("customerName");
	const [searchTerm, setSearchTerm] = useState<string | null>(null);
	const debouncedSearchTerm = useDebounce(searchTerm, 600);
	const [isDataRefetched, setIsDataRefetched] = useState<boolean>(false);

	const handleTabClick = (tabName: string) => {
		setActiveTab(tabName);
	};

	const handleSortMethodClick = (sortMethod: SortingMethod) => {
		setIsDataRefetched(false);
		setActiveSortingMethod({
			uniqueMethodName: sortMethod.uniqueMethodName,
			sortColumn: sortMethod.sortColumn,
			sortOrder: sortMethod.sortOrder
		});
	};

	const handleSearchInputChange = (event: ChangeEvent<HTMLInputElement>) => {
		setIsDataRefetched(false);
		setSearchTerm(sanitizeInput(event.target.value));
	};

	const orderTab: OrderTab = {
		currentActiveTab: activeTab,
		currentActiveSortingMethod: activeSortingMethod,
		currentActiveSearchFilter: activeSearchFilter,
		searchTerm: debouncedSearchTerm,
		setIsDataRefetched: setIsDataRefetched,
		isDataRefetched: isDataRefetched
	};

	const ordersTabList: OrdersTabList = [
		{
			tabName: "Created",
			tabDescription: "Complete list of all orders.",
			tabTrigger: {
				classes: "block",
				onTabClick: handleTabClick
			},
			orderStatus: OrderStatus.Created,
			...orderTab
		},
		{
			tabName: "Pending for payment",
			tabDescription: "List of all orders pending for payment.",
			tabTrigger: {
				classes: "hidden min-[1200px]:block",
				onTabClick: handleTabClick
			},
			orderStatus: OrderStatus.PendingForPayment,
			...orderTab
		},
		{
			tabName: "Paid",
			tabDescription: "Complete list of paid orders.",
			tabTrigger: {
				classes: "hidden min-[520px]:block",
				onTabClick: handleTabClick
			},
			orderStatus: OrderStatus.Paid,
			...orderTab
		},
		{
			tabName: "In transit",
			tabDescription: "List of all orders in transit.",
			tabTrigger: {
				classes: "hidden min-[920px]:block",
				onTabClick: handleTabClick
			},
			orderStatus: OrderStatus.InTransit,
			...orderTab
		},
		{
			tabName: "Delivered",
			tabDescription: "List of all delivered orders.",
			tabTrigger: {
				classes: "hidden min-[768px]:block",
				onTabClick: handleTabClick
			},
			orderStatus: OrderStatus.Delivered,
			...orderTab
		},
		{
			tabName: "Cancelled",
			tabDescription: "List of all cancelled orders.",
			tabTrigger: {
				classes: "hidden min-[600px]:block",
				onTabClick: handleTabClick
			},
			orderStatus: OrderStatus.Cancelled,
			...orderTab
		}
	];

	const orderFilters = [
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
	];

	const orderSortMethods = [
		{
			groupName: "Sort by creation date",
			groupSortMethods: [
				{
					methodName: "Newest",
					uniqueMethodName: "CreationDateDesc",
					sortColumn: "CreatedAt",
					sortOrder: "DESC"
				},
				{
					methodName: "Oldest",
					uniqueMethodName: "CreationDateAsc",
					sortColumn: "CreatedAt",
					sortOrder: "ASC"
				}
			]
		}
	];

	return (
		<Tabs defaultValue="created">
			<div className="flex items-center">
				<TabsList>
					{ordersTabList.map(({ tabName, tabTrigger: { classes, onTabClick } }, index) => (
						<OrderTabsTrigger
							key={index + "-" + toCamelCase(tabName)}
							tabName={tabName}
							classes={classes}
							onTabClick={onTabClick}
						/>
					))}
				</TabsList>
				<div className="ml-auto flex items-center gap-2">
					<Search onSearchInputChange={handleSearchInputChange} />
					<Filter
						filterGroups={orderFilters}
						filterButtonName="Search By"
						currentlyActiveFilter={activeSearchFilter}
						onFilterClick={setActiveSearchFilter}
					/>
					<Sort
						sortMethodGroups={orderSortMethods}
						sortButtonName="Sort"
						currentlyActiveSortingMethod={activeSortingMethod}
						onSortMethodClick={handleSortMethodClick}
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
					<OrderTabsContent
						key={index + "-" + toCamelCase(tabName) + "-" + orderStatus}
						tabName={toCamelCase(tabName)}
						tabDescription={tabDescription}
						currentActiveTab={currentActiveTab}
						currentActiveSortingMethod={currentActiveSortingMethod}
						currentActiveSearchFilter={currentActiveSearchFilter}
						searchTerm={searchTerm}
						setSearchTerm={setSearchTerm}
						isDataRefetched={isDataRefetched}
						setIsDataRefetched={setIsDataRefetched}
						orderStatus={orderStatus}
					/>
				)
			)}
		</Tabs>
	);
};

// TODO
// This component must be reusable and universal
// Cards in dashboard must be reusable and universal
// TODO
