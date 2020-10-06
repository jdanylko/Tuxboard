"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.WidgetToolbarButton = void 0;
class WidgetToolbarButton {
    constructor(parent, selector) {
        this.parent = parent;
        this.selector = selector;
    }
    getDom() { return this.parent.getDom().querySelector(this.selector); }
    setName(name) { this.name = name; }
    getName() { return name; }
    getSelector() { return this.selector; }
}
exports.WidgetToolbarButton = WidgetToolbarButton;
//# sourceMappingURL=WidgetToolbarButton.js.map