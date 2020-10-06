import { WidgetPlacement } from "../Widget/WidgetPlacement";

export class WidgetCollection {

    private widgetSelector: string = ".card";

    constructor(
        private readonly parent: HTMLElement,
        private readonly columnIndex: number,
        private readonly layoutRowId: string,
        selector: string = null
    ) {
        this.widgetSelector = selector || this.widgetSelector;
    }

    getWidgetSelector() {
        return this.widgetSelector;
    }

    getWidgets() {
        return Array.from(this.parent.querySelectorAll<HTMLElement>(this.widgetSelector))
            .map((element, index) => this.createWidget(element, index));
    }

    createWidget(element: HTMLElement, index: number) {
        const widget = new WidgetPlacement(this.parent);
        const id = element.getAttribute(widget.getAttributeName());
        widget.setPlacementId(id);
        widget.setIndex(index);
        widget.setColumnIndex(this.columnIndex);

        return widget;
    }

    getWidgetProperties(widget: WidgetPlacement) {
        return widget.getProperties();
    }
}