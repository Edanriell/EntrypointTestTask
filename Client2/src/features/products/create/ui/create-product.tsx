import { FC } from "react";
import { SquarePen } from "lucide-react";

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

export const CreateProduct: FC = () => {
	return (
		<Sheet>
			<SheetTrigger asChild>
				<Button className="flex w-fit self-end">
					<SquarePen />
					Create new
				</Button>
			</SheetTrigger>
			<SheetContent className="overflow-y-auto">
				<SheetHeader>
					<SheetTitle>Create product</SheetTitle>
					<SheetDescription>
						Fill in the details below to create a new product. Click save when you're
						done.
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
					<div className="grid grid-cols-2 gap-3">
						<div className="grid gap-2">
							<Label htmlFor="price">Price</Label>
							<Input id="price" placeholder="Enter product price" />
						</div>
						<div className="grid gap-2">
							<Label htmlFor="stock">Stock</Label>
							<Input id="stock" placeholder="Enter product stock" />
						</div>
					</div>
				</div>
				<SheetFooter>
					<Button type="submit">Create New</Button>
					<SheetClose asChild>
						<Button variant="outline">Cancel</Button>
					</SheetClose>
				</SheetFooter>
			</SheetContent>
		</Sheet>
	);
};
