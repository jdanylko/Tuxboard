"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Column = void 0;
const common_1 = require("./common");
const WidgetCollection_1 = require("../Widget/WidgetCollection");
class Column {
    constructor(parent, selector = null) {
        this.parent = parent;
        this.selector = selector;
        this.columnSelector = ".column";
        this.columnSelector = selector || this.columnSelector;
    }
    getDom() { return this.parent; }
    getAttributeName() { return common_1.dataId; }
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
            this.widgets = new WidgetCollection_1.WidgetCollection(this.parent, this.index, this.layoutRowId);
        }
        return this.widgets;
    }
    getPlacement(placementId) {
        const widget = this.getWidgetCollection().getWidgets()
            .find((item, index) => item.getPlacementId() === placementId);
        return widget.getDom();
    }
    getColumnByPlacement(placementId) {
        const placement = this.getPlacement(placementId);
        return common_1.getClosestByClass(placement, common_1.noPeriod(this.columnSelector));
    }
}
exports.Column = Column;
//# sourceMappingURL=Column.js.map