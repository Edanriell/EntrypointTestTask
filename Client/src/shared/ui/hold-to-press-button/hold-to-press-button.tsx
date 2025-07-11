import { ComponentProps, FC, type ReactNode } from "react";

import { Button } from "@shared/ui/button";

import { useHoldToAction } from "./lib/hooks";

type HoldToPressButtonProps = {
	onPressAction: () => Promise<void>;
	holdDuration?: number;
	children: ReactNode;
} & Omit<ComponentProps<typeof Button>, "onMouseDown" | "onMouseUp" | "onMouseLeave">;

export const HoldToPressButton: FC<HoldToPressButtonProps> = ({
	onPressAction,
	holdDuration = 1500,
	children,
	className,
	style,
	...rest
}) => {
	const { isPressed, handlers } = useHoldToAction({
		onPressAction,
		holdDuration
	});

	return (
		<Button
			variant="outline"
			className={`relative overflow-hidden transition-transform ease-out active:scale-95 duration-[${holdDuration}ms] ${className || ""}`}
			{...rest}
			{...handlers}
			style={{
				transform: isPressed ? "scale(0.97)" : "scale(1)",
				...style
			}}
		>
			{children}
			<div
				className="absolute inset-0 flex items-center justify-center gap-2 rounded-md bg-red-50 text-red-600 dark:bg-red-950 dark:text-red-400"
				style={{
					clipPath: isPressed ? "inset(0px 0px 0px 0px)" : "inset(0px 100% 0px 0px)",
					transition: isPressed
						? `clip-path ${holdDuration}ms linear`
						: "clip-path 0.2s ease-out"
				}}
			>
				{children}
			</div>
		</Button>
	);
};
