export const orderStatusColorMap = (index: string) => {
	const colors: { [key: string]: string } = {
		"0": "#94a3b8",
		"1": "#f59e0b",
		"2": "#3b82f6",
		"3": "#8b5cf6",
		"4": "#10b981",
		"5": "#ef4444",
		"6": "#f97316",
		"7": "#6366f1",
		"8": "#14b8a6",
		"9": "#e11d48",
		"10": "#0ea5e9",
		"11": "#8b5cf6"
	};

	return colors[index];
};
