import { LayoutRow } from "./LayoutRow";
export class LayoutRowCollection {
    constructor(parent, selector = null) {
        this.parent = parent;
        this.layoutRowSelector = ".layout-row";
        this.layoutRowSelector = selector || this.layoutRowSelector;
    }
    fromLayout() {
        if (this.layoutRowSelector && this.parent) {
            const rows = Array.from(this.parent.querySelectorAll(this.layoutRowSelector));
            if (rows) {
                return rows.map((element, index) => this.createLayoutRow(element, index));
            }
        }
        throw new Error("No layout rows were found.");
    }
    createLayoutRow(element, index) {
        const row = new LayoutRow(this.parent);
        const id = element.getAttribute(row.getAttributeName());
        row.setId(id);
        row.setIndex(index);
        return row;
    }
    getLayoutRows() {
        return this.fromLayout();
    }
}
