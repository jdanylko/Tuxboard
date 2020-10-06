"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.PlacementItem = void 0;
class PlacementItem {
    constructor(PlacementId, Index, LayoutRowId, ColumnIndex, Static = false) {
        this.PlacementId = PlacementId;
        this.Index = Index;
        this.LayoutRowId = LayoutRowId;
        this.ColumnIndex = ColumnIndex;
        this.Static = Static;
    }
}
exports.PlacementItem = PlacementItem;
//# sourceMappingURL=PlacementItem.js.map