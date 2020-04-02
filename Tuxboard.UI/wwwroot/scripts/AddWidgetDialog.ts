import BaseDialog = require("./BaseDialog");
import BaseDialog1 = BaseDialog.BaseDialog;

export class AddWidgetDialog extends BaseDialog1 {

    public tuxWidgetDialog:string = "widget-dialog"; // default is #widget-dialog
    public tuxWidgetTabGroup:string = ".widget-tabs"; // left-side group in #widget-dialog
    public tuxWidgetListItem:string = "a.widget-item"; // each widget on right in #widget-dialog
    public tuxWidgetAdd:string = ".add-widget"; // Add Widget button
    public tuxWidgetTools:string = "card-tools"; // buttons on each widget

    constructor() { super(); }
}