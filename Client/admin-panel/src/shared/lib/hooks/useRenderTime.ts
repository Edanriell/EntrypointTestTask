import { useEffect, useRef } from "react";

export const useRenderTime = () => {
	const startTime = useRef(performance.now());
	const endTime = useRef<number | null>(null);

	useEffect(() => {
		endTime.current = performance.now();
		console.log(`Render time: ${endTime.current - startTime.current} ms`);
	});

	console.log(endTime.current ? endTime.current - startTime.current : 0);

	return endTime.current ? endTime.current - startTime.current : 0;
};
