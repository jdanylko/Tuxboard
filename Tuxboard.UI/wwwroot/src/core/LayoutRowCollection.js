"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.LayoutRowCollection = void 0;
const LayoutRow_1 = require("./LayoutRow");
class LayoutRowCollection {
    constructor(parent, selector = null) {
        this.parent = parent;
        this.layoutRowSelector = ".layout-row";
        this.layoutRowSelector = selector || this.layoutRowSelector;
    }
    fromLayout() {
        return Array.from(this.parent.querySelectorAll(this.layoutRowSelector))
            .map((element, index) => this.createLayoutRow(element, index));
    }
    createLayoutRow(element, index) {
        const row = new LayoutRow_1.LayoutRow(this.parent);
        const id = element.getAttribute(row.getAttributeName());
        row.setId(id);
        row.setIndex(index);
        return row;
    }
    getLayoutRows() {
        return this.fromLayout();
    }
}
exports.LayoutRowCollection = LayoutRowCollection;
//# sourceMappingURL=LayoutRowCollection.js.map