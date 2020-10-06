"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Layout = void 0;
const LayoutRowCollection_1 = require("./LayoutRowCollection");
class Layout {
    constructor(parent, selector = null) {
        this.parent = parent;
        this.layoutRowSelector = ".layout-row";
        this.layoutRowSelector = selector || this.layoutRowSelector;
    }
    getDom() {
        return this.parent.querySelectorAll(this.layoutRowSelector);
    }
    fromTab(tab) {
        return tab.getDom().querySelectorAll(this.layoutRowSelector);
    }
    getLayoutRows() {
        if (!this.layoutRows) {
            this.layoutRows = new LayoutRowCollection_1.LayoutRowCollection(this.parent);
        }
        return this.layoutRows.getLayoutRows();
    }
    getFirstLayoutRow() {
        return this.layoutRows.getLayoutRows()[0];
    }
    getColumns() {
        const result = [];
        const rows = this.getLayoutRows();
        rows.map((row, index) => {
            const columns = row.getColumns();
            result.push(...columns);
        });
        return result;
    }
    getWidgetPlacements() {
        const widgets = [];
        const rows = this.getLayoutRows();
        rows.map((row, index) => {
            const columns = row.getColumns();
            columns.map((col, num) => {
                const items = col.getWidgetCollection().getWidgets();
                widgets.push(...items);
            });
        });
        //const columns = this.getColumns();
        //columns.map((col: Column, num: number) => {
        //    const items = col.getWidgetCollection().getWidgets();
        //    widgets.push(...items);
        //});
        return widgets;
    }
}
exports.Layout = Layout;
//# sourceMappingURL=Layout.js.map