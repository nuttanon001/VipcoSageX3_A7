export interface Scroll {
  Skip?: number;
  Take?: number;
  SortField?: string;
  SortOrder?: number;
  Filter?: string;
  Reload?: boolean;
  Where?: string;
  WhereId?: number;
  TotalRow?: number;
  WhereWorkItem?: string;
  WhereWorkGroup?: string;
  WhereProject?: string;
  WhereProjects?: Array<string>;
  WhereBranch?: string;
  WhereBranchs?: Array<string>;
  WhereBank?: string;
  WhereBanks?: Array<string>;
  WhereSupplier?: string;
  WhereRange11?: string;
  WhereRange12?: string;
  WhereRange21?: string;
  WhereRange22?: string;
  SDate?: Date;
  EDate?: Date;
  SDate2?: Date;
  EDate2?: Date;
}
