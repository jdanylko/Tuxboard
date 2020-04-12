export class Tab {

    public static tabId: string = ".dashboard-tab";

    constructor() { }

    static getCurrentTab() { return document.querySelector<HTMLDivElement>(Tab.tabId + "[data-active='true']").attributes["data-id"]; }

    getLayout() {

    }
}