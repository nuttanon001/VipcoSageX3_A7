export interface ResueaColumn<Model> {
  columnField: string;
  columnName: string;
  canEdit?: boolean;
  cell: (row: Model) => any;
  width?: number;
}
