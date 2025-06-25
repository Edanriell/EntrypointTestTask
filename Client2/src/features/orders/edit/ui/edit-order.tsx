import { FC } from "react";
import { Edit } from "lucide-react";

import { DropdownMenuItem } from "@shared/ui/dropdown-menu";
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
import { Button } from "@shared/ui/button";
import { Label } from "@shared/ui/label";
import { Input } from "@shared/ui/input";
import { Textarea } from "@shared/ui/textarea";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@shared/ui/select";
import { Separator } from "@shared/ui/separator";

export const EditOrder: FC = () => {
	return (
		<Sheet>
			<SheetTrigger asChild>
				<DropdownMenuItem onSelect={(e) => e.preventDefault()}>
					<Edit className="mr-2 h-4 w-4" />
					Edit
				</DropdownMenuItem>
			</SheetTrigger>
			<SheetContent className="overflow-y-auto w-full sm:max-w-xl">
				<SheetHeader>
					<SheetTitle>Edit order</SheetTitle>
					<SheetDescription>
						Make changes to the order details below. Click save when you're done.
					</SheetDescription>
				</SheetHeader>

				<div className="grid flex-1 auto-rows-min gap-6 px-4">
					{/* Order Status */}
					<div className="space-y-4">
						<h3 className="text-lg font-medium">Order Status</h3>
						<div className="grid gap-2">
							<Label htmlFor="order-status">Status</Label>
							<Select>
								<SelectTrigger id="order-status" className="w-full">
									<SelectValue placeholder="Select order status" />
								</SelectTrigger>
								<SelectContent>
									<SelectItem value="pending">Pending</SelectItem>
									<SelectItem value="processing">Processing</SelectItem>
									<SelectItem value="shipped">Shipped</SelectItem>
									<SelectItem value="delivered">Delivered</SelectItem>
									<SelectItem value="cancelled">Cancelled</SelectItem>
									<SelectItem value="returned">Returned</SelectItem>
								</SelectContent>
							</Select>
						</div>
					</div>

					<Separator />

					{/* Shipping Address */}
					<div className="space-y-4">
						<h3 className="text-lg font-medium">Shipping Address</h3>
						<div className="grid grid-cols-2 gap-3">
							<div className="grid gap-2">
								<Label htmlFor="edit-country">Country</Label>
								<Input id="edit-country" placeholder="Enter country" />
							</div>
							<div className="grid gap-2">
								<Label htmlFor="edit-city">City</Label>
								<Input id="edit-city" placeholder="Enter city" />
							</div>
						</div>
						<div className="grid grid-cols-2 gap-3">
							<div className="grid gap-2">
								<Label htmlFor="edit-zipcode">Zip Code</Label>
								<Input id="edit-zipcode" placeholder="Enter zip code" />
							</div>
							<div className="grid gap-2">
								<Label htmlFor="edit-street">Street</Label>
								<Input id="edit-street" placeholder="Enter street address" />
							</div>
						</div>
					</div>

					<Separator />

					{/* Order Information */}
					<div className="space-y-4">
						<h3 className="text-lg font-medium">Order Information</h3>
						<div className="grid gap-2">
							<Label htmlFor="edit-order-info">Order Notes</Label>
							<Textarea
								id="edit-order-info"
								placeholder="Additional order information or notes"
								className="resize-none"
							/>
						</div>
					</div>

					<Separator />

					{/* Order Tracking */}
					<div className="space-y-4">
						<h3 className="text-lg font-medium">Tracking Information</h3>
						<div className="grid gap-2">
							<Label htmlFor="tracking-number">Tracking Number</Label>
							<Input
								id="tracking-number"
								placeholder="Enter tracking number (optional)"
							/>
						</div>
						<div className="grid gap-2">
							<Label htmlFor="shipping-carrier">Shipping Carrier</Label>
							<Select>
								<SelectTrigger id="shipping-carrier" className="w-full">
									<SelectValue placeholder="Select shipping carrier (optional)" />
								</SelectTrigger>
								<SelectContent>
									<SelectItem value="fedex">FedEx</SelectItem>
									<SelectItem value="ups">UPS</SelectItem>
									<SelectItem value="dhl">DHL</SelectItem>
									<SelectItem value="usps">USPS</SelectItem>
									<SelectItem value="other">Other</SelectItem>
								</SelectContent>
							</Select>
						</div>
					</div>

					<Separator />

					{/* Priority Level */}
					<div className="space-y-4">
						<h3 className="text-lg font-medium">Priority</h3>
						<div className="grid gap-2">
							<Label htmlFor="order-priority">Order Priority</Label>
							<Select>
								<SelectTrigger id="order-priority" className="w-full">
									<SelectValue placeholder="Select priority level" />
								</SelectTrigger>
								<SelectContent>
									<SelectItem value="low">Low</SelectItem>
									<SelectItem value="normal">Normal</SelectItem>
									<SelectItem value="high">High</SelectItem>
									<SelectItem value="urgent">Urgent</SelectItem>
								</SelectContent>
							</Select>
						</div>
					</div>

					<Separator />

					{/* Internal Notes */}
					<div className="space-y-4">
						<h3 className="text-lg font-medium">Internal Notes</h3>
						<div className="grid gap-2">
							<Label htmlFor="internal-notes">Staff Notes</Label>
							<Textarea
								id="internal-notes"
								placeholder="Internal notes (not visible to customer)"
								className="resize-none"
							/>
						</div>
					</div>

					{/* Order Summary (Read-only info) */}
					<div className="bg-muted/50 p-4 rounded-lg">
						<h3 className="font-medium mb-2">Order Summary (Read-only)</h3>
						<div className="space-y-1 text-sm">
							<div className="flex justify-between">
								<span className="text-muted-foreground">Order ID:</span>
								<span className="font-mono">#12345</span>
							</div>
							<div className="flex justify-between">
								<span className="text-muted-foreground">Created:</span>
								<span>2024-06-24 10:30 AM</span>
							</div>
							<div className="flex justify-between">
								<span className="text-muted-foreground">Customer:</span>
								<span>John Doe</span>
							</div>
							<div className="flex justify-between">
								<span className="text-muted-foreground">Total:</span>
								<span className="font-medium">$125.50</span>
							</div>
							<div className="flex justify-between">
								<span className="text-muted-foreground">Items:</span>
								<span>3 products</span>
							</div>
						</div>
					</div>
				</div>

				<SheetFooter>
					<Button type="submit">Save changes</Button>
					<SheetClose asChild>
						<Button variant="outline">Cancel</Button>
					</SheetClose>
				</SheetFooter>
			</SheetContent>
		</Sheet>
	);
};
