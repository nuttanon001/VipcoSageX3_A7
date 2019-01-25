export interface InvoiceSupBp {
  DocumentNo?: string;
  AccountDate?: Date;
  AccountDateString?: string;
  InvType?: string;
  Site?: string;
  Supplier?: string;
  SupplierName?: string;
  HeadAccountCode?: number;
  LineAccountCode?: string;
  AmountTax?: number;
  AmountTaxString?: string;
  Tax?: string;
  TaxAmount?: number
  TaxAmountString?: string;
  Comment?: string;
  Project1?: string;
  Project2?: string;
  Branch?: string;
  Bom?: string;
  WorkGroup?: string;
  CostCenter?: string;
  Issued?: string;
  Title?: string;
  TaxinvNo?: string;
}
