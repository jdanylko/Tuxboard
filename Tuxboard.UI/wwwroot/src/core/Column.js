import { dataId, getClosestByClass, noPeriod } from "./common";
import { WidgetCollection } from "../Widget/WidgetCollection";
export class Column {
    constructor(parent, selector = null) {
        this.parent = parent;
        this.selector = selector;
        this.columnSelector = ".column";
        this.columnSelector = selector || this.columnSelector;
    }
    getDom() { return this.parent; }
    getAttributeName() { return dataId; }
    getIndex() { return this.index; }
    setIndex(value) { this.index = value; }
    setLayoutRowId(value) { this.layoutRowId = value; }
    getSelector() {
        return this.selector;
    }
    getColumnSelector() {
        // ".column[data-column='1']"
        return `${this.selector}:nth-child(${this.index + 1})`;
    }
    getWidgetCollection() {
        if (!this.widgets) {
            this.widgets = new WidgetCollection(this.parent, this.index, this.layoutRowId);
        }
        return this.widgets;
    }
    getPlacement(placementId) {
        const item = this.getWidgetCollection().getWidgets().find((item, index) => item.getPlacementId() === placementId);
        return item.getDom();
    }
    getColumnByPlacement(placementId) {
        const placement = this.getPlacement(placementId);
        return getClosestByClass(placement, noPeriod(this.columnSelector));
    }
}
