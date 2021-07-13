import { AddWidgetButton } from "./AddWidgetButton";
import { TuxbarMessage } from "./TuxbarMessage";
import { ChangeLayoutButton } from "./ChangeLayoutButton";
import { ITuxbarControl } from "./ITuxbarControl";
import { RefreshButton } from "./RefreshButton";
import { Tuxboard } from "../../Tuxboard";

export class Tuxbar {

    private tuxbarSelector: string = ".tuxbar";

    private controls: ITuxbarControl[] = [];

    constructor(
        readonly parent: Tuxboard,
        selector: string = null) {
        this.tuxbarSelector = selector || this.tuxbarSelector;

        this.initialize();
    }

    public getDom() {
        return document.querySelector(this.tuxbarSelector) as HTMLElement;
    }

    public getTuxboard() {
        return this.parent;
    }

    public initialize() {
        this.controls.push(new AddWidgetButton(this));
        this.controls.push(new ChangeLayoutButton(this));
        this.controls.push(new RefreshButton(this));
        this.controls.push(new TuxbarMessage(this));
    }
}
