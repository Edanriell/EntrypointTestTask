import { NotFoundPage } from "@views/not-found";

import { metadata, NotFoundLayout } from "@widgets/layout/not-found";

export { metadata };

export default async function NotFound() {
	return (
		<NotFoundLayout>
			<NotFoundPage />
		</NotFoundLayout>
	);
}
