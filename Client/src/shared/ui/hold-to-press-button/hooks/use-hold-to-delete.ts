import { useRef, useState } from "react";

type useHoldToActionOptions = {
	onPressAction: () => Promise<void>;
	holdDuration?: number;
};

export const useHoldToAction = ({ onPressAction, holdDuration = 1500 }: useHoldToActionOptions) => {
	const [isPressed, setIsPressed] = useState(false);
	const timeoutRef = useRef<NodeJS.Timeout | null>(null);

	const clearCurrentTimeout = () => {
		if (timeoutRef.current) {
			clearTimeout(timeoutRef.current);
			timeoutRef.current = null;
		}
	};

	const handleMouseDown = () => {
		setIsPressed(true);
		timeoutRef.current = setTimeout(async () => {
			await onPressAction();
		}, holdDuration);
	};

	const handleMouseUp = () => {
		setIsPressed(false);
		clearCurrentTimeout();
	};

	const handleMouseLeave = () => {
		setIsPressed(false);
		clearCurrentTimeout();
	};

	return {
		isPressed,
		handlers: {
			onMouseDown: handleMouseDown,
			onMouseUp: handleMouseUp,
			onMouseLeave: handleMouseLeave
		}
	};
};
