import { Layout } from "./Layout";
import { dataId } from "./common";
export class Tab {
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
            this.layout = new Layout(this.getDom());
        }
        return this.layout;
    }
    getCurrentTab() {
        return document.querySelector(this.tabSelector + "[data-active='true']");
    }
    getCurrentTabId() {
        const tab = this.getCurrentTab();
        return tab.getAttribute(dataId);
    }
}
