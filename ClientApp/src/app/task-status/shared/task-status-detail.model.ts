import { BaseModel } from 'src/app/shared2/basemode/base-model.model';

export interface TaskStatusDetail extends BaseModel {
  TaskStatusDetailId: number;
  EmployeeCode?: string;
  Name?: string;
  Email?: string;
  Remark?: string;
  // FK
  //TaskStatusMaster
  TaskStatusMasterId?: number;
}
