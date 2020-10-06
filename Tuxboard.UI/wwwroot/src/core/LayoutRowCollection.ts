import { LayoutRow } from "./LayoutRow";
import {Column} from "./Column";

export class LayoutRowCollection {

    private layoutRowSelector:string = ".layout-row";

    constructor(
        private readonly parent: HTMLElement,
        selector: string = null
    ) {
        this.layoutRowSelector = selector || this.layoutRowSelector;
    }

    fromLayout() {
        return Array.from(this.parent.querySelectorAll<HTMLElement>(this.layoutRowSelector))
            .map((element, index) => this.createLayoutRow(element, index));
    }

    createLayoutRow(element: HTMLElement, index: number) {
        const row = new LayoutRow(this.parent);
        const id = element.getAttribute(row.getAttributeName());
        row.setId(id);
        row.setIndex(index);
        return row;
    }

    getLayoutRows() {
        return this.fromLayout();
    }

    //getColumns() {
    //    const columns: Column[] = [];
    //    this.fromLayout().map((elem: LayoutRow, index: number) => {
    //        columns.push(...elem.getColumns());
    //    });
    //    return columns;
    //}

}