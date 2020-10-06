"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.WidgetCollection = void 0;
const WidgetPlacement_1 = require("../Widget/WidgetPlacement");
class WidgetCollection {
    constructor(parent, columnIndex, layoutRowId, selector = null) {
        this.parent = parent;
        this.columnIndex = columnIndex;
        this.layoutRowId = layoutRowId;
        this.widgetSelector = ".card";
        this.widgetSelector = selector || this.widgetSelector;
    }
    getWidgetSelector() {
        return this.widgetSelector;
    }
    getWidgets() {
        return Array.from(this.parent.querySelectorAll(this.widgetSelector))
            .map((element, index) => this.createWidget(element, index));
    }
    createWidget(element, index) {
        const widget = new WidgetPlacement_1.WidgetPlacement(this.parent);
        const id = element.getAttribute(widget.getAttributeName());
        widget.setPlacementId(id);
        widget.setIndex(index);
        widget.setColumnIndex(this.columnIndex);
        return widget;
    }
    getWidgetProperties(widget) {
        return widget.getProperties();
    }
}
exports.WidgetCollection = WidgetCollection;
//# sourceMappingURL=WidgetCollection.js.map