import { z } from "zod";

import { shipOrderSchema } from "./schema";

export type ShipOrderFormData = z.infer<typeof shipOrderSchema>;
