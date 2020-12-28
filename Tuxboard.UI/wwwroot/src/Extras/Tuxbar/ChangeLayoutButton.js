"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
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
        dialog.getDom().addEventListener('show.bs.modal', (e) => __awaiter(this, void 0, void 0, function* () {
            yield this.service.getLayoutDialog(tab.getCurrentTabId())
                .then((result) => {
                dialog.initialize(result);
                dialog.hideOverlay();
            });
        }), { once: true });
        const bsDialog = new bootstrap_1.Modal(dialog.getLayoutDialog());
        bsDialog.show();
    }
}
exports.ChangeLayoutButton = ChangeLayoutButton;
//# sourceMappingURL=ChangeLayoutButton.js.map