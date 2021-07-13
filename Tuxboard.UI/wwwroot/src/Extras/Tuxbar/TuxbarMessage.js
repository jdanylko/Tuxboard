export class TuxbarMessage {
    constructor(tuxBar, selector = null) {
        this.tuxBar = tuxBar;
        this.selector = selector;
        this.tuxbarMessageSelector = "#tuxbar-status";
        this.tuxbarMessageSelector = selector || this.tuxbarMessageSelector;
    }
    getDom() {
        return this.tuxBar.getDom().querySelector(this.tuxbarMessageSelector);
    }
    setMessage(message, fade) {
        const control = this.getDom();
        if (control) {
            control.innerHTML = message;
        }
    }
}
