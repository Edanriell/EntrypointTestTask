export const formatDateString = (dateString: string) => {
	const dateObj = new Date(dateString); // Create a Date object from the ISO format string

	const day = String(dateObj.getDate()).padStart(2, "0");
	const month = String(dateObj.getMonth() + 1).padStart(2, "0"); // Month is zero-indexed, so we add 1
	const year = dateObj.getFullYear();
	const hours = String(dateObj.getHours()).padStart(2, "0");
	const minutes = String(dateObj.getMinutes()).padStart(2, "0");
	const seconds = String(dateObj.getSeconds()).padStart(2, "0");

	return `${day}.${month}.${year} ${hours}:${minutes}:${seconds}`;
};
