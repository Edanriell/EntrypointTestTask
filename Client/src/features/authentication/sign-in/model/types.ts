import { z } from "zod";

import { signInSchema } from "./schemas";

export type SignInFormData = z.infer<typeof signInSchema>;
