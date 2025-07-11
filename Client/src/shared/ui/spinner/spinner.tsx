import { FC } from "react";

type SpinnerProps = {
	className?: string;
	size?: number;
};

export const Spinner: FC<SpinnerProps> = ({ className = "", size = 24 }) => {
	return (
		<div className={`inline-flex items-center justify-center mr-2 ${className}`}>
			<svg
				className="animate-spin text-current [animation-duration:1s]"
				width={size}
				height={size}
				viewBox="0 0 24 24"
				fill="none"
				xmlns="http://www.w3.org/2000/svg"
			>
				<circle
					className="[animation:spinner-dash_1.5s_ease-in-out_infinite]"
					cx="12"
					cy="12"
					r="10"
					stroke="currentColor"
					strokeWidth="4"
					fill="none"
				/>
			</svg>
		</div>
	);
};
