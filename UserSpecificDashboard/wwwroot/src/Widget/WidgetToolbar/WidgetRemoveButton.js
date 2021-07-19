import { dataId } from "../../core/common";
import { TuxboardService } from "../../Services/TuxboardService";
import { WidgetToolbarButton } from "./WidgetToolbarButton";
export class WidgetRemoveButton extends WidgetToolbarButton {
    constructor(parent, buttonSelector = null) {
        super(parent, buttonSelector);
        this.removeWidgetButtonSelector = ".remove-widget";
        this.service = new TuxboardService();
        this.selector = buttonSelector || this.removeWidgetButtonSelector;
        this.setName("removeButton");
        const element = this.getDom();
        if (element) {
            element.addEventListener("click", (ev) => { this.removeWidget(ev, parent); }, false);
        }
    }
    removeWidget(ev, toolbar) {
        const button = ev.currentTarget;
        if (!button) {
            return;
        }
        const placementId = button.getAttribute(dataId);
        this.service.removeWidgetService(placementId)
            .then((result) => {
            if (result && result.success) {
                const widgetId = `[${dataId}='${result.id}']`;
                const widget = document.querySelector(widgetId);
                if (widget) {
                    widget.remove();
                }
            }
        });
    }
}
