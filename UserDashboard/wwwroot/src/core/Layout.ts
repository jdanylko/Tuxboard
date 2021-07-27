import { Column } from "./Column";
import { LayoutRow } from "./LayoutRow";
import { LayoutRowCollection } from "./LayoutRowCollection";
import { Tab } from "./Tab";
import { WidgetPlacement } from "../Widget/WidgetPlacement";

export class Layout {

    public layoutRows: LayoutRowCollection;

    private layoutRowSelector: string = ".layout-row";

    constructor(
        private readonly parent: HTMLElement,
        selector: string = null)
    {
        this.layoutRowSelector = selector || this.layoutRowSelector;
    }

    public getDom() {
        return this.parent.querySelectorAll(this.layoutRowSelector);
    }

    public fromTab(tab: Tab) {
        return tab.getDom().querySelectorAll(this.layoutRowSelector);
    }

    public getLayoutRows() {
        if (!this.layoutRows) {
            this.layoutRows = new LayoutRowCollection(this.parent);
        }
        if (this.layoutRows) {
            return this.layoutRows.getLayoutRows();
        }
        throw new Error("No layout rows were found.");
    }

    public getFirstLayoutRow() {
        if (this.layoutRows) {
            return this.layoutRows.getLayoutRows()[0];
        }
        return null;
    }

    public getColumns() {
        const result: Column[] = [];

        const rows = this.getLayoutRows();
        rows.map((row: LayoutRow, index: number) => {
            const columns = row.getColumns();
            result.push(...columns);
        });
        return result;
    }

    public getWidgetPlacements(token:string = "") {
        const widgets: WidgetPlacement[] = [];
        const rows = this.getLayoutRows();
        rows.map((row: LayoutRow, index: number) => {
            const columns = row.getColumns();
            columns.map((col: Column, num: number) => {
                const items = col.getWidgetCollection().getWidgets(token);
                widgets.push(...items);
            });
        });
        //const columns = this.getColumns();
        //columns.map((col: Column, num: number) => {
        //    const items = col.getWidgetCollection().getWidgets();
        //    widgets.push(...items);
        //});
        return widgets;
    }
}
