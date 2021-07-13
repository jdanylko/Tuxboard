﻿import { Column } from "./Column";
import { LayoutRow } from "./LayoutRow";

export class LayoutRowCollection {

    private layoutRowSelector:string = ".layout-row";

    constructor(
        private readonly parent: HTMLElement,
        selector: string = null)
    {
        this.layoutRowSelector = selector || this.layoutRowSelector;
    }

    public fromLayout() {
        if (this.layoutRowSelector && this.parent) {
            const rows = Array.from(this.parent.querySelectorAll<HTMLElement>(this.layoutRowSelector));
            if (rows) {
                return rows.map((element, index) => this.createLayoutRow(element, index));
            }
        }
        throw new Error("No layout rows were found.");
    }

    public createLayoutRow(element: HTMLElement, index: number) {
        const row = new LayoutRow(this.parent);
        const id = element.getAttribute(row.getAttributeName());
        row.setId(id);
        row.setIndex(index);
        return row;
    }

    public getLayoutRows() {
        return this.fromLayout();
    }
}