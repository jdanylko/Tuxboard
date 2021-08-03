import { Tuxbar } from "./Tuxbar";
import { TuxbarButton } from "./TuxbarButton";

export class RefreshButton extends TuxbarButton {

    private tuxbarRefreshButtonSelector: string = "#refresh-button";

    constructor(
        tuxBar: Tuxbar,
        changeLayoutSelector: string = null) {

        super(tuxBar, changeLayoutSelector);

        this.selector = changeLayoutSelector || this.tuxbarRefreshButtonSelector;

        const element = this.getDom();
        if (element) {
            element.addEventListener("click", (ev: Event) => {
                ev.preventDefault();
                ev.stopPropagation();
                this.tuxBar.getTuxboard().refresh();
            }, false);
        }
    }

    getDom() {
        return this.tuxBar.getDom().querySelector(this.selector)
    }
}