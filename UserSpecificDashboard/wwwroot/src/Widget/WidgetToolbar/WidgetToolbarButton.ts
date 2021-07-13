import { WidgetToolBar } from "./WidgetToolBar";

export class WidgetToolbarButton {

    private name: string;

    constructor(
        protected readonly parent: WidgetToolBar,
        protected selector: string = null) {
    }

    public getDom() { return this.parent.getDom().querySelector(this.selector); }

    public setName(name: string) { this.name = name; }
    public getName() { return name; }

    public getSelector() { return this.selector; }
}
