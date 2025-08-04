import { z } from "zod";

import { updateProductPriceSchema } from "./schema";

export type UpdateProductPriceFormData = z.infer<typeof updateProductPriceSchema>;
