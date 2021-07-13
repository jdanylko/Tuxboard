import { Column } from "./Column";
export class ColumnCollection {
    constructor(parent, layoutRowId, selector = null) {
        this.parent = parent;
        this.layoutRowId = layoutRowId;
        this.columnSelector = ".column";
        this.columnSelector = selector || this.columnSelector;
    }
    fromLayoutRow() {
        return Array.from(this.parent.querySelectorAll(this.columnSelector))
            .map((element, index) => this.createColumn(element, index));
    }
    createColumn(element, index) {
        const column = new Column(element);
        column.setIndex(index);
        column.setLayoutRowId(this.layoutRowId);
        return column;
    }
    getWidgets() {
        return this.fromLayoutRow().map((elem, index) => {
            return elem.getWidgetCollection();
        });
    }
}
