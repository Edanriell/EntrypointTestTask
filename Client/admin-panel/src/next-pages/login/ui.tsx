import Image from "next/image";

import { Login } from "@/features";
import { getSession } from "@/entitites/session";

export const LoginPage = async () => {
	const session = await getSession();
	console.log(session);

	return (
		<section className="w-full lg:grid lg:min-h-[600px] lg:grid-cols-2 xl:min-h-[800px]">
			<h1 className="visually-hidden">Login page</h1>
			<div className="flex min-h-[100vh] items-center justify-center py-12">
				<Login />
			</div>
			<div className="hidden bg-muted lg:block">
				<Image
					src="/images/admin-banner.jpg"
					alt="Image"
					width="1920"
					height="1080"
					className="h-[100vh] w-full object-cover dark:brightness-[0.2] dark:grayscale"
				/>
			</div>
		</section>
	);
};
