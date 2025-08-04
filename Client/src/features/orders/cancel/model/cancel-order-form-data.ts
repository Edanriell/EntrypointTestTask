import { z } from "zod/index";

import { cancelOrderSchema } from "./schema";

export type CancelOrderFormData = z.infer<typeof cancelOrderSchema>;
