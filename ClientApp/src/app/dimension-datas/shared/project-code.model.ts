import { BaseModel } from "../../shared/base-model.model";

export interface ProjectCode extends BaseModel {
  ProjectName?: string;
  ProjectCode?: string;
  Rowid?: number;
}
