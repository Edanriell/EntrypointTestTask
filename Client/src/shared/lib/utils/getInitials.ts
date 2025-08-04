// TODO looks like duplicate !!!
export const getInitials = (fullName: string) => {
	const parts = fullName.split(" ");
	return parts.map((part) => part.charAt(0)).join("");
};
