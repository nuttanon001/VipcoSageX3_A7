import { BaseModel } from "../../shared/base-model.model";

export interface Workgroup extends BaseModel {
  WorkGroupName?: string;
  WorkGroupCode?: string;
  Rowid?: number;
}
