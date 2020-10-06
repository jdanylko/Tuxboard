"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Tab = void 0;
const Layout_1 = require("./Layout");
const common_1 = require("./common");
class Tab {
    constructor(parent, selector = null) {
        this.parent = parent;
        this.tabSelector = ".dashboard-tab";
        this.tabSelector = selector || this.tabSelector;
    }
    getDom() {
        return this.parent.querySelector(this.tabSelector);
    }
    getLayout() {
        if (!this.layout) {
            this.layout = new Layout_1.Layout(this.getDom());
        }
        return this.layout;
    }
    getCurrentTab() {
        return document.querySelector(this.tabSelector + "[data-active='true']");
    }
    getCurrentTabId() {
        const tab = this.getCurrentTab();
        return tab.getAttribute(common_1.dataId);
    }
}
exports.Tab = Tab;
//# sourceMappingURL=Tab.js.map