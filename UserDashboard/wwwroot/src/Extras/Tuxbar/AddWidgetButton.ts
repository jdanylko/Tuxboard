import { AddWidgetDialog } from "../Dialogs/AddWidgetDialog/AddWidgetDialog";
import { AddWidgetService } from "../../Extras/Dialogs/AddWidgetDialog/AddWidgetService";
import { Modal, Tab } from "bootstrap";
import { Tuxbar } from "./Tuxbar";
import { TuxbarButton } from "./TuxbarButton";

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
            element.addEventListener("click", (ev: Event) => {
                ev.preventDefault();
                ev.stopPropagation();
                this.displayDialogEvent(ev, tuxBar);
            }, false);
        }
    }

    public getDom() {
        return this.tuxBar.getDom().querySelector(this.selector);
    }

    public displayDialogEvent(ev: Event, tuxbar: Tuxbar) {

        const button: HTMLElement = ev.currentTarget as HTMLElement;
        if (!button) {
            return;
        }

        const dialog = new AddWidgetDialog(tuxbar.getTuxboard());
        dialog.getWidgetDialog().addEventListener("show.bs.modal",
            () => {
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
