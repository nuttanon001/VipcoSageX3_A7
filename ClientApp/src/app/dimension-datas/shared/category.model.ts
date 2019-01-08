import { BaseModel } from "../../shared/base-model.model";

export interface Category extends BaseModel {
  CategoryName?: string;
  CategoryCode?: string;
  Rowid?: number;
}
