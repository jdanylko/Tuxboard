"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.RefreshButton = void 0;
const TuxbarButton_1 = require("./TuxbarButton");
class RefreshButton extends TuxbarButton_1.TuxbarButton {
    constructor(tuxBar, changeLayoutSelector = null) {
        super(tuxBar, changeLayoutSelector);
        this.tuxbarRefreshButtonSelector = "#refresh-button";
        this.selector = changeLayoutSelector || this.tuxbarRefreshButtonSelector;
        const element = this.getDom();
        if (element) {
            element.addEventListener("click", (ev) => {
                this.tuxBar.getTuxboard().refresh();
            }, false);
        }
    }
    getDom() {
        return this.tuxBar.getDom().querySelector(this.selector);
    }
}
exports.RefreshButton = RefreshButton;
//# sourceMappingURL=RefreshButton.js.map