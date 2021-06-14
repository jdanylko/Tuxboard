import { Column } from "./Column";

export class ColumnCollection {

    private columnSelector: string = ".column";

    constructor(
        private readonly parent: HTMLElement,
        private readonly layoutRowId: string,
        selector: string = null
    ) {
        this.columnSelector = selector || this.columnSelector;
    }

    public fromLayoutRow() {
        return Array.from(this.parent.querySelectorAll<HTMLElement>(this.columnSelector))
            .map((element: HTMLElement, index: number) => this.createColumn(element, index)
        );
    }

    public createColumn(element: HTMLElement, index: number) {
        const column = new Column(element);
        column.setIndex(index);
        column.setLayoutRowId(this.layoutRowId);
        return column;
    }

    public getWidgets() {
        return this.fromLayoutRow().map((elem: Column, index: number) => {
            return elem.getWidgetCollection();
        });
    }
}