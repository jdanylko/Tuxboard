import BaseDialog = require("./BaseDialog");
import BaseDialog1 = BaseDialog.BaseDialog;

export class ChangeLayoutDialog extends BaseDialog1 {

    public tuxLayoutDialog: string = "layout-dialog"; // default is #layout-dialog
    public tuxSaveLayoutButton:string = ".save-layout"; // save layout button.
    public tuxLayoutDeleteButton:string = ".layout-delete-button";
    public tuxLayoutList:string = ".layout-list";
    public tuxLayoutItem:string = "layout-item";
    public tuxLayoutTypes:string = ".layout-types a";
    public tuxLayoutListHandle:string = ".handle";
    public tuxLayoutMessage:string = "#layout-message";

    constructor() { super(); }
}