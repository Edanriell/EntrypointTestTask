import { FC, useState } from "react";
import { Plus, SquarePen, User, X } from "lucide-react";

import { Button } from "@shared/ui/button";
import {
	Sheet,
	SheetClose,
	SheetContent,
	SheetDescription,
	SheetFooter,
	SheetHeader,
	SheetTitle,
	SheetTrigger
} from "@shared/ui/sheet";
import { Label } from "@shared/ui/label";
import { Input } from "@shared/ui/input";
import { Textarea } from "@shared/ui/textarea";
import {
	Command,
	CommandEmpty,
	CommandGroup,
	CommandInput,
	CommandItem,
	CommandList
} from "@shared/ui/command";
import { Popover, PopoverContent, PopoverTrigger } from "@shared/ui/popover";
import { Separator } from "@shared/ui/separator";

// import { Product } from "@shared/model/product";
// import { User as UserType } from "@shared/model/user";

type Product = any;
type UserType = any;

type OrderProduct = {
	id: string;
	product: Product | null;
	quantity: number;
};

export const CreateOrder: FC = () => {
	const [selectedClient, setSelectedClient] = useState<UserType | null>(null);
	const [orderProducts, setOrderProducts] = useState<OrderProduct[]>([
		{ id: "1", product: null, quantity: 1 }
	]);
	const [openProductSelector, setOpenProductSelector] = useState<string | null>(null);
	const [openClientSelector, setOpenClientSelector] = useState(false);

	// Mock clients - replace with your actual clients data
	const mockClients: UserType[] = [
		{
			id: 1,
			firstName: "John",
			lastName: "Doe",
			email: "john.doe@example.com",
			phoneNumber: "123-456-7890",
			gender: "Male",
			country: "USA",
			city: "New York",
			zipCode: "10001",
			street: "123 Main St",
			password: "",
			orders: []
		},
		{
			id: 2,
			firstName: "Jane",
			lastName: "Smith",
			email: "jane.smith@example.com",
			phoneNumber: "987-654-3210",
			gender: "Female",
			country: "USA",
			city: "Los Angeles",
			zipCode: "90210",
			street: "456 Oak Ave",
			password: "",
			orders: []
		},
		{
			id: 3,
			firstName: "Bob",
			lastName: "Johnson",
			email: "bob.johnson@example.com",
			phoneNumber: "555-123-4567",
			gender: "Male",
			country: "Canada",
			city: "Toronto",
			zipCode: "M5V 3A8",
			street: "789 Pine St",
			password: "",
			orders: []
		}
	];

	// Mock products - replace with your actual products data
	const mockProducts: Product[] = [
		{
			id: 1,
			code: "P001",
			productName: "Product 1",
			description: "Description 1",
			unitPrice: 10.99,
			unitsInStock: 100,
			unitsOnOrder: 0,
			productOrders: []
		},
		{
			id: 2,
			code: "P002",
			productName: "Product 2",
			description: "Description 2",
			unitPrice: 25.5,
			unitsInStock: 50,
			unitsOnOrder: 10,
			productOrders: []
		},
		{
			id: 3,
			code: "P003",
			productName: "Product 3",
			description: "Description 3",
			unitPrice: 15.75,
			unitsInStock: 75,
			unitsOnOrder: 5,
			productOrders: []
		}
	];

	const addOrderProduct = () => {
		const newProduct: OrderProduct = {
			id: Date.now().toString(),
			product: null,
			quantity: 1
		};
		setOrderProducts([...orderProducts, newProduct]);
	};

	const removeOrderProduct = (id: string) => {
		if (orderProducts.length > 1) {
			setOrderProducts(orderProducts.filter((op) => op.id !== id));
		}
	};

	const updateOrderProduct = (id: string, updates: Partial<OrderProduct>) => {
		setOrderProducts(orderProducts.map((op) => (op.id === id ? { ...op, ...updates } : op)));
	};

	const selectProduct = (orderProductId: string, product: Product) => {
		updateOrderProduct(orderProductId, { product });
		setOpenProductSelector(null);
	};

	const selectClient = (client: UserType) => {
		setSelectedClient(client);
		setOpenClientSelector(false);
	};

	return (
		<Sheet>
			<SheetTrigger asChild>
				<Button className="flex w-fit self-end">
					<SquarePen />
					Create new
				</Button>
			</SheetTrigger>
			<SheetContent className="overflow-y-auto w-full sm:max-w-[36%]">
				<SheetHeader>
					<SheetTitle>Create order</SheetTitle>
					<SheetDescription>
						Fill in the details below to create a new order. Click save when you're
						done.
					</SheetDescription>
				</SheetHeader>

				<div className="grid flex-1 auto-rows-min gap-6 px-4">
					{/* Client Selection */}
					<div className="space-y-4">
						<h3 className="text-lg font-medium">Client</h3>
						<div className="grid gap-2">
							<Label>Select Client</Label>
							<Popover open={openClientSelector} onOpenChange={setOpenClientSelector}>
								<PopoverTrigger asChild>
									<Button
										variant="outline"
										role="combobox"
										className="justify-between w-full"
									>
										{selectedClient
											? `${selectedClient.firstName} ${selectedClient.lastName}`
											: "Select client..."}
										<User className="ml-2 h-4 w-4 shrink-0 opacity-50" />
									</Button>
								</PopoverTrigger>
								<PopoverContent className="w-full p-0">
									<Command>
										<CommandInput placeholder="Search clients..." />
										<CommandList>
											<CommandEmpty>No clients found.</CommandEmpty>
											<CommandGroup>
												{mockClients.map((client) => (
													<CommandItem
														key={client.id}
														value={`${client.firstName} ${client.lastName} ${client.email}`}
														onSelect={() => selectClient(client)}
													>
														<div className="flex flex-col w-full">
															<span className="font-medium">
																{client.firstName} {client.lastName}
															</span>
															<span className="text-sm text-muted-foreground">
																{client.email} |{" "}
																{client.phoneNumber}
															</span>
															<span className="text-xs text-muted-foreground">
																{client.street}, {client.city},{" "}
																{client.country} {client.zipCode}
															</span>
														</div>
													</CommandItem>
												))}
											</CommandGroup>
										</CommandList>
									</Command>
								</PopoverContent>
							</Popover>
						</div>

						{/* Selected Client Info */}
						{selectedClient && (
							<div className="bg-muted/50 p-3 rounded-lg">
								<div className="text-sm space-y-1">
									<div>
										<strong>Name:</strong> {selectedClient.firstName}{" "}
										{selectedClient.lastName}
									</div>
									<div>
										<strong>Email:</strong> {selectedClient.email}
									</div>
									<div>
										<strong>Phone:</strong> {selectedClient.phoneNumber}
									</div>
									<div>
										<strong>Address:</strong> {selectedClient.street},{" "}
										{selectedClient.city}, {selectedClient.country}{" "}
										{selectedClient.zipCode}
									</div>
								</div>
							</div>
						)}
					</div>

					<Separator />

					{/* Shipping Address Section */}
					<div className="space-y-4">
						<h3 className="text-lg font-medium">Shipping Address</h3>
						<div className="grid grid-cols-2 gap-3">
							<div className="grid gap-2">
								<Label htmlFor="country">Country</Label>
								<Input
									id="country"
									placeholder="Enter country"
									defaultValue={selectedClient?.country || ""}
								/>
							</div>
							<div className="grid gap-2">
								<Label htmlFor="city">City</Label>
								<Input
									id="city"
									placeholder="Enter city"
									defaultValue={selectedClient?.city || ""}
								/>
							</div>
						</div>
						<div className="grid grid-cols-2 gap-3">
							<div className="grid gap-2">
								<Label htmlFor="zipcode">Zip Code</Label>
								<Input
									id="zipcode"
									placeholder="Enter zip code"
									defaultValue={selectedClient?.zipCode || ""}
								/>
							</div>
							<div className="grid gap-2">
								<Label htmlFor="street">Street</Label>
								<Input
									id="street"
									placeholder="Enter street address"
									defaultValue={selectedClient?.street || ""}
								/>
							</div>
						</div>
					</div>

					<Separator />

					{/* Order Information */}
					<div className="grid gap-2">
						<Label htmlFor="order-info">Order Information</Label>
						<Textarea
							id="order-info"
							placeholder="Additional order information or notes"
							className="resize-none"
						/>
					</div>

					<Separator />

					{/* Products Section */}
					<div className="space-y-4">
						<div className="flex items-center justify-between">
							<h3 className="text-lg font-medium">Products</h3>
							<Button
								type="button"
								variant="outline"
								size="sm"
								onClick={addOrderProduct}
							>
								<Plus className="h-4 w-4 mr-2" />
								Add Product
							</Button>
						</div>

						<div className="space-y-4">
							{orderProducts.map((orderProduct, index) => (
								<div
									key={orderProduct.id}
									className="flex items-end gap-3 p-4 border rounded-lg"
								>
									<div className="flex-1 grid gap-2">
										<Label>Product {index + 1}</Label>
										<Popover
											open={openProductSelector === orderProduct.id}
											onOpenChange={(open) =>
												setOpenProductSelector(
													open ? orderProduct.id : null
												)
											}
										>
											<PopoverTrigger asChild>
												<Button
													variant="outline"
													role="combobox"
													className="justify-between w-full"
												>
													{orderProduct.product
														? orderProduct.product.productName
														: "Select product..."}
												</Button>
											</PopoverTrigger>
											<PopoverContent className="w-full p-0">
												<Command>
													<CommandInput placeholder="Search products..." />
													<CommandList>
														<CommandEmpty>
															No products found.
														</CommandEmpty>
														<CommandGroup>
															{mockProducts.map((product) => (
																<CommandItem
																	key={product.id}
																	value={product.productName}
																	onSelect={() =>
																		selectProduct(
																			orderProduct.id,
																			product
																		)
																	}
																>
																	<div className="flex flex-col">
																		<span className="font-medium">
																			{product.productName}
																		</span>
																		<span className="text-sm text-muted-foreground">
																			Code: {product.code} |
																			Price: $
																			{product.unitPrice} |
																			Stock:{" "}
																			{product.unitsInStock}
																		</span>
																	</div>
																</CommandItem>
															))}
														</CommandGroup>
													</CommandList>
												</Command>
											</PopoverContent>
										</Popover>
									</div>

									<div className="grid gap-2 w-24">
										<Label htmlFor={`quantity-${orderProduct.id}`}>
											Quantity
										</Label>
										<Input
											id={`quantity-${orderProduct.id}`}
											type="number"
											min="1"
											value={orderProduct.quantity}
											onChange={(e) =>
												updateOrderProduct(orderProduct.id, {
													quantity: Math.max(
														1,
														parseInt(e.target.value) || 1
													)
												})
											}
											className="text-center"
										/>
									</div>

									{orderProducts.length > 1 && (
										<Button
											type="button"
											variant="outline"
											size="sm"
											onClick={() => removeOrderProduct(orderProduct.id)}
											className="text-red-600 hover:text-red-700"
										>
											<X className="h-4 w-4" />
										</Button>
									)}
								</div>
							))}
						</div>
					</div>

					{/* Order Summary */}
					<div className="bg-muted/50 p-4 rounded-lg">
						<h3 className="font-medium mb-2">Order Summary</h3>
						{selectedClient && (
							<div className="mb-3 pb-2 border-b text-sm">
								<strong>Client:</strong> {selectedClient.firstName}{" "}
								{selectedClient.lastName}
							</div>
						)}
						<div className="space-y-1">
							{orderProducts.map(
								(orderProduct, index) =>
									orderProduct.product && (
										<div
											key={orderProduct.id}
											className="flex justify-between text-sm"
										>
											<span>
												{orderProduct.product.productName} x{" "}
												{orderProduct.quantity}
											</span>
											<span>
												$
												{(
													orderProduct.product.unitPrice *
													orderProduct.quantity
												).toFixed(2)}
											</span>
										</div>
									)
							)}
							<Separator className="my-2" />
							<div className="flex justify-between font-medium">
								<span>Total:</span>
								<span>
									$
									{orderProducts
										.reduce(
											(total, op) =>
												total +
												(op.product
													? op.product.unitPrice * op.quantity
													: 0),
											0
										)
										.toFixed(2)}
								</span>
							</div>
						</div>
					</div>
				</div>

				<SheetFooter>
					<Button type="submit" disabled={!selectedClient}>
						Create New
					</Button>
					<SheetClose asChild>
						<Button variant="outline">Cancel</Button>
					</SheetClose>
				</SheetFooter>
			</SheetContent>
		</Sheet>
	);
};
