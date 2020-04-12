export class BaseDialog {
    public modalBody: string = ".modal-body";

    public tuxOverlay:string  = ".overlay";
    public tuxWidgetOverlay:string = this.tuxOverlay+".loading-status"; // overlay on each dialog box/widget for loading


    showOverlay(widget) {
        const overlay = widget.querySelector(this.tuxWidgetOverlay);
        if (overlay && overlay.hasAttribute("hidden")) {
            overlay.removeAttribute("hidden");
        }
    }

    hideOverlay(widget) {
        const overlay = widget.querySelector(this.tuxWidgetOverlay);
        if (overlay && !overlay.hasAttribute("hidden")) {
            overlay.setAttribute("hidden", "");
        }
    }

}