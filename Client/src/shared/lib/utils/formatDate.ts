type DateFormatOptions = {
	includeTime?: boolean;
	locale?: string;
};

export const formatDate = (date: string | Date, options: DateFormatOptions = {}) => {
	const { includeTime = false, locale = "en-US" } = options;

	const formatOptions: Intl.DateTimeFormatOptions = {
		year: "numeric",
		month: "short",
		day: "numeric",
		...(includeTime && {
			hour: "2-digit",
			minute: "2-digit"
		})
	};

	return new Date(date).toLocaleDateString(locale, formatOptions);
};
