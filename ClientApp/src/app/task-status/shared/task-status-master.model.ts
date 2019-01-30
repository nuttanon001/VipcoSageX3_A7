import { TaskStatusDetail } from './task-status-detail.model';
import { BaseModel } from 'src/app/shared2/basemode/base-model.model';

export interface TaskStatusMaster extends BaseModel {
  TaskStatusMasterId: number;
  WorkGroupCode?: string;
  WorkGroupName?: string;
  Remark?: string;
  //TaskStatusDetail
  TaskStatusDetails?: Array<TaskStatusDetail>;
}
