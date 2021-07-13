export class PlacementItem {
    constructor(
        public PlacementId: string,
        public Index: number,
        public LayoutRowId: string,
        public ColumnIndex: number,
        public Static: boolean = false) { }
}