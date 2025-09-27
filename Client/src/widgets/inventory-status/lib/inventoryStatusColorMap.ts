export const inventoryStatusColorMap = (index: string): string => {
	const colors: Record<string, string> = {
		"0": "#94a3b8",
		"1": "#f59e0b",
		"2": "#3b82f6",
		"3": "#a855f7",
		"4": "#10b981",
		"5": "#ef4444",
		"6": "#f97316",
		"7": "#22d3ee",
		"8": "#84cc16",
		"9": "#ec4899",
		"10": "#0ea5e9",
		"11": "#9333ea"
	};

	return colors[index] ?? "#9ca3af";
};
