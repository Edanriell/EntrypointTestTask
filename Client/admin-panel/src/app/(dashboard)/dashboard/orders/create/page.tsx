import { Fragment } from "react";
import { PlusCircle } from "lucide-react";

import { Button } from "@shared/ui/button";
import {
	Card,
	CardContent,
	CardDescription,
	CardFooter,
	CardHeader,
	CardTitle
} from "@shared/ui/card";
import { Label } from "@shared/ui/label";
import { Input } from "@shared/ui/input";
import { Textarea } from "@shared/ui/textarea";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@shared/ui/table";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@shared/ui/select";

export default function Dashboard() {
	return (
		<Fragment>
			<div className="flex items-center gap-4">
				<h1 className="flex-1 shrink-0 whitespace-nowrap text-xl font-semibold tracking-tight sm:grow-0">
					Create New Order
				</h1>
				<div className="hidden items-center gap-2 md:ml-auto md:flex">
					<Button variant="outline" size="sm">
						Cancel
					</Button>
					<Button size="sm">Create Order</Button>
				</div>
			</div>
			<div className="grid gap-4 md:grid-cols-[1fr_250px] lg:grid-cols-3 lg:gap-8">
				<div className="grid auto-rows-max items-start gap-4 lg:col-span-2 lg:gap-8">
					<Card x-chunk="dashboard-07-chunk-0">
						<CardHeader>
							<CardTitle>Order Details</CardTitle>
							<CardDescription>Lipsum dolor sit amet, consectetur adipiscing elit</CardDescription>
						</CardHeader>
						<CardContent>
							<div className="grid gap-6">
								<div className="grid gap-3">
									<Label htmlFor="name">UserId</Label>
									<Input
										id="name"
										type="text"
										className="w-full"
										defaultValue="Gamer Gear Pro Controller"
									/>
								</div>
								<div className="grid gap-3">
									<Label htmlFor="description">Ship Address</Label>
									<Textarea
										id="description"
										defaultValue="Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam auctor, nisl nec ultricies ultricies, nunc nisl ultricies nunc, nec ultricies nunc nisl nec nunc."
										className="min-h-32"
									/>
								</div>
								<div className="grid gap-3">
									<Label htmlFor="description">Order Information</Label>
									<Textarea
										id="description"
										defaultValue="Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam auctor, nisl nec ultricies ultricies, nunc nisl ultricies nunc, nec ultricies nunc nisl nec nunc."
										className="min-h-32"
									/>
								</div>
							</div>
						</CardContent>
					</Card>
					<Card x-chunk="dashboard-07-chunk-1">
						<CardHeader>
							<CardTitle>Products</CardTitle>
							<CardDescription>Lipsum dolor sit amet, consectetur adipiscing elit</CardDescription>
						</CardHeader>
						<CardContent>
							<Table>
								<TableHeader>
									<TableRow>
										<TableHead>Product Id</TableHead>
										<TableHead>Quantity</TableHead>
									</TableRow>
								</TableHeader>
								<TableBody>
									<TableRow>
										<TableCell>
											<Label htmlFor="stock-1" className="sr-only">
												Stock
											</Label>
											<Input id="stock-1" type="number" defaultValue="100" />
										</TableCell>
										<TableCell>
											<Label htmlFor="price-1" className="sr-only">
												Price
											</Label>
											<Input id="price-1" type="number" defaultValue="99.99" />
										</TableCell>
									</TableRow>
									<TableRow>
										<TableCell>
											<Label htmlFor="stock-2" className="sr-only">
												Stock
											</Label>
											<Input id="stock-2" type="number" defaultValue="143" />
										</TableCell>
										<TableCell>
											<Label htmlFor="price-2" className="sr-only">
												Price
											</Label>
											<Input id="price-2" type="number" defaultValue="99.99" />
										</TableCell>
									</TableRow>
									<TableRow>
										<TableCell>
											<Label htmlFor="stock-3" className="sr-only">
												Stock
											</Label>
											<Input id="stock-3" type="number" defaultValue="32" />
										</TableCell>
										<TableCell>
											<Label htmlFor="price-3" className="sr-only">
												Stock
											</Label>
											<Input id="price-3" type="number" defaultValue="99.99" />
										</TableCell>
									</TableRow>
								</TableBody>
							</Table>
						</CardContent>
						<CardFooter className="justify-center border-t p-4">
							<Button size="sm" variant="ghost" className="gap-1">
								<PlusCircle className="h-3.5 w-3.5" />
								Add Product
							</Button>
						</CardFooter>
					</Card>
				</div>
				<div className="grid auto-rows-max items-start gap-4 lg:gap-8">
					<Card x-chunk="dashboard-07-chunk-3">
						<CardHeader>
							<CardTitle>Order Status</CardTitle>
						</CardHeader>
						<CardContent>
							<div className="grid gap-6">
								<div className="grid gap-3">
									<Label htmlFor="status">Status</Label>
									<Select>
										<SelectTrigger id="status" aria-label="Select status">
											<SelectValue placeholder="Select status" />
										</SelectTrigger>
										<SelectContent>
											<SelectItem value="draft">Created</SelectItem>
										</SelectContent>
									</Select>
								</div>
							</div>
						</CardContent>
					</Card>
				</div>
			</div>
			<div className="flex items-center justify-center gap-2 md:hidden">
				<Button variant="outline" size="sm">
					Discard
				</Button>
				<Button size="sm">Save Product</Button>
			</div>
		</Fragment>
	);
}
