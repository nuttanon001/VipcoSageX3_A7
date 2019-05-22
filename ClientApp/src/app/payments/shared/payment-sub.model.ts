export interface PaymentSub {
  PartnerNo?: string;
  Comment?: string;
  PartnerName?: string;
  PaymentNo?: string;
  Reference?: string;
  PaymentDate?: Date;
  PaymentDateString?: string;
  Project?: string;
  Currency?: string;
  Attribute?: string;
  PayType?: string;
  AmountProgress?: number;
  AmountProgressString?: string;
  AmountDown?: number;
  AmountDownString?: string;
  AmountConsume?: number;
  AmountConsumeString?: string;
  AmountRetenion?: number;
  AmountRetenionString?: string;
  AmountVat?: number;
  AmountVatString?: string;
  AmountTax?: number;
  AmountTaxString?: string;
  AmountDeduct?: number;
  AmountDeductString?: string;
}
