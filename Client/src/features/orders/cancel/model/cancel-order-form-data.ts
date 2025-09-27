import { z } from "zod";

import { cancelOrderSchema } from "./schema";

export type CancelOrderFormData = z.infer<typeof cancelOrderSchema>;
