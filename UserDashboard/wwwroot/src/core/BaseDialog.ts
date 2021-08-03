
export class BaseDialog {

    protected dialogBodySelector: string = ".modal-body";
    protected generalOverlaySelector: string = ".overlay";
    protected loadingSelector: string = ".loading-status";
    protected dialogOverlaySelector: string = this.generalOverlaySelector + this.loadingSelector;

    constructor(protected selector: string) { }

    public getDom() {
        return document.querySelector(this.selector);
    }

    public getOverlay() {
        return this.getDom().querySelector(this.generalOverlaySelector);
    }

    public showOverlay() {
        const overlay = this.getOverlay();
        if (overlay && overlay.hasAttribute("hidden")) {
            overlay.removeAttribute("hidden");
        }
    }

    public hideOverlay() {
        const overlay = this.getOverlay();
        if (overlay && !overlay.hasAttribute("hidden")) {
            overlay.setAttribute("hidden", "");
        }
    }
}
