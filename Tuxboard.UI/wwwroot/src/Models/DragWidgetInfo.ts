import { PlacementItem } from "./PlacementItem";

export class DragWidgetInfo {
    constructor(
        public placementId: string,
        public currentColumnIndex: number,
        public currentLayoutRowId: string,
        public previousColumnIndex: number,
        public previousLayoutRowId: string,
    ) { }

    placementList: PlacementItem[];
}