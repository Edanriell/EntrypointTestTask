import { FC } from "react";
import { Copy, CreditCard, MoreVertical } from "lucide-react";

import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuSeparator,
	DropdownMenuTrigger
} from "@shared/ui/dropdown";
import { Separator } from "@shared/ui/separator";
import { Drawer, DrawerContent } from "@shared/ui/drawer";
import {
	Card,
	CardContent,
	CardDescription,
	CardFooter,
	CardHeader,
	CardTitle
} from "@shared/ui/card";
import { Button } from "@shared/ui/button";
import { ScrollArea } from "@shared/ui/scroll-area";

type OrderDrawerProps = {
	onDrawerClose: () => void;
	order: any;
	isOrderDrawerOpen: boolean;
};

export const OrderDrawer: FC<OrderDrawerProps> = ({ isOrderDrawerOpen, order, onDrawerClose }) => {
	return (
		<Drawer direction="right" open={isOrderDrawerOpen} onClose={onDrawerClose}>
			<DrawerContent className="h-full w-[480px]">
				<ScrollArea>
					<Card className="overflow-hidden border-none" x-chunk="dashboard-05-chunk-4">
						<CardHeader className="flex flex-row items-start pt-6 pl-12 pr-6 pb-6">
							<div className="grid gap-0.5">
								<CardTitle className="group flex items-center gap-2 text-lg">
									Order Id: {order?.id}
									<Button
										size="icon"
										variant="outline"
										className="h-6 w-6 opacity-0 transition-opacity group-hover:opacity-100"
									>
										<Copy className="h-3 w-3" />
										<span className="sr-only">Copy Order ID</span>
									</Button>
								</CardTitle>
								<CardDescription>{order?.createdAt}</CardDescription>
							</div>
							<div className="ml-auto flex items-center gap-1">
								<DropdownMenu>
									<DropdownMenuTrigger asChild>
										<Button size="icon" variant="outline" className="h-8 w-8">
											<MoreVertical className="h-3.5 w-3.5" />
											<span className="sr-only">More</span>
										</Button>
									</DropdownMenuTrigger>
									<DropdownMenuContent align="end">
										<DropdownMenuItem>Edit</DropdownMenuItem>
										<DropdownMenuSeparator />
										<DropdownMenuItem>Delete</DropdownMenuItem>
									</DropdownMenuContent>
								</DropdownMenu>
							</div>
						</CardHeader>
						<CardContent className="pt-6 pl-12 pr-6 pb-6 text-sm">
							<div className="grid gap-3">
								<div className="font-semibold">Order Details</div>
								<ul className="grid gap-3">
									{order?.products.length >= 1 &&
										order.products.map((product: any, index: number) => (
											<li key={index} className="flex items-center justify-between">
												<span className="text-muted-foreground">
													{product.productName} x <span>{product.quantity}</span>
												</span>
												<span>${product.quantity * product.unitPrice}</span>
											</li>
										))}
								</ul>
								<Separator className="my-2" />
								<ul className="grid gap-3">
									<li className="flex items-center justify-between">
										<span className="text-muted-foreground">Subtotal</span>
										<span>$299.00</span>
									</li>
									<li className="flex items-center justify-between">
										<span className="text-muted-foreground">Shipping</span>
										<span>$5.00</span>
									</li>
									<li className="flex items-center justify-between">
										<span className="text-muted-foreground">Tax</span>
										<span>$25.00</span>
									</li>
									<li className="flex items-center justify-between font-semibold">
										<span className="text-muted-foreground">Total</span>
										<span>$329.00</span>
									</li>
								</ul>
							</div>
							<Separator className="my-4" />
							<div className="grid grid-cols-2 gap-4">
								<div className="grid gap-3">
									<div className="font-semibold">Shipping Information</div>
									<address className="grid gap-0.5 not-italic text-muted-foreground">
										<span>Liam Johnson</span>
										<span>1234 Main St.</span>
										<span>Anytown, CA 12345</span>
									</address>
								</div>
								<div className="grid auto-rows-max gap-3">
									<div className="font-semibold">Billing Information</div>
									<div className="text-muted-foreground">Same as shipping address</div>
								</div>
							</div>
							<Separator className="my-4" />
							<div className="grid gap-3">
								<div className="font-semibold">Customer Information</div>
								<dl className="grid gap-3">
									<div className="flex items-center justify-between">
										<dt className="text-muted-foreground">Customer</dt>
										<dd>
											{order?.customer.name} {order?.customer.surname}
										</dd>
									</div>
									<div className="flex items-center justify-between">
										<dt className="text-muted-foreground">Email</dt>
										<dd>
											<a href="mailto:">{order?.customer.email}</a>
										</dd>
									</div>
									<div className="flex items-center justify-between">
										<dt className="text-muted-foreground">Phone</dt>
										<dd>
											<a href="tel:">+1 234 567 890</a>
										</dd>
									</div>
								</dl>
							</div>
							<Separator className="my-4" />
							<div className="grid gap-3">
								<div className="font-semibold">Payment Information</div>
								<dl className="grid gap-3">
									<div className="flex items-center justify-between">
										<dt className="flex items-center gap-1 text-muted-foreground">
											<CreditCard className="h-4 w-4" />
											Visa
										</dt>
										<dd>**** **** **** 4532</dd>
									</div>
								</dl>
							</div>
						</CardContent>
						<CardFooter className="flex flex-row items-center border-t bg-muted/50 pt-6 pl-12 pr-6 pb-6">
							<div className="text-xs text-muted-foreground">
								Updated <time dateTime="2023-11-23">{order?.updatedAt}</time>
							</div>
						</CardFooter>
					</Card>
				</ScrollArea>
			</DrawerContent>
		</Drawer>
	);
};
