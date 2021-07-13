"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.WidgetCollapseButton = void 0;
const common_1 = require("../../core/common");
const TuxboardService_1 = require("../../Services/TuxboardService");
const WidgetToolbarButton_1 = require("./WidgetToolbarButton");
class WidgetCollapseButton extends WidgetToolbarButton_1.WidgetToolbarButton {
    constructor(parent, buttonSelector = null) {
        super(parent, buttonSelector);
        this.collapsedButtonSelector = ".collapse-widget";
        this.collapsedToggleSelector = "collapsed";
        this.service = new TuxboardService_1.TuxboardService();
        this.selector = buttonSelector || this.collapsedButtonSelector;
        this.setName("collapseButton");
        const element = this.getDom();
        if (element) {
            element.addEventListener("click", (ev) => { this.minimizeWidget(ev, parent); }, false);
        }
    }
    minimizeWidget(ev, toolbar) {
        const button = ev.currentTarget;
        if (!button) {
            return;
        }
        const placementId = button.getAttribute(common_1.dataId);
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
                    .then((data) => { return; });
            }
        }
    }
    getWidgetBody(placementId) {
        const widget = common_1.getDomWidget(placementId);
        // TODO: DefaultSelector.getInstance().widgetBodySelector);
        if (widget) {
            return widget.querySelector(".card");
        }
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
exports.WidgetCollapseButton = WidgetCollapseButton;
//# sourceMappingURL=WidgetCollapseButton.js.map