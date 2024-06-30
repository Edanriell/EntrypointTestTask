"use client";

import { Fragment } from "react";
import { useSession } from "next-auth/react";
import { useQuery } from "@tanstack/react-query";

import { TotalRevenue } from "@widgets/total-revenue";
import { TotalUsers } from "@widgets/total-users";
import { TotalOrders } from "@widgets/total-orders";
import { TotalProducts } from "@widgets/total-products";
import { RecentOrders } from "@widgets/recent-orders";
import { RecentUsers } from "@widgets/recent-users";

import { getAllOrders, getAllProducts, getAllUsers, getRecentOrders, getRecentUsers } from "./api";

export const DashboardPage = () => {
	const { data: session, status } = useSession();

	const userId = (session as any)?.user?.id;
	const accessToken = (session as any)?.accessToken;

	const {
		data: ordersData,
		error: ordersError,
		isPending: isOrdersPending,
		isError: isOrdersError
	} = useQuery({
		queryKey: ["getAllOrders", userId],
		queryFn: () => getAllOrders(accessToken),
		enabled: !!userId && !!accessToken
	});

	const {
		data: usersData,
		error: usersError,
		isPending: isUsersPending,
		isError: isUsersError
	} = useQuery({
		queryKey: ["getAllUsers", userId],
		queryFn: () => getAllUsers(accessToken),
		enabled: !!userId && !!accessToken
	});

	const {
		data: productsData,
		error: productsError,
		isPending: isProductsPending,
		isError: isProductsError
	} = useQuery({
		queryKey: ["getAllProducts", userId],
		queryFn: () => getAllProducts(accessToken),
		enabled: !!userId && !!accessToken
	});

	const {
		data: recentOrdersData,
		error: recentOrdersError,
		isPending: isRecentOrdersPending,
		isError: isRecentOrdersError
	} = useQuery({
		queryKey: ["recentOrders", userId],
		queryFn: () => getRecentOrders(accessToken),
		enabled: !!userId && !!accessToken
	});

	const {
		data: recentUsersData,
		error: recentUsersError,
		isPending: isRecentUsersPending,
		isError: isRecentUsersError
	} = useQuery({
		queryKey: ["recentUsers", userId],
		queryFn: () => getRecentUsers(accessToken),
		enabled: !!userId && !!accessToken
	});

	return (
		<Fragment>
			<div className="grid gap-4 md:grid-cols-2 md:gap-8 lg:grid-cols-4">
				<TotalRevenue
					data={ordersData}
					error={ordersError}
					isPending={isOrdersPending}
					isError={isOrdersError}
				/>
				<TotalUsers
					data={usersData}
					error={usersError}
					isPending={isUsersPending}
					isError={isUsersError}
				/>
				<TotalOrders
					data={ordersData}
					error={ordersError}
					isPending={isOrdersPending}
					isError={isOrdersError}
				/>
				<TotalProducts
					data={productsData}
					error={productsError}
					isPending={isProductsPending}
					isError={isProductsError}
				/>
			</div>
			<div className="grid gap-4 md:gap-8 lg:grid-cols-2 xl:grid-cols-3">
				<RecentOrders
					data={recentOrdersData}
					error={recentOrdersError}
					isPending={isRecentOrdersPending}
					isError={isRecentOrdersError}
				/>
				<RecentUsers
					data={recentUsersData}
					error={recentUsersError}
					isPending={isRecentUsersPending}
					isError={isRecentUsersError}
				/>
			</div>
		</Fragment>
	);
};
