"use client";

import { Fragment } from "react";
import { useSession } from "next-auth/react";
import { useQuery } from "@tanstack/react-query";

import type { Order } from "@entities/orders/model";
import type { User } from "@entities/users/model";
import type { Product } from "@entities/products/model";
import { getAllOrders, getRecentOrders } from "@entities/orders/api";
import { getAllProducts } from "@entities/products/api";
import { getAllUsers, getRecentUsers } from "@entities/users/api";

import { MemoizedTotalRevenue } from "@widgets/total-revenue";
import { MemoizedTotalUsers } from "@widgets/total-users";
import { MemoizedTotalOrders } from "@widgets/total-orders";
import { MemoizedTotalProducts } from "@widgets/total-products";
import { RecentOrders } from "@widgets/recent-orders";
import { RecentUsers } from "@widgets/recent-users";

export const DashboardPage = () => {
	const { data: session, status } = useSession();

	const userId = session?.user.id;
	const accessToken = session?.accessToken;

	const {
		data: ordersData,
		error: ordersError,
		isPending: isOrdersPending,
		isError: isOrdersError
	} = useQuery({
		queryKey: ["getAllOrders", userId],
		queryFn: (): Promise<Array<Order>> => getAllOrders(accessToken!),
		enabled: !!userId && !!accessToken
	});

	const {
		data: usersData,
		error: usersError,
		isPending: isUsersPending,
		isError: isUsersError
	} = useQuery({
		queryKey: ["getAllUsers", userId],
		queryFn: (): Promise<Array<User>> => getAllUsers(accessToken!),
		enabled: !!userId && !!accessToken
	});

	const {
		data: productsData,
		error: productsError,
		isPending: isProductsPending,
		isError: isProductsError
	} = useQuery({
		queryKey: ["getAllProducts", userId],
		queryFn: (): Promise<Array<Product>> => getAllProducts(accessToken!),
		enabled: !!userId && !!accessToken
	});

	const {
		data: recentOrdersData,
		error: recentOrdersError,
		isPending: isRecentOrdersPending,
		isError: isRecentOrdersError
	} = useQuery({
		queryKey: ["recentOrders", userId],
		queryFn: (): Promise<Array<Order>> => getRecentOrders(accessToken!),
		enabled: !!userId && !!accessToken
	});

	const {
		data: recentUsersData,
		error: recentUsersError,
		isPending: isRecentUsersPending,
		isError: isRecentUsersError
	} = useQuery({
		queryKey: ["recentUsers", userId],
		queryFn: (): Promise<Array<User>> => getRecentUsers(accessToken!),
		enabled: !!userId && !!accessToken
	});

	return (
		<Fragment>
			<div className="grid gap-4 md:grid-cols-2 md:gap-8 lg:grid-cols-4">
				<MemoizedTotalRevenue
					data={ordersData}
					error={ordersError}
					isPending={isOrdersPending}
					isError={isOrdersError}
				/>
				<MemoizedTotalUsers
					data={usersData}
					error={usersError}
					isPending={isUsersPending}
					isError={isUsersError}
				/>
				<MemoizedTotalOrders
					data={ordersData}
					error={ordersError}
					isPending={isOrdersPending}
					isError={isOrdersError}
				/>
				<MemoizedTotalProducts
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
