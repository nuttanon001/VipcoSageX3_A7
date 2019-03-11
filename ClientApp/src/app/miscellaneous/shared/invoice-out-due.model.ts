export interface InvoiceOutDue {
  InvoiceNo?: string;
  InvPriceInTax?: number;
  InvPriceInTaxString?: string;
  InvPriceExTax?: number;
  InvPriceExTaxString?: string;
  Currency?: string;
  CustomerNo?: string;
  CustomerName?: string;
  Project?: string;
  StatusClose?: number;
  THB_TAX?: number;
  THB_TAXString?: string;
  USD_TAX?: number;
  USD_TAXString?: string;
  EUR_TAX?: number;
  EUR_TAXString?: string;
  THB?: number;
  THBString?: string;
  USD?: number;
  USDString?: string;
  EUR?: number;
  EURString?: string;
  DueDate?: Date;
  DueDateString?: string;
  DocDate?: Date;
  DocDateString?: string;
  NowDate?: Date;
  NowDateString?: string;
  DIFF?: number;
  DIFFString?: string;
  InvoiceStatus?: InvoiceStatus;
  InvoiceStatusString?: string;
}


export enum InvoiceStatus {
  OutStanding = 1,
  OverDue
}
