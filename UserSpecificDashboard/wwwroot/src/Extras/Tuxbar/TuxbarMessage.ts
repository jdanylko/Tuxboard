import { ITuxbarControl } from "./ITuxbarControl";
import { Tuxbar } from "./Tuxbar";

export class TuxbarMessage implements ITuxbarControl {

    private tuxbarMessageSelector: string = "#tuxbar-status";

    constructor(
        private readonly tuxBar: Tuxbar,
        private selector: string = null) {

        this.tuxbarMessageSelector = selector || this.tuxbarMessageSelector;
    }

    public getDom() {
        return this.tuxBar.getDom().querySelector(this.tuxbarMessageSelector);
    }

    public setMessage(message: string, fade: boolean) {
        const control = this.getDom();

        if (control) {
            control.innerHTML = message;
        }
    }
}
