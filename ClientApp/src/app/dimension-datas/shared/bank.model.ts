import { BaseModel } from "../../shared/base-model.model";

export interface Bank extends BaseModel {
  BankNumber?: string;
  Description?: string;
  Rowid?: number;
}
