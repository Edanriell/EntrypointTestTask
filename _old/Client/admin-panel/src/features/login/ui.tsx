"use client";

import {signIn} from "next-auth/react";
import {useRouter} from "next/navigation";
import {toast} from "sonner";
import {useForm} from "react-hook-form";
import {z} from "zod";
import {zodResolver} from "@hookform/resolvers/zod";

import {Input} from "@shared/ui/input";
import {Button} from "@shared/ui/button";
import {Form, FormControl, FormField, FormItem, FormLabel, FormMessage} from "@shared/ui/form";

const FormSchema = z.object({
	username: z.string().min(4, {
		message: "The username must be at least 4 characters long."
	}),
	password: z.string().regex(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/, {
		message:
			"The password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one number."
	})
});

export const Login = () => {
	const router = useRouter();

	const loginForm = useForm<z.infer<typeof FormSchema>>({
		resolver: zodResolver(FormSchema),
		defaultValues: {
			username: "",
			password: ""
		},
		mode: "onChange"
	});

	const onFormSubmit = async (data: z.infer<typeof FormSchema>) => {
		const { username, password } = data;

		try {
			const signInResult = await signIn("credentials", {
				username,
				password,
				redirect: false
			});

			if (signInResult?.status === 401) toast.error("Unsuccessful auth attempt.");

			if (signInResult?.ok) {
				router.push("/dashboard");
			}
		} catch (error) {
			console.error(error);
		}
	};

	return (
		<article className="mx-auto grid w-[350px] gap-6">
			<div className="grid gap-2 text-center">
				<h2 className="text-3xl font-bold">Login</h2>
				<p className="text-balance text-muted-foreground">
					Enter your username and password below to login to your account
				</p>
			</div>
			<Form {...loginForm}>
				<form onSubmit={loginForm.handleSubmit(onFormSubmit)} className="grid gap-4">
					<FormField
						control={loginForm.control}
						name="username"
						render={({ field }) => (
							<FormItem className="grid gap-2">
								<FormLabel>Username</FormLabel>
								<FormControl>
									<Input type="text" placeholder="John" {...field} />
								</FormControl>
								<FormMessage />
							</FormItem>
						)}
					/>
					<FormField
						control={loginForm.control}
						name="password"
						render={({ field }) => (
							<FormItem className="grid gap-2">
								<FormLabel>Password</FormLabel>
								<FormControl>
									<Input type="password" {...field} />
								</FormControl>
								<FormMessage />
							</FormItem>
						)}
					/>
					<Button className="w-full" type="submit">
						Login
					</Button>
				</form>
			</Form>
		</article>
	);
};
