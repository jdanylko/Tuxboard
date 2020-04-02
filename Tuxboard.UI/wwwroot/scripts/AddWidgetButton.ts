import TuxbarButton = require("./TuxbarButton");
import TuxbarButton1 = TuxbarButton.TuxbarButton;

export class AddWidgetButton extends TuxbarButton1 {
    public tuxWidgetButton:string = "widget-button";

    constructor() { super(); }
}