import { dataId } from "./common";
import { Layout } from "./Layout";

export class Tab {

    private tabSelector: string = ".dashboard-tab";

    private layout: Layout;

    constructor(
        private readonly parent: HTMLElement,
        selector: string = null
    ) {
        this.tabSelector = selector || this.tabSelector;
    }

    public getDom() {
        return this.parent.querySelector(this.tabSelector) as HTMLElement;
    }

    public getLayout() {
        if (!this.layout) {
            this.layout = new Layout(this.getDom());
        }
        return this.layout;
    }

    public getCurrentTab() {
        return document.querySelector<HTMLElement>(this.tabSelector + "[data-active='true']");
    }

    public getCurrentTabId() {
        const tab = this.getCurrentTab();
        return tab.getAttribute(dataId);
    }
}
