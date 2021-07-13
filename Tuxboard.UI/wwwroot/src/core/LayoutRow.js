import { ColumnCollection } from "./ColumnCollection";
import { dataId } from "./common";
export class LayoutRow {
    constructor(parent, selector = null) {
        this.parent = parent;
        this.layoutRowSelector = ".layout-row";
        this.layoutRowSelector = selector || this.layoutRowSelector;
    }
    getColumns() {
        if (!this.columns) {
            this.columns = new ColumnCollection(this.getDom(), this.id);
        }
        return this.columns.fromLayoutRow();
    }
    getWidgets() {
        return this.getColumns().map((elem, index) => {
            return elem.getWidgetCollection();
        });
    }
    getDom() {
        return this.parent.querySelector(this.getSelector());
    }
    getAttributeName() {
        return dataId;
    }
    setId(value) {
        this.id = value;
    }
    setIndex(value) {
        this.index = value;
    }
    getSelector() {
        // ".layout-row[data-id='9AF39A45-D9FE-493A-8BB5-1F12218D03CD']"
        return `${this.layoutRowSelector}[${this.getAttributeName()}='${this.id}']`;
    }
}
