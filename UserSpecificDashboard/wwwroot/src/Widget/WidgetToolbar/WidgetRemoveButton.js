"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.WidgetRemoveButton = void 0;
const common_1 = require("../../core/common");
const TuxboardService_1 = require("../../Services/TuxboardService");
const WidgetToolbarButton_1 = require("./WidgetToolbarButton");
class WidgetRemoveButton extends WidgetToolbarButton_1.WidgetToolbarButton {
    constructor(parent, buttonSelector = null) {
        super(parent, buttonSelector);
        this.removeWidgetButtonSelector = ".remove-widget";
        this.service = new TuxboardService_1.TuxboardService();
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
        const placementId = button.getAttribute(common_1.dataId);
        this.service.removeWidgetService(placementId)
            .then((result) => {
            if (result && result.success) {
                const widgetId = `[${common_1.dataId}='${result.id}']`;
                const widget = document.querySelector(widgetId);
                if (widget) {
                    widget.remove();
                }
            }
        });
    }
}
exports.WidgetRemoveButton = WidgetRemoveButton;
//# sourceMappingURL=WidgetRemoveButton.js.map