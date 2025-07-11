import { z } from "zod";

import { editUserSchema } from "../model";

export type EditUserFormData = z.infer<typeof editUserSchema>;
