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
  WhereBranch?: string;
  WhereBank?: string;
  WhereBanks?: Array<string>;
  WhereSupplier?: string;
  SDate ?: Date;
  EDate ?: Date;
}
