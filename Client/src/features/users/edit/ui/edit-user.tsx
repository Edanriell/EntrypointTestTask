import { FC, useEffect } from "react";
import { Edit } from "lucide-react";
import { AnimatePresence, motion } from "motion/react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import { Gender } from "@entities/users";

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
import { Spinner } from "@shared/ui/spinner";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@shared/ui/select";
import { getChangedFields } from "@shared/lib/utils";

import { useGetCustomerById, useUpdateUser } from "../api";
import { EditUserFormData, editUserSchema, GENDER_MAPPING } from "../model";
import { USER_UPDATABLE_FIELDS } from "../config";
import { genderComparator } from "../lib";

type EditUserProps = {
	userId: string;
};

export const EditUser: FC<EditUserProps> = ({ userId }) => {
	const {
		register,
		handleSubmit,
		formState: { errors, isSubmitting },
		setValue,
		watch,
		reset,
		setError
	} = useForm<EditUserFormData>({
		resolver: zodResolver(editUserSchema),
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
			street: ""
		}
	});

	const selectedGender = watch("gender");

	// Fetch user data for prefilling the form
	const { data: userData, isLoading: isLoadingUser } = useGetCustomerById(userId);

	// Update user mutation
	const { mutateAsync: updateUser, isPending } = useUpdateUser(setError);

	// Prefill form when user data is loaded
	useEffect(() => {
		if (userData) {
			setValue("firstName", userData.firstName);
			setValue("lastName", userData.lastName);
			setValue("email", userData.email);
			setValue("phoneNumber", userData.phoneNumber);
			setValue("gender", GENDER_MAPPING[userData.gender]);
			setValue("country", userData.country);
			setValue("city", userData.city);
			setValue("zipCode", userData.zipCode);
			setValue("street", userData.street);
		}
	}, [userData, setValue]);

	const onSubmit = async (data: EditUserFormData) => {
		try {
			const updatedUserData = getChangedFields(
				userData!,
				data,
				USER_UPDATABLE_FIELDS,
				genderComparator
			);

			// console.log("Changed fields:", updatedUserData);

			if (Object.keys(updatedUserData).length > 0) {
				await updateUser({ userId, updatedUserData });
			} else {
				console.log("No changes detected");
			}
		} catch (error) {
			console.error("Error updating user:", error);
		}
	};

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
					<SheetTitle>Edit client</SheetTitle>
					<SheetDescription>
						Make changes to the client details below. Click save when you're done.
					</SheetDescription>
				</SheetHeader>
				{isLoadingUser ? (
					<div className="flex justify-center items-center h-64">
						<Spinner />
					</div>
				) : (
					<form onSubmit={handleSubmit(onSubmit)}>
						<div className="grid flex-1 auto-rows-min gap-8 px-4 mb-10">
							<div className="grid grid-cols-2 gap-3">
								<div className="grid gap-2 relative">
									<Label htmlFor="edit-first-name">First Name</Label>
									<Input
										id="edit-first-name"
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
												exit={{
													opacity: 0,
													x: 15,
													filter: "blur(0.24rem)"
												}}
												className="text-sm text-red-500 absolute bottom-[-1.5rem]"
											>
												{errors.firstName.message}
											</motion.p>
										)}
									</AnimatePresence>
								</div>
								<div className="grid gap-2 relative">
									<Label htmlFor="edit-last-name">Last Name</Label>
									<Input
										id="edit-last-name"
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
												exit={{
													opacity: 0,
													x: 15,
													filter: "blur(0.24rem)"
												}}
												className="text-sm text-red-500 absolute bottom-[-1.5rem]"
											>
												{errors.lastName.message}
											</motion.p>
										)}
									</AnimatePresence>
								</div>
							</div>
							<div className="grid gap-2 col-span-full relative">
								<Label htmlFor="edit-email">Email</Label>
								<Input
									id="edit-email"
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
								<Label htmlFor="edit-phone-number">Phone Number</Label>
								<Input
									id="edit-phone-number"
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
								<Label htmlFor="edit-gender">Gender</Label>
								<Select
									value={
										selectedGender !== undefined
											? selectedGender.toString()
											: ""
									}
									onValueChange={(value) =>
										setValue("gender", parseInt(value) as Gender, {
											shouldValidate: true
										})
									}
								>
									<SelectTrigger
										id="edit-gender"
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
									<Label htmlFor="edit-country">Country</Label>
									<Input
										id="edit-country"
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
									<Label htmlFor="edit-city">City</Label>
									<Input
										id="edit-city"
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
									<Label htmlFor="edit-zip-code">Zip Code</Label>
									<Input
										id="edit-zip-code"
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
									<Label htmlFor="edit-street">Street</Label>
									<Input
										id="edit-street"
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
						<SheetFooter>
							<Button type="submit" disabled={isSubmitting || isPending}>
								{isSubmitting || isPending ? (
									<>
										<Spinner />
										<span>Saving...</span>
									</>
								) : (
									<span>Save Changes</span>
								)}
							</Button>
							<SheetClose asChild>
								<Button variant="outline">Cancel</Button>
							</SheetClose>
						</SheetFooter>
					</form>
				)}
			</SheetContent>
		</Sheet>
	);
};
