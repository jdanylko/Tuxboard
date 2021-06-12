import { TuxbarButton } from "./TuxbarButton";
import { Tuxbar } from "./Tuxbar";
import { ChangeLayoutDialog } from "../Dialogs/ChangeLayoutDialog/ChangeLayoutDialog";
import { Tuxboard } from "../../Tuxboard";
import { Modal } from "bootstrap";
import { ChangeLayoutService } from "../Dialogs/ChangeLayoutDialog/ChangeLayoutService";

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
            element.addEventListener("click", (ev: Event) => { this.changeLayoutClick(ev, tuxBar); }, false);
        }
    }

    getDom() {
        return this.tuxBar.getDom().querySelector(this.selector)
    }

    changeLayoutClick(ev: Event, tuxbar: Tuxbar) {

        const button: HTMLElement = ev.currentTarget as HTMLElement;
        if (!button)
            return;

        const dashboard: Tuxboard = tuxbar.getTuxboard();
        const tab = dashboard.getTab();
        const dialog = new ChangeLayoutDialog(dashboard);

        dialog.getDom().addEventListener('show.bs.modal',
            e => {
                this.service.getLayoutDialog(tab.getCurrentTabId())
                    .then((result: string) => {
                        dialog.initialize(result);
                        dialog.hideOverlay();
                    });
            },
            { once: true });

        const bsDialog = new Modal(dialog.getLayoutDialog());
        bsDialog.show();
    }
}
