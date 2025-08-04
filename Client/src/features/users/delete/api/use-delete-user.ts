import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

import { deleteUser } from "@entities/users";

import { ErrorHandler } from "@shared/lib/handlers/error";

// Much more flexible approach but not so clean.
// Left for reference as an option
// export const composeCallbacks = <TData, TError, TVariables>(
// 	first?: (data: TData, variables: TVariables, context: unknown) => void,
// 	second?: (data: TData, variables: TVariables, context: unknown) => void
// ) => {
// 	return (data: TData, variables: TVariables, context: unknown) => {
// 		first?.(data, variables, context);
// 		second?.(data, variables, context);
// 	};
// };
//
// export const useDeleteUser = (options?: UseMutationOptions<void, Error, string>) => {
// 	const queryClient = useQueryClient();
//
// 	const handleSuccess = (data: void, userId: string, context: unknown) => {
// 		queryClient.invalidateQueries({
// 			queryKey: ["users", "list", "customers"],
// 			exact: false,
// 			refetchType: "active"
// 		});
//
// 		queryClient.removeQueries({
// 			queryKey: ["users", "customerDetail", userId]
// 		});
// 	};
//
// 	return useMutation({
// 		mutationFn: deleteUser,
// 		...options,
// 		onSuccess: composeCallbacks(handleSuccess, options?.onSuccess),
// 		onError: composeCallbacks(undefined, options?.onError)
// 	});
// };
// Example of usage
// const { mutateAsync, isPending } = useDeleteUser({
// 	onSuccess: () => {
// 		toast.success("User deleted successfully");
// 		setIsDialogOpen(false);
// 	},
// 	onError: (error) => {
// 		toast.error("Failed to delete user. Please try again.");
// 		console.error("Delete user error:", error);
// 	}
// });

export const useDeleteUser = (setIsDialogOpen: (open: boolean) => void) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: deleteUser,
		onSuccess: (_, { userId }) => {
			queryClient.invalidateQueries({
				queryKey: ["users", "list", "customers"]
			});

			queryClient.removeQueries({
				queryKey: ["users", "customerDetail", userId]
			});

			toast.success("User deleted successfully", {
				description: `Id: ${userId}`
			});

			setIsDialogOpen(false);
		},
		onError: ErrorHandler.createMutationErrorHandler(undefined, undefined, {
			action: "delete_user",
			resource: "user"
		})
	});
};
