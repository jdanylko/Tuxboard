import { TuxbarButton } from "./TuxbarButton";
import { Tuxbar } from "./Tuxbar";
import { AddWidgetDialog } from "../Dialogs/AddWidgetDialog/AddWidgetDialog";
import { Modal, Tab } from "bootstrap";
import { AddWidgetService } from "../../Extras/Dialogs/AddWidgetDialog/AddWidgetService";

export class AddWidgetButton extends TuxbarButton {

    private tuxbarAddWidgetSelector: string = "#widget-button";

    private service: AddWidgetService = new AddWidgetService();

    constructor(
        tuxBar: Tuxbar,
        addWidgetButtonSelector: string = null) {

        super(tuxBar, addWidgetButtonSelector);

        this.selector = addWidgetButtonSelector || this.tuxbarAddWidgetSelector;

        const element = this.tuxBar.getDom().querySelector(this.selector);
        if (element) {
            element.addEventListener("click", (ev: Event) => { this.displayDialogEvent(ev, tuxBar); }, false);
        }
    }

    getDom() {
        return this.tuxBar.getDom().querySelector(this.selector);
    }

    displayDialogEvent(ev: Event, tuxbar: Tuxbar) {

        const button: HTMLElement = ev.currentTarget as HTMLElement;
        if (!button)
            return;

        const dialog = new AddWidgetDialog(tuxbar.getTuxboard());
        dialog.getWidgetDialog().addEventListener('show.bs.modal',
            e => {
                this.service.getWidgetDialogService()
                    .then((result: string) => {
                        dialog.initialize(result);
                        dialog.hideOverlay();
                    });
            },
            { once: true });

        const bsDialog = new Modal(dialog.getWidgetDialog());
        bsDialog.show();
    }

}