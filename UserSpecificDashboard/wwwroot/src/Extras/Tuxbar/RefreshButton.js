import { TuxbarButton } from "./TuxbarButton";
export class RefreshButton extends TuxbarButton {
    constructor(tuxBar, changeLayoutSelector = null) {
        super(tuxBar, changeLayoutSelector);
        this.tuxbarRefreshButtonSelector = "#refresh-button";
        this.selector = changeLayoutSelector || this.tuxbarRefreshButtonSelector;
        const element = this.getDom();
        if (element) {
            element.addEventListener("click", (ev) => {
                this.tuxBar.getTuxboard().refresh();
            }, false);
        }
    }
    getDom() {
        return this.tuxBar.getDom().querySelector(this.selector);
    }
}
