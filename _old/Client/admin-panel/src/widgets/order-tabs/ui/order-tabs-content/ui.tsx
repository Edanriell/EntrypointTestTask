import { FC, useEffect } from "react";
import { useSession } from "next-auth/react";
import { useInfiniteQuery } from "@tanstack/react-query";
import { useInView } from "react-intersection-observer";

import {
	getPaginatedSortedAndFilteredOrders,
	OrderCard,
	OrderRow,
	OrderStatus
} from "@entities/orders";

import { ContentTable } from "@widgets/content-table";
import { ContentDrawer } from "@widgets/content-drawer";
import { SortingMethod } from "@widgets/order-tabs";

import { TabsContent } from "@shared/ui/tabs";

type OrderTabsContentProps = {
	tabName: string;
	tabDescription: string;
	currentActiveTab: string;
	currentActiveSortingMethod: SortingMethod | null;
	currentActiveSearchFilter: string | null;
	searchTerm: string | null;
	setSearchTerm: (searchTerm: string | null) => void;
	setIsDataRefetched: (isRefetched: boolean) => void;
	isDataRefetched: boolean;
	orderStatus: OrderStatus;
};

export const OrderTabsContent: FC<OrderTabsContentProps> = ({
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

	const { ref: tableLastRowRef, inView: isLastTableRowInView } = useInView();

	useEffect(() => {
		if (isLastTableRowInView && hasOrdersNextPage) {
			fetchNextOrdersPage();
		}
	}, [isLastTableRowInView, hasOrdersNextPage]);

	useEffect(() => {
		if (
			currentActiveSortingMethod?.uniqueMethodName &&
			currentActiveTab === tabName &&
			!isDataRefetched
		) {
			refetchOrders();
			setIsDataRefetched(true);
		}
	}, [currentActiveSortingMethod?.uniqueMethodName]);

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

	const tableColumns = [
		{
			name: "Customer",
			classes: "block"
		},
		{
			name: "Status",
			classes: "hidden min-[520px]:table-cell"
		},
		{
			name: "Date",
			classes: "hidden min-[920px]:table-cell"
		},
		{
			name: "Last update",
			classes: "hidden min-[1040px]:table-cell"
		},
		{
			name: "Ship address",
			classes: "hidden min-[1200px]:table-cell"
		},
		{
			name: "Order information",
			classes: "hidden min-[1320px]:table-cell"
		},
		{
			name: "Amount",
			classes: "text-right"
		}
	];

	return (
		<TabsContent value={tabName}>
			<ContentTable
				tableLastRowRef={tableLastRowRef}
				data={orders}
				error={ordersError}
				isPending={isOrdersPending}
				isError={isOrdersError}
				isFetchingNextPage={isOrdersFetchingNextPage}
				hasNextPage={hasOrdersNextPage}
				status={ordersStatus}
				description={tabDescription}
				columnsData={tableColumns}
				Row={OrderRow}
				Drawer={ContentDrawer}
				DrawerInnerContent={OrderCard}
			/>
		</TabsContent>
	);
};
