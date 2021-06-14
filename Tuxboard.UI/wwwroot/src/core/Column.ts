import { dataId, getClosestByClass, noPeriod } from "./common";
import { DragWidgetInfo as DragInfo } from "../Models/DragWidgetInfo";
import { WidgetCollection } from "../Widget/WidgetCollection";

export class Column {

    private columnSelector: string = ".column";

    index: number;
    layoutRowId: string;
    widgets: WidgetCollection;

    public dragInfo: DragInfo;

    constructor(
        private readonly parent: HTMLElement,
        private readonly selector: string = null
    ) {
        this.columnSelector = selector || this.columnSelector;
    }

    public getDom() { return this.parent; }
    public getAttributeName() { return dataId }
    public getIndex() { return this.index; }
    public setIndex(value: number) { this.index = value; }
    public setLayoutRowId(value: string) { this.layoutRowId = value; }

    public getSelector() {
        return this.selector;
    }

    public getColumnSelector() {
        // ".column[data-column='1']"
        return `${this.selector}:nth-child(${this.index+1})`;
    }

    public getWidgetCollection() {
        if (!this.widgets) {
            this.widgets = new WidgetCollection(this.parent, this.index, this.layoutRowId);
        }
        return this.widgets;
    }

    public getPlacement(placementId: string) {
        const item = this.getWidgetCollection().getWidgets().find((item, index) => item.getPlacementId() === placementId);
        return item.getDom();
    }

    public getColumnByPlacement(placementId:string) {
        const placement = this.getPlacement(placementId);
        return getClosestByClass(placement, noPeriod(this.columnSelector));
    }
}