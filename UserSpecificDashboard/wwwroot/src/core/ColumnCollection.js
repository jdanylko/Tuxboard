"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ColumnCollection = void 0;
const Column_1 = require("./Column");
class ColumnCollection {
    constructor(parent, layoutRowId, selector = null) {
        this.parent = parent;
        this.layoutRowId = layoutRowId;
        this.columnSelector = ".column";
        this.columnSelector = selector || this.columnSelector;
    }
    fromLayoutRow() {
        return Array.from(this.parent.querySelectorAll(this.columnSelector))
            .map((element, index) => this.createColumn(element, index));
    }
    createColumn(element, index) {
        const column = new Column_1.Column(element);
        column.setIndex(index);
        column.setLayoutRowId(this.layoutRowId);
        return column;
    }
    getWidgets() {
        return this.fromLayoutRow().map((elem, index) => {
            return elem.getWidgetCollection();
        });
    }
}
exports.ColumnCollection = ColumnCollection;
//# sourceMappingURL=ColumnCollection.js.map