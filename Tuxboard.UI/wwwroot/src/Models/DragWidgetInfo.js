export class DragWidgetInfo {
    constructor(placementId, currentColumnIndex, currentLayoutRowId, previousColumnIndex, previousLayoutRowId) {
        this.placementId = placementId;
        this.currentColumnIndex = currentColumnIndex;
        this.currentLayoutRowId = currentLayoutRowId;
        this.previousColumnIndex = previousColumnIndex;
        this.previousLayoutRowId = previousLayoutRowId;
    }
}
