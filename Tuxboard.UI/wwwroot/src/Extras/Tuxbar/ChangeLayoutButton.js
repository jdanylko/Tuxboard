import { ChangeLayoutDialog } from "../Dialogs/ChangeLayoutDialog/ChangeLayoutDialog";
import { ChangeLayoutService } from "../Dialogs/ChangeLayoutDialog/ChangeLayoutService";
import { Modal } from "bootstrap";
import { TuxbarButton } from "./TuxbarButton";
export class ChangeLayoutButton extends TuxbarButton {
    constructor(tuxBar, changeLayoutSelector = null) {
        super(tuxBar, changeLayoutSelector);
        this.tuxbarChangeLayoutButtonSelector = "#layout-button";
        this.service = new ChangeLayoutService();
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
        if (!button) {
            return;
        }
        const dashboard = tuxbar.getTuxboard();
        const tab = dashboard.getTab();
        const dialog = new ChangeLayoutDialog(dashboard);
        dialog.getDom().addEventListener("shown.bs.modal", (e) => {
            this.service.getLayoutDialog(tab.getCurrentTabId())
                .then((result) => {
                dialog.initialize(result);
                dialog.hideOverlay();
            });
        }, { once: true });
        const bsDialog = new Modal(dialog.getLayoutDialog());
        bsDialog.show();
    }
}
