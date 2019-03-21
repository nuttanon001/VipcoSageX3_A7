import { Issue } from "./issue.model";
import { Journal } from "./journal.model";

export interface MiscAccount {
  MiscNumber?: string;
  MiscDate?: Date;
  MiscDateString?: string;
  Project?: string;
  ProjectName?: string;
  Description?: string;
  // Account
  AccNumber?: string;
  AccDate?: Date;
  AccDateString?: string;
  MiscLink?: string;
  AccType?: string;
  AccCat?: number;
  AccIssue?: string;
  DocDate?: Date;
  DocDateString?: string;
  // Detail
  Issues?: Array<Issue>;
  Journals?: Array<Journal>;
}
