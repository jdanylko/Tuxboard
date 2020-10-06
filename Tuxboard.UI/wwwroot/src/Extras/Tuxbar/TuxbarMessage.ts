import { Tuxbar } from "./Tuxbar";
import {ITuxbarControl} from "./ITuxbarControl";

export class TuxbarMessage implements ITuxbarControl {

    private tuxbarMessageSelector: string = "#tuxbar-status";

    constructor(private readonly tuxBar: Tuxbar,
        private selector: string = null) {

        this.tuxbarMessageSelector = selector || this.tuxbarMessageSelector;
    }

    getDom() {
        return this.tuxBar.getDom().querySelector(this.tuxbarMessageSelector);
    }

    setMessage(message: string, fade: boolean) {
        const control = this.getDom();
        
        if (control)
            control.innerHTML = message;

    }
}