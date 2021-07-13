﻿import { PlacementItem } from "./PlacementItem";

export class DragWidgetInfo {

    public placementList: PlacementItem[];

    constructor(
        public placementId: string,
        public currentColumnIndex: number,
        public currentLayoutRowId: string,
        public previousColumnIndex: number,
        public previousLayoutRowId: string,
    ) { }
}
