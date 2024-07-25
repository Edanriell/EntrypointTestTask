import { FC } from "react";
import { Drawer, DrawerContent } from "@shared/ui/drawer";

type ContentDrawerProps = {
	data: any;
	onDrawerClose: () => void;
	isOrderDrawerOpen: boolean;
	DrawerInnerContent: FC<any>;
};

export const ContentDrawer: FC<ContentDrawerProps> = ({
	isOrderDrawerOpen,
	data,
	onDrawerClose,
	DrawerInnerContent
}) => {
	return (
		<Drawer direction="right" open={isOrderDrawerOpen} onClose={onDrawerClose}>
			<DrawerContent className="h-full w-[480px]">
				<DrawerInnerContent order={data} />
			</DrawerContent>
		</Drawer>
	);
};
