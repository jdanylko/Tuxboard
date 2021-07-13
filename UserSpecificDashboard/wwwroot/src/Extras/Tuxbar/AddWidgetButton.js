import { AddWidgetDialog } from "../Dialogs/AddWidgetDialog/AddWidgetDialog";
import { AddWidgetService } from "../../Extras/Dialogs/AddWidgetDialog/AddWidgetService";
import { Modal } from "bootstrap";
import { TuxbarButton } from "./TuxbarButton";
export class AddWidgetButton extends TuxbarButton {
    constructor(tuxBar, addWidgetButtonSelector = null) {
        super(tuxBar, addWidgetButtonSelector);
        this.tuxbarAddWidgetSelector = "#widget-button";
        this.service = new AddWidgetService();
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
        if (!button) {
            return;
        }
        const dialog = new AddWidgetDialog(tuxbar.getTuxboard());
        dialog.getWidgetDialog().addEventListener("show.bs.modal", () => {
            this.service.getWidgetDialogService()
                .then((result) => {
                dialog.initialize(result);
                dialog.hideOverlay();
            });
        }, { once: true });
        const bsDialog = new Modal(dialog.getWidgetDialog());
        bsDialog.show();
    }
}
