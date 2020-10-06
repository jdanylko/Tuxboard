import { TuxbarMessage } from "./TuxbarMessage";
import { AddWidgetButton } from "./AddWidgetButton";
import { ChangeLayoutButton } from "./ChangeLayoutButton";
import { Tuxboard } from "../../Tuxboard";
import { ITuxbarControl } from "./ITuxbarControl";
import { RefreshButton } from "./RefreshButton";

export class Tuxbar {

    private tuxbarSelector: string = ".tuxbar";

    private controls: ITuxbarControl[] = [];

    constructor(
        readonly parent: Tuxboard,
        selector: string = null
    ) {
        this.tuxbarSelector = selector || this.tuxbarSelector;

        this.initialize();
    }

    getDom() {
        return document.querySelector(this.tuxbarSelector) as HTMLElement;
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