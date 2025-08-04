import { z } from "zod";

import { createOrderSchema } from "./schema";

export type CreateOrderFormData = z.infer<typeof createOrderSchema>;
