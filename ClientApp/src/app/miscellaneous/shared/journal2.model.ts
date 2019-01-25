export interface Journal2 {
    DocumentNo ?:string;
    Date ?: Date;
    DateString ?: string;
    EntryType ?:string;
    Journal ?:string;
    Site ?:string;
    Account ?:string;
    Debit ?: number;
    DebitString ?: string;
    Credit ?:number;
    CreditString ?: string;
    Description ?:string;
    Project ?:string;
    Branch ?:string;
    Bom ?:string;
    WorkGroup ?:string;
    CostCenter ?:string;
    FreeReference ?:string;
    Tax ?:string;
}
