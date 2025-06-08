export const formatDateString = (dateString: string) => {
	const dateObj = new Date(dateString); // Create a Date object from the ISO format string

	const options: Intl.DateTimeFormatOptions = {
		year: "numeric",
		month: "short",
		day: "numeric",
		hour: "numeric",
		minute: "numeric",
		second: "numeric",
		hour12: true
	};

	return dateObj.toLocaleString("en-US", options);
};
