import { ChangeLayoutDialog } from "../Dialogs/ChangeLayoutDialog/ChangeLayoutDialog";
import { ChangeLayoutService } from "../Dialogs/ChangeLayoutDialog/ChangeLayoutService";
import { Modal } from "bootstrap";
import { Tuxbar } from "./Tuxbar";
import { TuxbarButton } from "./TuxbarButton";
import { Tuxboard } from "../../Tuxboard";

export class ChangeLayoutButton extends TuxbarButton {

    private tuxbarChangeLayoutButtonSelector: string = "#layout-button";

    private service: ChangeLayoutService = new ChangeLayoutService();

    constructor(
        tuxBar: Tuxbar,
        changeLayoutSelector: string = null) {

        super(tuxBar, changeLayoutSelector);

        this.selector = changeLayoutSelector || this.tuxbarChangeLayoutButtonSelector;

        const element = this.getDom();
        if (element) {
            element.addEventListener("click", (ev: Event) => {
                ev.preventDefault();
                ev.stopPropagation();
                this.changeLayoutClick(ev, tuxBar);
            }, false);
        }
    }

    public getDom() {
        return this.tuxBar.getDom().querySelector(this.selector)
    }

    public changeLayoutClick(ev: Event, tuxbar: Tuxbar) {

        const button: HTMLElement = ev.currentTarget as HTMLElement;
        if (!button) {
            return;
        }

        const dashboard: Tuxboard = tuxbar.getTuxboard();
        const tab = dashboard.getTab();
        const dialog = new ChangeLayoutDialog(dashboard);

        dialog.getDom().addEventListener("shown.bs.modal",
            (e): void => {
                this.service.getLayoutDialog(tab.getCurrentTabId())
                    .then((result: string) => {
                        dialog.initialize(result);
                        dialog.hideOverlay();
                    });
            },
            { once: false });

        const bsDialog = new Modal(dialog.getLayoutDialog());
        bsDialog.show();
    }
}
