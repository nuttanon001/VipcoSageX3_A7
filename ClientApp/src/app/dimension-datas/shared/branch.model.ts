import { BaseModel } from 'src/app/shared/base-model.model';

export interface Branch extends BaseModel {
  BranchName?: string;
  BranchCode?: string;
  RowId?: number;
}
