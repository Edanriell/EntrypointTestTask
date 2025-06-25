import { NotFoundPage } from "@views/not-found";

import { metadata, NotFoundLayout } from "@widgets/layout/not-found";

export { metadata };

export default function NotFound() {
	return (
		<NotFoundLayout>
			<NotFoundPage />
		</NotFoundLayout>
	);
}
