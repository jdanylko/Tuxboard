import { WidgetToolBar } from "./WidgetToolBar";

export class WidgetToolbarButton {

    private name: string;

    constructor(
        protected readonly parent: WidgetToolBar,
        protected selector : string) {
    }

    getDom() { return this.parent.getDom().querySelector(this.selector); }

    setName(name: string) { this.name = name; }
    getName() { return name; }

    getSelector() { return this.selector; }
}