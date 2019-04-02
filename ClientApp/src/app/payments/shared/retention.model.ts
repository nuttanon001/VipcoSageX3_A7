export interface Retention {
  PartnerNo?: string;
  PartnerName?: string;
  PaymentNo?: string;
  PaymentDateString?: string;
  DescriptionLine?: string;
  Branch?: string;
  WorkItem?: string;
  Project?: string;
  WorkGroup?: string;
  Currency?: string;
  Attribute?: string;
  AmountRetenionString?: string;
  AmountDeductString?: string;
  Comment?: string;

  PaymentDate?: Date;

  AmountRetenion?: number;
  AmountDeduct?: number;
}
