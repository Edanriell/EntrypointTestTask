import { z } from "zod";

import { editProductSchema } from "../model";

export type EditProductFormData = z.infer<typeof editProductSchema>;
