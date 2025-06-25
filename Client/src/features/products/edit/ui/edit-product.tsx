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

export const EditProduct: FC = () => {
	return (
		<Sheet>
			<SheetTrigger asChild>
				<DropdownMenuItem onSelect={(e) => e.preventDefault()}>
					<Edit className="mr-2 h-4 w-4" />
					Edit
				</DropdownMenuItem>
			</SheetTrigger>
			<SheetContent className="overflow-y-auto">
				<SheetHeader>
					<SheetTitle>Edit product</SheetTitle>
					<SheetDescription>
						Make changes to the product details below. Click save when you're done.
					</SheetDescription>
				</SheetHeader>
				<div className="grid flex-1 auto-rows-min gap-6 px-4">
					<div className="grid gap-2 col-span-full">
						<Label htmlFor="name">Name</Label>
						<Input id="name" type="text" placeholder="Enter product name" />
					</div>
					<div className="grid gap-2 col-span-full">
						<Label htmlFor="description">Description</Label>
						<Textarea
							id="description"
							placeholder="Product description"
							className="resize-none"
						/>
					</div>
					<div className="grid gap-2 col-span-full">
						<Label htmlFor="price">Price</Label>
						<Input id="price" placeholder="Enter product price" />
					</div>
					<div className="grid grid-cols-2 gap-3">
						<div className="grid gap-2">
							<Label htmlFor="stock">Stock</Label>
							<Input id="stock" placeholder="Enter product stock" />
						</div>
						<div className="grid gap-2">
							<Label htmlFor="reserved-stock">Reserved stock</Label>
							<Input id="reserved-stock" placeholder="Enter product reserved stock" />
						</div>
					</div>
				</div>
				<SheetFooter>
					<Button type="submit">Save Changes</Button>
					<SheetClose asChild>
						<Button variant="outline">Cancel</Button>
					</SheetClose>
				</SheetFooter>
			</SheetContent>
		</Sheet>
	);
};
