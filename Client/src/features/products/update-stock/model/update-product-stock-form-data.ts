import { z } from "zod";

import { updateProductStockSchema } from "./schema";

export type UpdateProductStockFormData = z.infer<typeof updateProductStockSchema>;
