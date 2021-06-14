import { Column } from "./Column";
import { ColumnCollection } from "./ColumnCollection";
import { dataId } from "./common";

export class LayoutRow {

    private layoutRowSelector: string = ".layout-row";

    private id: string;
    private index: number;
    private layoutType: number;
    private columns: ColumnCollection;

    constructor(
        private readonly parent: HTMLElement,
        selector: string = null
    ) {
        this.layoutRowSelector = selector || this.layoutRowSelector;
    }

    public getColumns() {
        if (!this.columns) {
            this.columns = new ColumnCollection(this.getDom(), this.id);
        }
        return this.columns.fromLayoutRow();
    }

    public getWidgets() {
        return this.getColumns().map((elem: Column, index: number) => {
            return elem.getWidgetCollection();
        });
    }

    public getDom() {
         return this.parent.querySelector(this.getSelector()) as HTMLElement;
    }

    public getAttributeName() {
        return dataId
    }

    public setId(value: string) {
        this.id = value;
    }

    public setIndex(value: number) {
        this.index = value;
    }

    private getSelector() {
        // ".layout-row[data-id='9AF39A45-D9FE-493A-8BB5-1F12218D03CD']"
        return `${this.layoutRowSelector}[${this.getAttributeName()}='${this.id}']`;
    }
}