import { WidgetToolbarButton } from "./WidgetToolbarButton";
import { dataId, getDomWidget } from "../../core/common";
import { TuxboardService } from "../../Services/TuxboardService";
export class WidgetCollapseButton extends WidgetToolbarButton {
    constructor(parent, buttonSelector = null) {
        super(parent, buttonSelector);
        this.collapsedButtonSelector = ".collapse-widget";
        this.collapsedToggleSelector = "collapsed";
        this.service = new TuxboardService();
        this.selector = buttonSelector || this.collapsedButtonSelector;
        this.setName("collapseButton");
        const element = this.getDom();
        if (element) {
            element.addEventListener("click", (ev) => { this.minimizeWidget(ev, parent); }, false);
        }
    }
    minimizeWidget(ev, toolbar) {
        const button = ev.currentTarget;
        if (!button)
            return;
        const placementId = button.getAttribute(dataId);
        const widget = toolbar.getWidgetPlacement().getDom();
        if (widget) {
            const minimized = widget.classList.contains(this.collapsedToggleSelector);
            if (minimized) {
                widget.classList.remove(this.collapsedToggleSelector);
                this.showWidgetBody(placementId);
                this.service.updateCollapsedWidgetService(placementId, false)
                    .then((data) => { });
            }
            else {
                widget.classList.add(this.collapsedToggleSelector);
                this.hideWidgetBody(placementId);
                this.service.updateCollapsedWidgetService(placementId, true)
                    .then((data) => { });
            }
        }
    }
    getWidgetBody(placementId) {
        const widget = getDomWidget(placementId);
        if (widget)
            return widget.querySelector(".card"); // TODO: DefaultSelector.getInstance().widgetBodySelector);
        return null;
    }
    hideWidgetBody(placementId) {
        const body = this.getWidgetBody(placementId);
        if (body) {
            body.setAttribute("hidden", "");
        }
    }
    showWidgetBody(placementId) {
        const body = this.getWidgetBody(placementId);
        if (body) {
            body.removeAttribute("hidden");
        }
    }
}
