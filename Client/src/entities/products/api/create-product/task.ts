import { useEffect, useRef, useState } from "react";

const useWindowSize = () => {
	const [windowSize, setWindowSize] = useState<{ width: number; height: number }>({
		width: 0,
		height: 0
	});
	const timeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);

	const handleResize = () => {
		// Clear the previous timeout (reset the waiting period)
		if (timeoutRef.current) clearTimeout(timeoutRef.current);

		// Wait 100ms of inactivity before executing
		timeoutRef.current = setTimeout(() => {
			setWindowSize({
				width: window.innerWidth,
				height: window.innerHeight
			});
		}, 100);
	};

	useEffect(() => {
		// Set the initial size immediately (no delay)
		setWindowSize({
			width: window.innerWidth,
			height: window.innerHeight
		});

		// Add an event listener for future changes (with debouncing)
		window.addEventListener("resize", handleResize);

		return () => {
			if (timeoutRef.current) clearTimeout(timeoutRef.current);
			window.removeEventListener("resize", handleResize);
		};
	}, []);

	return windowSize;
};
