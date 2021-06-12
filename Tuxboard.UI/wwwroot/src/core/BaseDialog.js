export class BaseDialog {
    constructor(selector) {
        this.selector = selector;
        this.dialogBodySelector = ".modal-body";
        this.generalOverlaySelector = ".overlay";
        this.loadingSelector = ".loading-status";
        this.dialogOverlaySelector = this.generalOverlaySelector + this.loadingSelector;
    }
    getDom() {
        return document.querySelector(this.selector);
    }
    getOverlay() {
        return this.getDom().querySelector(this.generalOverlaySelector);
    }
    showOverlay() {
        const overlay = this.getOverlay();
        if (overlay && overlay.hasAttribute("hidden")) {
            overlay.removeAttribute("hidden");
        }
    }
    hideOverlay() {
        const overlay = this.getOverlay();
        if (overlay && !overlay.hasAttribute("hidden")) {
            overlay.setAttribute("hidden", "");
        }
    }
}
