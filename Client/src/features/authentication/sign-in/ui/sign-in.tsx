"use client";

import { useForm } from "react-hook-form";
import Image from "next/image";
import { zodResolver } from "@hookform/resolvers/zod";
import { AnimatePresence, motion } from "motion/react";

import { useCredentialsAuth } from "@features/authentication/general/lib/hooks";
import { SignInFormData, signInSchema } from "@features/authentication/sign-in";

import { Button } from "@shared/ui/button";
import { Input } from "@shared/ui/input";
import { Label } from "@shared/ui/label";
import { Card, CardContent } from "@shared/ui/card";
import { cn } from "@shared/lib/utils";
import { Spinner } from "@shared/ui/spinner";

export function SignIn({ className, ...props }: { className?: string }) {
	const { login, isLoading } = useCredentialsAuth();

	const {
		register,
		handleSubmit,
		formState: { errors, isSubmitting }
	} = useForm<SignInFormData>({
		resolver: zodResolver(signInSchema),
		mode: "onTouched"
	});

	const onSubmit = async (data: SignInFormData) => {
		await login(data);
	};

	return (
		<div className={cn("flex flex-col gap-6", className)} {...props}>
			<Card className="overflow-hidden p-0">
				<CardContent className="grid p-0 md:grid-cols-2">
					<form onSubmit={handleSubmit(onSubmit)} className="p-6 md:p-8">
						<div className="flex flex-col gap-6">
							<div className="flex flex-col items-center text-center">
								<h1 className="text-2xl font-bold">Welcome back</h1>
								<p className="text-muted-foreground text-balance">
									Login to your account
								</p>
							</div>
							<div className="grid gap-3 relative">
								<Label htmlFor="email">Email</Label>
								<Input
									id="email"
									type="email"
									placeholder="m@example.com"
									{...register("email")}
									disabled={isLoading || isSubmitting}
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
											className="text-sm text-red-500 absolute bottom-[-1.3rem]"
										>
											{errors.email.message}
										</motion.p>
									)}
								</AnimatePresence>
							</div>
							<div className="grid gap-3 relative">
								<Label htmlFor="password">Password</Label>
								<Input
									id="password"
									type="password"
									{...register("password")}
									disabled={isLoading || isSubmitting}
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
											className="text-sm text-red-500 absolute bottom-[-1.3rem]"
										>
											{errors.password.message}
										</motion.p>
									)}
								</AnimatePresence>
							</div>
							<Button type="submit" className="w-full" disabled={isSubmitting}>
								{isSubmitting ? (
									<>
										<Spinner />
										<span>Signing in...</span>
									</>
								) : (
									<span>Sign in</span>
								)}
							</Button>
						</div>
					</form>
					<div className="bg-muted relative hidden md:block">
						<Image
							src="/images/banner.jpg"
							alt="Picture of woman working in office"
							className="absolute inset-0 h-full w-full object-cover dark:brightness-[0.2] dark:grayscale"
							layout="fill"
							priority
						/>
					</div>
				</CardContent>
			</Card>
		</div>
	);
}
