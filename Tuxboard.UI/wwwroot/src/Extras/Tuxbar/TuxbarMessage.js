"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.TuxbarMessage = void 0;
class TuxbarMessage {
    constructor(tuxBar, selector = null) {
        this.tuxBar = tuxBar;
        this.selector = selector;
        this.tuxbarMessageSelector = "#tuxbar-status";
        this.tuxbarMessageSelector = selector || this.tuxbarMessageSelector;
    }
    getDom() {
        return this.tuxBar.getDom().querySelector(this.tuxbarMessageSelector);
    }
    setMessage(message, fade) {
        const control = this.getDom();
        if (control)
            control.innerHTML = message;
    }
}
exports.TuxbarMessage = TuxbarMessage;
//# sourceMappingURL=TuxbarMessage.js.map