export class PlacementItem {
    constructor(PlacementId, Index, LayoutRowId, ColumnIndex, Static = false) {
        this.PlacementId = PlacementId;
        this.Index = Index;
        this.LayoutRowId = LayoutRowId;
        this.ColumnIndex = ColumnIndex;
        this.Static = Static;
    }
}
