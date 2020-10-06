import { dataId, getClosestByClass, noPeriod } from "./common";
import { WidgetCollection } from "../Widget/WidgetCollection";
import { DragWidgetInfo as DragInfo} from "../Models/DragWidgetInfo";

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

    getDom() { return this.parent; }
    getAttributeName() { return dataId }
    getIndex() { return this.index; }
    setIndex(value: number) { this.index = value; }
    setLayoutRowId(value: string) { this.layoutRowId = value; }

    getSelector() {
        return this.selector;
    }

    getColumnSelector() {
        // ".column[data-column='1']"
        return `${this.selector}:nth-child(${this.index+1})`;
    }

    getWidgetCollection() {
        if (!this.widgets) {
            this.widgets = new WidgetCollection(this.parent, this.index, this.layoutRowId);
        }
        return this.widgets;
    }

    getPlacement(placementId: string) {
        const item = this.getWidgetCollection().getWidgets().find((item, index) => item.getPlacementId() === placementId);
        return item.getDom();
    }

    getColumnByPlacement(placementId:string) {
        const placement = this.getPlacement(placementId);
        return getClosestByClass(placement, noPeriod(this.columnSelector));
    }
}