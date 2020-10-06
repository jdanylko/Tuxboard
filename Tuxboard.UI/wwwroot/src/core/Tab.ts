import { Layout } from "./Layout";
import { dataId } from "./common";

export class Tab {

    private tabSelector: string = ".dashboard-tab";

    layout: Layout;

    constructor(
        private readonly parent: HTMLElement,
        selector: string = null
    ) {
        this.tabSelector = selector || this.tabSelector;
    }

    getDom() {
        return this.parent.querySelector(this.tabSelector) as HTMLElement;
    }

    getLayout() {
        if (!this.layout) {
            this.layout = new Layout(this.getDom());
        }
        return this.layout;
    }

    getCurrentTab() {
        return document.querySelector<HTMLElement>(this.tabSelector + "[data-active='true']");
    }

    getCurrentTabId() {
        const tab = this.getCurrentTab();
        return tab.getAttribute(dataId);
    }


}