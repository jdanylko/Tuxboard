"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.AddWidgetButton = void 0;
const TuxbarButton_1 = require("./TuxbarButton");
const AddWidgetDialog_1 = require("../Dialogs/AddWidgetDialog/AddWidgetDialog");
const bootstrap_bundle_1 = require("../../../lib/bootstrap/dist/js/bootstrap.bundle");
const AddWidgetService_1 = require("../../Extras/Dialogs/AddWidgetDialog/AddWidgetService");
class AddWidgetButton extends TuxbarButton_1.TuxbarButton {
    constructor(tuxBar, addWidgetButtonSelector = null) {
        super(tuxBar, addWidgetButtonSelector);
        this.tuxbarAddWidgetSelector = "#widget-button";
        this.service = new AddWidgetService_1.AddWidgetService();
        this.selector = addWidgetButtonSelector || this.tuxbarAddWidgetSelector;
        const element = this.tuxBar.getDom().querySelector(this.selector);
        if (element) {
            element.addEventListener("click", (ev) => { this.displayDialogEvent(ev, tuxBar); }, false);
        }
    }
    getDom() {
        return this.tuxBar.getDom().querySelector(this.selector);
    }
    displayDialogEvent(ev, tuxbar) {
        const button = ev.currentTarget;
        if (!button)
            return;
        const dialog = new AddWidgetDialog_1.AddWidgetDialog(tuxbar.getTuxboard());
        dialog.getWidgetDialog().addEventListener('show.bs.modal', e => {
            this.service.getWidgetDialogService()
                .then((result) => {
                dialog.initialize(result);
                dialog.hideOverlay();
            });
        }, { once: true });
        const bsDialog = new bootstrap_bundle_1.Modal(dialog.getWidgetDialog());
        bsDialog.show();
    }
}
exports.AddWidgetButton = AddWidgetButton;
//# sourceMappingURL=AddWidgetButton.js.map