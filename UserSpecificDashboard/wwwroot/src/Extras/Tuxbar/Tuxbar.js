import { AddWidgetButton } from "./AddWidgetButton";
import { TuxbarMessage } from "./TuxbarMessage";
import { ChangeLayoutButton } from "./ChangeLayoutButton";
import { RefreshButton } from "./RefreshButton";
export class Tuxbar {
    constructor(parent, selector = null) {
        this.parent = parent;
        this.tuxbarSelector = ".tuxbar";
        this.controls = [];
        this.tuxbarSelector = selector || this.tuxbarSelector;
        this.initialize();
    }
    getDom() {
        return document.querySelector(this.tuxbarSelector);
    }
    getTuxboard() {
        return this.parent;
    }
    initialize() {
        this.controls.push(new AddWidgetButton(this));
        this.controls.push(new ChangeLayoutButton(this));
        this.controls.push(new RefreshButton(this));
        this.controls.push(new TuxbarMessage(this));
    }
}
