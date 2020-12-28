import { LayoutRowCollection } from "./LayoutRowCollection";
import { Tab } from "./Tab";
import { Column } from "./Column";
import { WidgetPlacement } from "../Widget/WidgetPlacement";
import { LayoutRow } from "./LayoutRow";

export class Layout {

    layoutRows: LayoutRowCollection;

    private layoutRowSelector: string = ".layout-row";

    constructor(
        private readonly parent: HTMLElement,
        selector: string = null
    ) {
        this.layoutRowSelector = selector || this.layoutRowSelector;
    }

    getDom() {
        return this.parent.querySelectorAll(this.layoutRowSelector);
    }



    fromTab(tab: Tab) {
        return tab.getDom().querySelectorAll(this.layoutRowSelector);
    }

    getLayoutRows() {
        if (!this.layoutRows) {
            this.layoutRows = new LayoutRowCollection(this.parent);
        }
        if (this.layoutRows) {
            return this.layoutRows.getLayoutRows();
        }
        throw new Error("No layout rows were found.");
    }

    getFirstLayoutRow() {
        if (this.layoutRows) {
            return this.layoutRows.getLayoutRows()[0];
        }
        return null;
    }

    getColumns() {
        const result: Column[] = [];

        const rows = this.getLayoutRows();
        rows.map((row: LayoutRow, index: number) => {
            const columns = row.getColumns();
            result.push(...columns);
        });
        return result;
    }

    getWidgetPlacements() {
        const widgets: WidgetPlacement[] = [];
        const rows = this.getLayoutRows();
        rows.map((row: LayoutRow, index: number) => {
            const columns = row.getColumns();
            columns.map((col: Column, num: number) => {
                const items = col.getWidgetCollection().getWidgets();
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