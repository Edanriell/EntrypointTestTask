"use client";

import { FC } from "react";
import { AuthGuard } from "@features/authentication/general";
import {
	Area,
	AreaChart,
	Bar,
	BarChart,
	CartesianGrid,
	Cell,
	Legend,
	Line,
	LineChart,
	Pie,
	PieChart,
	ResponsiveContainer,
	Tooltip,
	XAxis,
	YAxis
} from "recharts";

// Sample data based on your actual entities - replace with real API calls
const orderStatusData = [
	{ status: "Created", count: 23, color: "#94a3b8" },
	{ status: "PendingForPayment", count: 45, color: "#f59e0b" },
	{ status: "Paid", count: 67, color: "#3b82f6" },
	{ status: "InTransit", count: 34, color: "#8b5cf6" },
	{ status: "Delivered", count: 189, color: "#10b981" },
	{ status: "Cancelled", count: 8, color: "#ef4444" }
];

const monthlyOrdersData = [
	{ month: "Jan", orders: 65, revenue: 12400 },
	{ month: "Feb", orders: 89, revenue: 18200 },
	{ month: "Mar", orders: 76, revenue: 15800 },
	{ month: "Apr", orders: 112, revenue: 22100 },
	{ month: "May", orders: 134, revenue: 28600 },
	{ month: "Jun", orders: 158, revenue: 35200 }
];

const topProductsData = [
	{ productName: "Wireless Headphones", unitsSold: 245, revenue: 24500 },
	{ productName: "Smart Watch", unitsSold: 189, revenue: 37800 },
	{ productName: "Laptop Stand", unitsSold: 167, revenue: 8350 },
	{ productName: "USB-C Cable", unitsSold: 432, revenue: 8640 },
	{ productName: "Bluetooth Speaker", unitsSold: 156, revenue: 15600 }
];

const inventoryStatusData = [
	{ category: "In Stock", products: 156, color: "#22c55e" },
	{ category: "Low Stock", products: 23, color: "#f59e0b" },
	{ category: "Out of Stock", products: 8, color: "#ef4444" },
	{ category: "On Order", products: 34, color: "#3b82f6" }
];

const customersData = [
	{ month: "Jan", totalCustomers: 245, totalOrders: 65 },
	{ month: "Feb", totalCustomers: 289, totalOrders: 89 },
	{ month: "Mar", totalCustomers: 334, totalOrders: 76 },
	{ month: "Apr", totalCustomers: 378, totalOrders: 112 },
	{ month: "May", totalCustomers: 423, totalOrders: 134 },
	{ month: "Jun", totalCustomers: 467, totalOrders: 158 }
];

