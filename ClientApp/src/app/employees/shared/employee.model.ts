import { BaseModel } from 'src/app/shared2/basemode/base-model.model';

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
