import { Metadata } from "next";

type GenerateStaticMetadataParameters = {
	title: string;
	description: string;
	ogTitle: string;
	ogDescription: string;
	ogImagePath?: string;
};

export const generateStaticMetadata = ({
	title,
	description,
	ogTitle,
	ogDescription,
	ogImagePath
}: GenerateStaticMetadataParameters): Metadata => {
	const openGraphConfig: Metadata["openGraph"] = {
		title: ogTitle,
		description: ogDescription
	};

	if (ogImagePath) {
		openGraphConfig.images = [
			{
				url: ogImagePath,
				alt: ogTitle
			}
		];
	}

	return {
		metadataBase: new URL(process.env.NEXT_PUBLIC_APP_URL || "http://localhost:3000"),
		title,
		description,
		openGraph: openGraphConfig
	};
};
