import { z } from "zod";

import { createProductSchema } from "./schema";

export type CreateProductFormData = z.infer<typeof createProductSchema>;