export const HomePage: FC = () => {
	return (
		<AuthGuard>
			<div className="flex flex-1 flex-col gap-6 p-6">
				{/* Header */}
				<div className="flex flex-col gap-2">
					<h1 className="text-3xl font-bold tracking-tight">Business Dashboard</h1>
					<p className="text-muted-foreground">
						Monitor your orders, products, and customer metrics in real-time.
					</p>
				</div>

				{/* Key Metrics Cards */}
				<div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
					<div className="rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
						<div className="flex flex-row items-center justify-between space-y-0 pb-2">
							<h3 className="text-sm font-medium">Total Orders</h3>
							<svg
								xmlns="http://www.w3.org/2000/svg"
								viewBox="0 0 24 24"
								fill="none"
								stroke="currentColor"
								strokeLinecap="round"
								strokeLinejoin="round"
								strokeWidth="2"
								className="h-4 w-4 text-muted-foreground"
							>
								<path d="M16 4h2a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h2" />
								<rect x="8" y="2" width="8" height="4" rx="1" ry="1" />
							</svg>
						</div>
						<div className="space-y-1">
							<div className="text-2xl font-bold">1,234</div>
							<p className="text-xs text-muted-foreground">+12% from last month</p>
						</div>
					</div>

					<div className="rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
						<div className="flex flex-row items-center justify-between space-y-0 pb-2">
							<h3 className="text-sm font-medium">Active Products</h3>
							<svg
								xmlns="http://www.w3.org/2000/svg"
								viewBox="0 0 24 24"
								fill="none"
								stroke="currentColor"
								strokeLinecap="round"
								strokeLinejoin="round"
								strokeWidth="2"
								className="h-4 w-4 text-muted-foreground"
							>
								<path d="M20 7h-9" />
								<path d="M14 17H5" />
								<circle cx="17" cy="17" r="3" />
								<circle cx="7" cy="7" r="3" />
							</svg>
						</div>
						<div className="space-y-1">
							<div className="text-2xl font-bold">187</div>
							<p className="text-xs text-muted-foreground">23 low stock items</p>
						</div>
					</div>

					<div className="rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
						<div className="flex flex-row items-center justify-between space-y-0 pb-2">
							<h3 className="text-sm font-medium">Total Customers</h3>
							<svg
								xmlns="http://www.w3.org/2000/svg"
								viewBox="0 0 24 24"
								fill="none"
								stroke="currentColor"
								strokeLinecap="round"
								strokeLinejoin="round"
								strokeWidth="2"
								className="h-4 w-4 text-muted-foreground"
							>
								<path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2" />
								<circle cx="9" cy="7" r="4" />
								<path d="M22 21v-2a4 4 0 0 0-3-3.87M16 3.13a4 4 0 0 1 0 7.75" />
							</svg>
						</div>
						<div className="space-y-1">
							<div className="text-2xl font-bold">892</div>
							<p className="text-xs text-muted-foreground">+67 new this month</p>
						</div>
					</div>

					<div className="rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
						<div className="flex flex-row items-center justify-between space-y-0 pb-2">
							<h3 className="text-sm font-medium">Monthly Revenue</h3>
							<svg
								xmlns="http://www.w3.org/2000/svg"
								viewBox="0 0 24 24"
								fill="none"
								stroke="currentColor"
								strokeLinecap="round"
								strokeLinejoin="round"
								strokeWidth="2"
								className="h-4 w-4 text-muted-foreground"
							>
								<path d="M12 2v20m9-9H3" />
							</svg>
						</div>
						<div className="space-y-1">
							<div className="text-2xl font-bold">$35,200</div>
							<p className="text-xs text-muted-foreground">+23% from last month</p>
						</div>
					</div>
				</div>

				{/* Charts Grid */}
				<div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
					{/* Monthly Orders & Revenue */}
					<div className="col-span-2 rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
						<div className="flex items-center justify-between mb-4">
							<h3 className="text-lg font-semibold">Orders & Revenue Trend</h3>
						</div>
						<ResponsiveContainer width="100%" height={300}>
							<AreaChart data={monthlyOrdersData}>
								<CartesianGrid strokeDasharray="3 3" />
								<XAxis dataKey="month" />
								<YAxis yAxisId="left" />
								<YAxis yAxisId="right" orientation="right" />
								<Tooltip
									formatter={(value, name) => [
										name === "orders" ? value : `$${value}`,
										name === "orders" ? "Orders" : "Revenue"
									]}
								/>
								<Legend />
								<Area
									yAxisId="left"
									type="monotone"
									dataKey="orders"
									stroke="#3b82f6"
									fill="#3b82f6"
									fillOpacity={0.6}
								/>
								<Area
									yAxisId="right"
									type="monotone"
									dataKey="revenue"
									stroke="#10b981"
									fill="#10b981"
									fillOpacity={0.3}
								/>
							</AreaChart>
						</ResponsiveContainer>
					</div>

					{/* Order Status Distribution */}
					<div className="rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
						<div className="flex items-center justify-between mb-4">
							<h3 className="text-lg font-semibold">Order Status</h3>
						</div>
						<ResponsiveContainer width="100%" height={300}>
							<PieChart>
								<Pie
									data={orderStatusData}
									cx="50%"
									cy="50%"
									outerRadius={80}
									dataKey="count"
									label={({ status, count }) => `${status}: ${count}`}
								>
									{orderStatusData.map((entry, index) => (
										<Cell key={`cell-${index}`} fill={entry.color} />
									))}
								</Pie>
								<Tooltip />
							</PieChart>
						</ResponsiveContainer>
					</div>

					{/* Top Selling Products */}
					<div className="col-span-2 rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
						<div className="flex items-center justify-between mb-4">
							<h3 className="text-lg font-semibold">Top Selling Products</h3>
						</div>
						<ResponsiveContainer width="100%" height={300}>
							<BarChart data={topProductsData}>
								<CartesianGrid strokeDasharray="3 3" />
								<XAxis
									dataKey="productName"
									angle={-45}
									textAnchor="end"
									height={80}
									interval={0}
								/>
								<YAxis />
								<Tooltip
									formatter={(value, name) => [
										name === "unitsSold" ? `${value} units` : `$${value}`,
										name === "unitsSold" ? "Units Sold" : "Revenue"
									]}
								/>
								<Legend />
								<Bar dataKey="unitsSold" fill="#8b5cf6" name="Units Sold" />
								<Bar dataKey="revenue" fill="#10b981" name="Revenue ($)" />
							</BarChart>
						</ResponsiveContainer>
					</div>

					{/* Inventory Status */}
					<div className="rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
						<div className="flex items-center justify-between mb-4">
							<h3 className="text-lg font-semibold">Inventory Status</h3>
						</div>
						<ResponsiveContainer width="100%" height={300}>
							<PieChart>
								<Pie
									data={inventoryStatusData}
									cx="50%"
									cy="50%"
									innerRadius={40}
									outerRadius={80}
									dataKey="products"
									label={({ category, products }) => `${category}: ${products}`}
								>
									{inventoryStatusData.map((entry, index) => (
										<Cell key={`cell-${index}`} fill={entry.color} />
									))}
								</Pie>
								<Tooltip />
							</PieChart>
						</ResponsiveContainer>
					</div>

					{/* Customer Growth & Order Volume */}
					<div className="col-span-2 rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
						<div className="flex items-center justify-between mb-4">
							<h3 className="text-lg font-semibold">
								Customer Growth & Order Volume
							</h3>
						</div>
						<ResponsiveContainer width="100%" height={300}>
							<LineChart data={customersData}>
								<CartesianGrid strokeDasharray="3 3" />
								<XAxis dataKey="month" />
								<YAxis yAxisId="left" />
								<YAxis yAxisId="right" orientation="right" />
								<Tooltip
									formatter={(value, name) => [
										value,
										name === "totalCustomers"
											? "Total Customers"
											: "Total Orders"
									]}
								/>
								<Legend />
								<Line
									yAxisId="left"
									type="monotone"
									dataKey="totalCustomers"
									stroke="#3b82f6"
									strokeWidth={3}
									dot={{ fill: "#3b82f6", strokeWidth: 2, r: 4 }}
									name="Total Customers"
								/>
								<Line
									yAxisId="right"
									type="monotone"
									dataKey="totalOrders"
									stroke="#10b981"
									strokeWidth={3}
									dot={{ fill: "#10b981", strokeWidth: 2, r: 4 }}
									name="Total Orders"
								/>
							</LineChart>
						</ResponsiveContainer>
					</div>

					{/* Low Stock Alert */}
					<div className="rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
						<h3 className="text-lg font-semibold mb-4">Low Stock Alerts</h3>
						<div className="space-y-3">
							{[
								{ product: "Wireless Mouse", stock: 5, minStock: 20 },
								{ product: "Keyboard Cover", stock: 8, minStock: 25 },
								{ product: "Screen Protector", stock: 12, minStock: 30 },
								{ product: "Phone Case", stock: 3, minStock: 15 }
							].map((item, index) => (
								<div
									key={index}
									className="flex items-center justify-between p-3 bg-red-50 dark:bg-red-900/20 rounded-lg"
								>
									<div>
										<p className="text-sm font-medium">{item.product}</p>
										<p className="text-xs text-muted-foreground">
											{item.stock} units left (min: {item.minStock})
										</p>
									</div>
									<div className="text-red-600 dark:text-red-400">
										<svg
											className="w-5 h-5"
											fill="currentColor"
											viewBox="0 0 20 20"
										>
											<path
												fillRule="evenodd"
												d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z"
												clipRule="evenodd"
											/>
										</svg>
									</div>
								</div>
							))}
						</div>
					</div>
				</div>

				{/* Recent Orders Table */}
				<div className="rounded-lg border bg-card p-6 text-card-foreground shadow-sm">
					<h3 className="text-lg font-semibold mb-4">Recent Orders</h3>
					<div className="overflow-x-auto">
						<table className="w-full text-sm">
							<thead>
								<tr className="border-b">
									<th className="text-left p-2">Order ID</th>
									<th className="text-left p-2">Customer</th>
									<th className="text-left p-2">Status</th>
									<th className="text-left p-2">Total</th>
									<th className="text-left p-2">Date</th>
								</tr>
							</thead>
							<tbody>
								{[
									{
										id: "#1234",
										customer: "John Doe",
										status: "Delivered",
										total: "$245.00",
										date: "2024-01-15"
									},
									{
										id: "#1235",
										customer: "Jane Smith",
										status: "InTransit",
										total: "$189.50",
										date: "2024-01-14"
									},
									{
										id: "#1236",
										customer: "Bob Johnson",
										status: "Paid",
										total: "$312.75",
										date: "2024-01-14"
									},
									{
										id: "#1237",
										customer: "Alice Brown",
										status: "PendingForPayment",
										total: "$98.25",
										date: "2024-01-13"
									},
									{
										id: "#1238",
										customer: "Mike Wilson",
										status: "Created",
										total: "$156.80",
										date: "2024-01-13"
									}
								].map((order, index) => (
									<tr key={index} className="border-b hover:bg-muted/50">
										<td className="p-2 font-medium">{order.id}</td>
										<td className="p-2">{order.customer}</td>
										<td className="p-2">
											<span
												className={`px-2 py-1 rounded-full text-xs ${
													order.status === "Delivered"
														? "bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-400"
														: order.status === "InTransit"
															? "bg-purple-100 text-purple-800 dark:bg-purple-900/20 dark:text-purple-400"
															: order.status === "Paid"
																? "bg-blue-100 text-blue-800 dark:bg-blue-900/20 dark:text-blue-400"
																: order.status ===
																	  "PendingForPayment"
																	? "bg-yellow-100 text-yellow-800 dark:bg-yellow-900/20 dark:text-yellow-400"
																	: order.status === "Created"
																		? "bg-gray-100 text-gray-800 dark:bg-gray-900/20 dark:text-gray-400"
																		: "bg-red-100 text-red-800 dark:bg-red-900/20 dark:text-red-400"
												}`}
											>
												{order.status}
											</span>
										</td>
										<td className="p-2">{order.total}</td>
										<td className="p-2">{order.date}</td>
									</tr>
								))}
							</tbody>
						</table>
					</div>
				</div>
			</div>
		</AuthGuard>
	);
};