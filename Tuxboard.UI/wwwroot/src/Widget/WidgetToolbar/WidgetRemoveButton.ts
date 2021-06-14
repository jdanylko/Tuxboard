import { dataId } from "../../core/common";
import { TuxboardService } from "../../Services/TuxboardService";
import { TuxViewMessage } from "../../Models/TuxViewMessage";
import { WidgetToolbarButton } from "./WidgetToolbarButton";
import { WidgetToolBar } from "./WidgetToolBar";

export class WidgetRemoveButton extends WidgetToolbarButton {

    private removeWidgetButtonSelector: string = ".remove-widget";

    private service: TuxboardService = new TuxboardService();

    constructor(
        parent: WidgetToolBar,
        buttonSelector: string = null) {

        super(parent, buttonSelector);

        this.selector = buttonSelector || this.removeWidgetButtonSelector;

        this.setName("removeButton");

        const element = this.getDom();
        if (element) {
            element.addEventListener("click", (ev: Event) => { this.removeWidget(ev, parent); }, false);
        }
    }

    public removeWidget(ev: Event, toolbar: WidgetToolBar) {

        const button: HTMLElement = ev.currentTarget as HTMLElement;
        if (!button) {
            return;
        }

        const placementId = button.getAttribute(dataId);

        this.service.removeWidgetService(placementId)
            .then((result: TuxViewMessage) => {
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
