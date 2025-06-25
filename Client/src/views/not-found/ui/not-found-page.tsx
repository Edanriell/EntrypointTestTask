import Link from "next/link";

import { Button } from "@shared/ui/button";

export const NotFoundPage = () => {
	return (
		<section className="flex flex-col items-center justify-center">
			<div className="text-center">
				<h1 className="text-9xl font-bold text-gray-200">404</h1>
				<div className="mt-4">
					<h2 className="text-2xl font-bold text-gray-800 mb-2">Page Not Found</h2>
					<p className="text-gray-600 mb-8">
						The page you're looking for doesn't exist or has been moved.
					</p>
				</div>
			</div>
			<div className="flex flex-col items-center justify-center space-y-4">
				<Button asChild>
					<Link href="/">Go Home</Link>
				</Button>
			</div>
		</section>
	);
};
