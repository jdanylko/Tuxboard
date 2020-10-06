import { WidgetToolbarButton } from "./WidgetToolbarButton";
import { dataId, getDomWidget } from "../../core/common";
import { TuxboardService } from "../../Services/TuxboardService";
import { WidgetToolBar } from "./WidgetToolBar";

export class WidgetCollapseButton extends WidgetToolbarButton {

    private collapsedButtonSelector: string = ".collapse-widget";
    private collapsedToggleSelector: string = "collapsed";

    private service: TuxboardService = new TuxboardService();

    constructor(
        parent: WidgetToolBar,
        buttonSelector: string = null) {

        super(parent, buttonSelector);

        this.selector = buttonSelector || this.collapsedButtonSelector;

        this.setName("collapseButton");

        const element = this.getDom();
        if (element) {
            element.addEventListener("click", (ev: Event) => { this.minimizeWidget(ev, parent); }, false);
        }
    }

    minimizeWidget(ev: Event, toolbar: WidgetToolBar) {

        const button: HTMLElement = ev.currentTarget as HTMLElement;
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
                    .then((data) => {});
            } else {
                widget.classList.add(this.collapsedToggleSelector);
                this.hideWidgetBody(placementId);
                this.service.updateCollapsedWidgetService(placementId, true)
                    .then((data) => { });
            }
        }
    }

    getWidgetBody(placementId: string) {
        const widget = getDomWidget(placementId);
        if (widget)
            return widget.querySelector(".card"); // TODO: DefaultSelector.getInstance().widgetBodySelector);
        return null;
    }
    
    hideWidgetBody(placementId:string) {
        const body = this.getWidgetBody(placementId);
        if (body) {
            body.setAttribute("hidden", "");
        }
    }

    showWidgetBody(placementId: string) {
        const body = this.getWidgetBody(placementId);
        if (body) {
            body.removeAttribute("hidden");
        }
    }



}