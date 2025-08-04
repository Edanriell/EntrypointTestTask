import { z } from "zod";

import { returnOrderSchema } from "./schema";

export type ReturnOrderFormData = z.infer<typeof returnOrderSchema>;
