import { TuxbarButton } from "./TuxbarButton";
import { Tuxbar } from "./Tuxbar";

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
                 this.tuxBar.getTuxboard().refresh();
            }, false);
        }
    }

    getDom() {
        return this.tuxBar.getDom().querySelector(this.selector)
    }
}