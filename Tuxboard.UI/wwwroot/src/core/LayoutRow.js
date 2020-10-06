"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.LayoutRow = void 0;
const common_1 = require("./common");
const ColumnCollection_1 = require("./ColumnCollection");
class LayoutRow {
    constructor(parent, selector = null) {
        this.parent = parent;
        this.layoutRowSelector = ".layout-row";
        this.layoutRowSelector = selector || this.layoutRowSelector;
    }
    getDom() { return this.parent.querySelector(this.getSelector()); }
    getAttributeName() { return common_1.dataId; }
    setId(value) { this.id = value; }
    setIndex(value) { this.index = value; }
    getSelector() {
        // ".layout-row[data-id='9AF39A45-D9FE-493A-8BB5-1F12218D03CD']"
        return `${this.layoutRowSelector}[${this.getAttributeName()}='${this.id}']`;
    }
    getColumns() {
        if (!this.columns) {
            this.columns = new ColumnCollection_1.ColumnCollection(this.getDom(), this.id);
        }
        return this.columns.fromLayoutRow();
    }
    getWidgets() {
        return this.getColumns().map((elem, index) => {
            return elem.getWidgetCollection();
        });
    }
}
exports.LayoutRow = LayoutRow;
//# sourceMappingURL=LayoutRow.js.map