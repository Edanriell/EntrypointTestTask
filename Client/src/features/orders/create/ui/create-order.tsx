import { FC, useEffect, useState } from "react";
import { Plus, SquarePen, User, X } from "lucide-react";
import { zodResolver } from "@hookform/resolvers/zod";
import { Controller, useForm } from "react-hook-form";
import { AnimatePresence, motion } from "motion/react";
import { useQuery } from "@tanstack/react-query";

import { productsQueries } from "@entities/products";
import { usersQueries } from "@entities/users";

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
import { Spinner } from "@shared/ui/spinner";
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

import { useCreateOrder } from "../api";
import { CreateOrderFormData, createOrderSchema } from "../model";

type OrderProduct = {
	id: string;
	productId: string | null;
	quantity: number;
};

export const CreateOrder: FC = () => {
	const [orderProducts, setOrderProducts] = useState<OrderProduct[]>([
		{ id: "1", productId: null, quantity: 1 }
	]);
	const [openProductSelector, setOpenProductSelector] = useState<string | null>(null);
	const [openClientSelector, setOpenClientSelector] = useState(false);

	// Search states
	const [customerSearchTerm, setCustomerSearchTerm] = useState("");
	const [productSearchTerm, setProductSearchTerm] = useState("");

	const {
		control,
		register,
		handleSubmit,
		formState: { errors, isSubmitting, isValid },
		reset,
		setError,
		watch,
		setValue,
		trigger
	} = useForm<CreateOrderFormData>({
		resolver: zodResolver(createOrderSchema),
		mode: "onTouched",
		defaultValues: {
			customerId: undefined,
			country: "",
			city: "",
			zipCode: "",
			street: "",
			orderInfo: "",
			orderProducts: []
		}
	});

	const selectedCustomerId = watch("customerId");

	// Dynamic customer search query
	const customersQuery = useQuery({
		...usersQueries.customersList({
			pageSize: 100, // Increased page size
			nameFilter: customerSearchTerm || null,
			emailFilter: customerSearchTerm || null
		})
	});

	// Dynamic product search query
	const productsQuery = useQuery({
		...productsQueries.productsList({
			pageSize: 100, // Increased page size
			nameFilter: productSearchTerm || null,
			descriptionFilter: productSearchTerm || null
		})
	});

	const {
		data: customersData,
		isLoading: isLoadingCustomers,
		error: customersError
	} = customersQuery;
	const { data: productsData, isLoading: isLoadingProducts } = productsQuery;

	const products = productsData?.products ?? [];
	const customers = customersData?.customers || [];

	const { mutateAsync: createOrder, isPending } = useCreateOrder(reset, setError);

	const selectedCustomer = customers.find((customer) => customer.id === selectedCustomerId);

	// Check if form can be submitted
	const hasValidProducts = orderProducts.some((op) => op.productId !== null);
	const canSubmit = Boolean(
		selectedCustomerId && hasValidProducts && !isSubmitting && !isPending
	);

	const addOrderProduct = () => {
		const newProduct: OrderProduct = {
			id: Date.now().toString(),
			productId: null,
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

	const selectProduct = (orderProductId: string, productId: string) => {
		updateOrderProduct(orderProductId, { productId });
		setOpenProductSelector(null);
		setProductSearchTerm(""); // Reset search term
		trigger("orderProducts");
	};

	const selectClient = (customerId: number) => {
		setValue("customerId", customerId, { shouldValidate: true });

		const customer = customers.find((c) => c.id === customerId);
		if (customer) {
			setValue("country", customer.country || "");
			setValue("city", customer.city || "");
			setValue("zipCode", customer.zipCode || "");
			setValue("street", customer.street || "");
		}

		setOpenClientSelector(false);
		setCustomerSearchTerm(""); // Reset search term
		trigger();
	};

	const onSubmit = async (data: CreateOrderFormData) => {
		try {
			// Convert orderProducts to the format expected by the API
			const orderItems = orderProducts
				.filter((op) => op.productId !== null)
				.map((op) => ({
					productId: op.productId!,
					quantity: op.quantity
				}));

			// Transform form data to match API expectations
			const orderData = {
				clientId: data.customerId, // Convert customerId to clientId
				shippingAddress: {
					country: data.country,
					city: data.city,
					zipCode: data.zipCode,
					street: data.street
				},
				currency: "USD", // You might want to make this dynamic
				info: data.orderInfo,
				orderItems: orderItems
			};

			await createOrder(orderData);
		} catch (error) {
			console.error("Error creating order:", error);
		}
	};

	// Update form validation when orderProducts change
	useEffect(() => {
		const formattedOrderProducts = orderProducts
			.filter((op) => op.productId !== null)
			.map((op) => ({
				productId: op.productId!,
				quantity: op.quantity
			}));

		setValue("orderProducts", formattedOrderProducts, { shouldValidate: true });
	}, [orderProducts, setValue]);

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
				<form onSubmit={handleSubmit(onSubmit)}>
					<div className="grid flex-1 auto-rows-min gap-6 px-4 mb-10">
						{/* Client Selection */}
						<div className="space-y-4">
							<h3 className="text-lg font-medium">Client</h3>
							<div className="grid gap-2 relative">
								<Label>Select Client *</Label>
								<Controller
									name="customerId"
									control={control}
									rules={{ required: "Please select a client" }}
									render={({ field }) => (
										<Popover
											open={openClientSelector}
											onOpenChange={setOpenClientSelector}
										>
											<PopoverTrigger asChild>
												<Button
													variant="outline"
													role="combobox"
													className={`justify-between w-full ${
														errors.customerId ? "border-red-500" : ""
													}`}
												>
													{selectedCustomer
														? `${selectedCustomer.firstName} ${selectedCustomer.lastName}`
														: "Select client..."}
													<User className="ml-2 h-4 w-4 shrink-0 opacity-50" />
												</Button>
											</PopoverTrigger>
											<PopoverContent className="w-full p-0">
												<Command>
													<CommandInput
														placeholder="Search clients..."
														value={customerSearchTerm}
														onValueChange={setCustomerSearchTerm}
													/>
													<CommandList>
														<CommandEmpty>
															{isLoadingCustomers
																? "Loading..."
																: "No clients found."}
														</CommandEmpty>
														<CommandGroup>
															{customers.map((client) => (
																<CommandItem
																	key={client.id}
																	value={`${client.firstName} ${client.lastName} ${client.email}`}
																	onSelect={() =>
																		selectClient(client.id)
																	}
																>
																	<div className="flex flex-col w-full">
																		<span className="font-medium">
																			{client.firstName}{" "}
																			{client.lastName}
																		</span>
																		<span className="text-sm text-muted-foreground">
																			{client.email} |{" "}
																			{client.phoneNumber}
																		</span>
																		<span className="text-xs text-muted-foreground">
																			{client.street},{" "}
																			{client.city},{" "}
																			{client.country}{" "}
																			{client.zipCode}
																		</span>
																	</div>
																</CommandItem>
															))}
														</CommandGroup>
													</CommandList>
												</Command>
											</PopoverContent>
										</Popover>
									)}
								/>
								<AnimatePresence>
									{errors.customerId && (
										<motion.p
											initial={{
												opacity: 0,
												x: -15,
												filter: "blur(0.24rem)"
											}}
											animate={{ opacity: 1, x: 0, filter: "blur(0)" }}
											exit={{ opacity: 0, x: 15, filter: "blur(0.24rem)" }}
											className="text-sm text-red-500 absolute bottom-[-1.5rem]"
										>
											{errors.customerId.message}
										</motion.p>
									)}
								</AnimatePresence>
							</div>

							{/* Selected Client Info */}
							{selectedCustomer && (
								<div className="bg-muted/50 p-3 rounded-lg">
									<div className="text-sm space-y-1">
										<div>
											<strong>Name:</strong> {selectedCustomer.firstName}{" "}
											{selectedCustomer.lastName}
										</div>
										<div>
											<strong>Email:</strong> {selectedCustomer.email}
										</div>
										<div>
											<strong>Phone:</strong> {selectedCustomer.phoneNumber}
										</div>
										<div>
											<strong>Address:</strong> {selectedCustomer.street},{" "}
											{selectedCustomer.city}, {selectedCustomer.country}{" "}
											{selectedCustomer.zipCode}
										</div>
									</div>
								</div>
							)}
						</div>

						{/* Shipping Address Section */}
						<div className="space-y-4">
							<h3 className="text-lg font-medium">Shipping Address</h3>
							<div className="grid grid-cols-2 gap-3">
								<div className="grid gap-2 relative">
									<Label htmlFor="country">Country *</Label>
									<Input
										id="country"
										placeholder="Enter country"
										{...register("country", {
											required: "Country is required"
										})}
										className={errors.country ? "border-red-500" : ""}
									/>
									<AnimatePresence>
										{errors.country && (
											<motion.p
												initial={{
													opacity: 0,
													x: -15,
													filter: "blur(0.24rem)"
												}}
												animate={{ opacity: 1, x: 0, filter: "blur(0)" }}
												exit={{
													opacity: 0,
													x: 15,
													filter: "blur(0.24rem)"
												}}
												className="text-sm text-red-500 absolute bottom-[-1.5rem]"
											>
												{errors.country.message}
											</motion.p>
										)}
									</AnimatePresence>
								</div>
								<div className="grid gap-2 relative">
									<Label htmlFor="city">City *</Label>
									<Input
										id="city"
										placeholder="Enter city"
										{...register("city", { required: "City is required" })}
										className={errors.city ? "border-red-500" : ""}
									/>
									<AnimatePresence>
										{errors.city && (
											<motion.p
												initial={{
													opacity: 0,
													x: -15,
													filter: "blur(0.24rem)"
												}}
												animate={{ opacity: 1, x: 0, filter: "blur(0)" }}
												exit={{
													opacity: 0,
													x: 15,
													filter: "blur(0.24rem)"
												}}
												className="text-sm text-red-500 absolute bottom-[-1.5rem]"
											>
												{errors.city.message}
											</motion.p>
										)}
									</AnimatePresence>
								</div>
							</div>
							<div className="grid grid-cols-2 gap-3">
								<div className="grid gap-2 relative">
									<Label htmlFor="zipCode">Zip Code *</Label>
									<Input
										id="zipCode"
										placeholder="Enter zip code"
										{...register("zipCode", {
											required: "Zip code is required"
										})}
										className={errors.zipCode ? "border-red-500" : ""}
									/>
									<AnimatePresence>
										{errors.zipCode && (
											<motion.p
												initial={{
													opacity: 0,
													x: -15,
													filter: "blur(0.24rem)"
												}}
												animate={{ opacity: 1, x: 0, filter: "blur(0)" }}
												exit={{
													opacity: 0,
													x: 15,
													filter: "blur(0.24rem)"
												}}
												className="text-sm text-red-500 absolute bottom-[-1.5rem]"
											>
												{errors.zipCode.message}
											</motion.p>
										)}
									</AnimatePresence>
								</div>
								<div className="grid gap-2 relative">
									<Label htmlFor="street">Street *</Label>
									<Input
										id="street"
										placeholder="Enter street address"
										{...register("street", { required: "Street is required" })}
										className={errors.street ? "border-red-500" : ""}
									/>
									<AnimatePresence>
										{errors.street && (
											<motion.p
												initial={{
													opacity: 0,
													x: -15,
													filter: "blur(0.24rem)"
												}}
												animate={{ opacity: 1, x: 0, filter: "blur(0)" }}
												exit={{
													opacity: 0,
													x: 15,
													filter: "blur(0.24rem)"
												}}
												className="text-sm text-red-500 absolute bottom-[-1.5rem]"
											>
												{errors.street.message}
											</motion.p>
										)}
									</AnimatePresence>
								</div>
							</div>
						</div>

						{/* Order Information */}
						<div className="grid gap-2 relative">
							<Label htmlFor="orderInfo">Order Information</Label>
							<Textarea
								id="orderInfo"
								placeholder="Additional order information or notes"
								className={`resize-none ${errors.orderInfo ? "border-red-500" : ""}`}
								{...register("orderInfo")}
							/>
							<AnimatePresence>
								{errors.orderInfo && (
									<motion.p
										initial={{
											opacity: 0,
											x: -15,
											filter: "blur(0.24rem)"
										}}
										animate={{ opacity: 1, x: 0, filter: "blur(0)" }}
										exit={{ opacity: 0, x: 15, filter: "blur(0.24rem)" }}
										className="text-sm text-red-500 absolute bottom-[-1.5rem]"
									>
										{errors.orderInfo.message}
									</motion.p>
								)}
							</AnimatePresence>
						</div>

						{/* Products Section */}
						<div className="space-y-4">
							<div className="flex items-center justify-between">
								<h3 className="text-lg font-medium">Products *</h3>
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

							{!hasValidProducts && (
								<p className="text-sm text-red-500">
									Please select at least one product
								</p>
							)}

							<div className="space-y-4">
								{orderProducts.map((orderProduct, index) => {
									const selectedProduct = products.find(
										(p) => p.id === orderProduct.productId
									);
									return (
										<div
											key={orderProduct.id}
											className="flex items-end gap-3 p-4 border rounded-lg"
										>
											<div className="flex-1 grid gap-2">
												<Label>Product {index + 1} *</Label>
												<Popover
													open={openProductSelector === orderProduct.id}
													onOpenChange={(open) => {
														setOpenProductSelector(
															open ? orderProduct.id : null
														);
														if (!open) {
															setProductSearchTerm("");
														}
													}}
												>
													<PopoverTrigger asChild>
														<Button
															variant="outline"
															role="combobox"
															className="justify-between w-full"
														>
															{selectedProduct
																? selectedProduct.name
																: "Select product..."}
														</Button>
													</PopoverTrigger>
													<PopoverContent className="w-full max-w-[400px] p-0">
														<Command>
															<CommandInput
																placeholder="Search products..."
																value={productSearchTerm}
																onValueChange={setProductSearchTerm}
															/>
															<CommandList>
																<CommandEmpty>
																	{isLoadingProducts
																		? "Loading..."
																		: "No products found."}
																</CommandEmpty>
																<CommandGroup>
																	{products.map((product) => (
																		<CommandItem
																			key={product.id}
																			value={`${product.name} ${product.description}`}
																			onSelect={() =>
																				selectProduct(
																					orderProduct.id,
																					product.id
																				)
																			}
																		>
																			<div className="flex flex-col">
																				<span className="font-medium">
																					{product.name}
																				</span>
																				<span className="text-sm text-muted-foreground">
																					Price: $
																					{product.price}{" "}
																					{
																						product.currency
																					}{" "}
																					| Available:{" "}
																					{
																						product.available
																					}{" "}
																					|
																					{product.isInStock
																						? " In Stock"
																						: " Out of Stock"}
																				</span>
																				{product.description && (
																					<span className="text-xs text-muted-foreground mt-1">
																						{
																							product.description
																						}
																					</span>
																				)}
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
													onClick={() =>
														removeOrderProduct(orderProduct.id)
													}
													className="text-red-600 hover:text-red-700"
												>
													<X className="h-4 w-4" />
												</Button>
											)}
										</div>
									);
								})}
							</div>
						</div>

						{/* Order Summary */}
						<div className="bg-muted/50 p-4 rounded-lg">
							<h3 className="font-medium mb-2">Order Summary</h3>
							{selectedCustomer && (
								<div className="mb-3 pb-2 border-b text-sm">
									<strong>Client:</strong> {selectedCustomer.firstName}{" "}
									{selectedCustomer.lastName}
								</div>
							)}
							<div className="space-y-1">
								{orderProducts.map((orderProduct) => {
									const product = products.find(
										(p) => p.id === orderProduct.productId
									);
									return product ? (
										<div
											key={orderProduct.id}
											className="flex justify-between text-sm"
										>
											<span>
												{product.name} x {orderProduct.quantity}
											</span>
											<span>
												$
												{(product.price * orderProduct.quantity).toFixed(2)}{" "}
												{product.currency}
											</span>
										</div>
									) : null;
								})}
								<Separator className="my-2" />
								<div className="flex justify-between font-medium">
									<span>Total:</span>
									<span>
										$
										{orderProducts
											.reduce((total, op) => {
												const product = products.find(
													(p) => p.id === op.productId
												);
												return (
													total +
													(product ? product.price * op.quantity : 0)
												);
											}, 0)
											.toFixed(2)}
									</span>
								</div>
							</div>
						</div>
					</div>
					<SheetFooter>
						<Button type="submit" disabled={!canSubmit}>
							{isSubmitting || isPending ? (
								<>
									<Spinner />
									<span>Creating...</span>
								</>
							) : (
								<span>Create New</span>
							)}
						</Button>
						<SheetClose asChild>
							<Button variant="outline">Cancel</Button>
						</SheetClose>
					</SheetFooter>
				</form>
			</SheetContent>
		</Sheet>
	);
};
