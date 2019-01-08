import { BaseModel } from "../../shared/base-model.model";

export interface Employee extends BaseModel {
  EmpCode: string;
  Title?: string;
  NameThai?: string;
  NameEng?: string;
  TypeEmployee?: number;
  GroupCode?: string;
  GroupName?: string;
  GroupMIS?: string;
  GroupMisName?: string;
  // ViewModel
  TypeEmployeeString?: string;
  InsertOrUpdate?: string;
}
