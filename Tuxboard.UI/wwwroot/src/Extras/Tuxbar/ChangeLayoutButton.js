"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ChangeLayoutButton = void 0;
const TuxbarButton_1 = require("./TuxbarButton");
const ChangeLayoutDialog_1 = require("../Dialogs/ChangeLayoutDialog/ChangeLayoutDialog");
const bootstrap_1 = require("bootstrap");
const ChangeLayoutService_1 = require("../Dialogs/ChangeLayoutDialog/ChangeLayoutService");
class ChangeLayoutButton extends TuxbarButton_1.TuxbarButton {
    constructor(tuxBar, changeLayoutSelector = null) {
        super(tuxBar, changeLayoutSelector);
        this.tuxbarChangeLayoutButtonSelector = "#layout-button";
        this.service = new ChangeLayoutService_1.ChangeLayoutService();
        this.selector = changeLayoutSelector || this.tuxbarChangeLayoutButtonSelector;
        const element = this.getDom();
        if (element) {
            element.addEventListener("click", (ev) => { this.changeLayoutClick(ev, tuxBar); }, false);
        }
    }
    getDom() {
        return this.tuxBar.getDom().querySelector(this.selector);
    }
    changeLayoutClick(ev, tuxbar) {
        const button = ev.currentTarget;
        if (!button)
            return;
        const dashboard = tuxbar.getTuxboard();
        const tab = dashboard.getTab();
        const dialog = new ChangeLayoutDialog_1.ChangeLayoutDialog(dashboard);
        dialog.getDom().addEventListener('show.bs.modal', e => {
            this.service.getLayoutDialog(tab.getCurrentTabId())
                .then((result) => {
                dialog.initialize(result);
                dialog.hideOverlay();
            });
        }, { once: true });
        const bsDialog = new bootstrap_1.Modal(dialog.getLayoutDialog());
        bsDialog.show();
    }
}
exports.ChangeLayoutButton = ChangeLayoutButton;
//# sourceMappingURL=ChangeLayoutButton.js.map