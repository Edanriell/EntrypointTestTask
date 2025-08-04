import { FC } from "react";
import { SquarePen } from "lucide-react";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { AnimatePresence, motion } from "motion/react";

import { Gender } from "@entities/users";

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
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@shared/ui/select";
import { Spinner } from "@shared/ui/spinner";

import { useCreateCustomer } from "../api";
import { CreateCustomerFormData, createCustomerSchema } from "../model";

export const CreateCustomer: FC = () => {
	const {
		register,
		handleSubmit,
		formState: { errors, isSubmitting },
		setValue,
		watch,
		reset,
		setError
	} = useForm<CreateCustomerFormData>({
		resolver: zodResolver(createCustomerSchema),
		mode: "onTouched",
		defaultValues: {
			firstName: "",
			lastName: "",
			email: "",
			phoneNumber: "",
			gender: undefined,
			country: "",
			city: "",
			zipCode: "",
			street: "",
			password: "",
			confirmPassword: ""
		}
	});

	const selectedGender = watch("gender");

	const { mutateAsync: createCustomer, isPending } = useCreateCustomer(reset, setError);

	const onSubmit = async (data: CreateCustomerFormData) => {
		const { confirmPassword, ...createCustomerData } = data;
		await createCustomer(createCustomerData);
	};

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
					<SheetTitle>Create client</SheetTitle>
					<SheetDescription>
						Fill in the details below to create a new client. Click save when you're
						done.
					</SheetDescription>
				</SheetHeader>
				<form onSubmit={handleSubmit(onSubmit)}>
					<div className="grid flex-1 auto-rows-min gap-8 px-4 mb-10">
						<div className="grid grid-cols-2 gap-3">
							<div className="grid gap-2 relative">
								<Label htmlFor="first-name">First Name</Label>
								<Input
									id="first-name"
									placeholder="Enter first name"
									{...register("firstName")}
									className={errors.firstName ? "border-red-500" : ""}
								/>
								<AnimatePresence>
									{errors.firstName && (
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
											{errors.firstName.message}
										</motion.p>
									)}
								</AnimatePresence>
							</div>
							<div className="grid gap-2 relative">
								<Label htmlFor="last-name">Last Name</Label>
								<Input
									id="last-name"
									placeholder="Enter last name"
									{...register("lastName")}
									className={errors.lastName ? "border-red-500" : ""}
								/>
								<AnimatePresence>
									{errors.lastName && (
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
											{errors.lastName.message}
										</motion.p>
									)}
								</AnimatePresence>
							</div>
						</div>
						<div className="grid gap-2 col-span-full relative">
							<Label htmlFor="email">Email</Label>
							<Input
								id="email"
								type="email"
								placeholder="Enter email address"
								{...register("email")}
								className={errors.email ? "border-red-500" : ""}
							/>
							<AnimatePresence>
								{errors.email && (
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
										{errors.email.message}
									</motion.p>
								)}
							</AnimatePresence>
						</div>
						<div className="grid gap-2 col-span-full relative">
							<Label htmlFor="phone-number">Phone Number</Label>
							<Input
								id="phone-number"
								type="tel"
								placeholder="Enter phone number"
								{...register("phoneNumber")}
								className={errors.phoneNumber ? "border-red-500" : ""}
							/>
							<AnimatePresence>
								{errors.phoneNumber && (
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
										{errors.phoneNumber.message}
									</motion.p>
								)}
							</AnimatePresence>
						</div>
						<div className="grid gap-2 col-span-full relative">
							<Label htmlFor="gender">Gender</Label>
							<Select
								value={
									selectedGender !== undefined ? selectedGender.toString() : ""
								}
								onValueChange={(value) =>
									setValue("gender", parseInt(value) as Gender, {
										shouldValidate: true
									})
								}
							>
								<SelectTrigger
									id="gender"
									className={`w-full ${errors.gender ? "border-red-500" : ""}`}
								>
									<SelectValue placeholder="Select gender" />
								</SelectTrigger>
								<SelectContent>
									<SelectItem value="0">Male</SelectItem>
									<SelectItem value="1">Female</SelectItem>
								</SelectContent>
							</Select>
							<AnimatePresence>
								{errors.gender && (
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
										{errors.gender.message}
									</motion.p>
								)}
							</AnimatePresence>
						</div>
						<div className="grid grid-cols-2 gap-3">
							<div className="grid gap-2 relative">
								<Label htmlFor="country">Country</Label>
								<Input
									id="country"
									placeholder="Enter country"
									{...register("country")}
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
											exit={{ opacity: 0, x: 15, filter: "blur(0.24rem)" }}
											className="text-sm text-red-500 absolute bottom-[-1.5rem]"
										>
											{errors.country.message}
										</motion.p>
									)}
								</AnimatePresence>
							</div>
							<div className="grid gap-2 relative">
								<Label htmlFor="city">City</Label>
								<Input
									id="city"
									placeholder="Enter city"
									{...register("city")}
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
											exit={{ opacity: 0, x: 15, filter: "blur(0.24rem)" }}
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
								<Label htmlFor="zip-code">Zip Code</Label>
								<Input
									id="zip-code"
									placeholder="Enter zip code"
									{...register("zipCode")}
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
											exit={{ opacity: 0, x: 15, filter: "blur(0.24rem)" }}
											className="text-sm text-red-500 absolute bottom-[-1.5rem]"
										>
											{errors.zipCode.message}
										</motion.p>
									)}
								</AnimatePresence>
							</div>
							<div className="grid gap-2 relative">
								<Label htmlFor="street">Street</Label>
								<Input
									id="street"
									placeholder="Enter street address"
									{...register("street")}
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
											exit={{ opacity: 0, x: 15, filter: "blur(0.24rem)" }}
											className="text-sm text-red-500 absolute bottom-[-1.5rem]"
										>
											{errors.street.message}
										</motion.p>
									)}
								</AnimatePresence>
							</div>
						</div>
						<div className="grid gap-2 relative">
							<Label htmlFor="password">Password</Label>
							<Input
								id="password"
								type="password"
								placeholder="Enter password"
								{...register("password")}
								className={errors.password ? "border-red-500" : ""}
							/>
							<AnimatePresence>
								{errors.password && (
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
										{errors.password.message}
									</motion.p>
								)}
							</AnimatePresence>
						</div>
						<div className="grid gap-2 relative">
							<Label htmlFor="confirm-password">Confirm Password</Label>
							<Input
								id="confirm-password"
								type="password"
								placeholder="Confirm password"
								{...register("confirmPassword")}
								className={errors.confirmPassword ? "border-red-500" : ""}
							/>
							<AnimatePresence>
								{errors.confirmPassword && (
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
										{errors.confirmPassword.message}
									</motion.p>
								)}
							</AnimatePresence>
						</div>
					</div>
					<SheetFooter>
						<Button type="submit" disabled={isSubmitting || isPending}>
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
