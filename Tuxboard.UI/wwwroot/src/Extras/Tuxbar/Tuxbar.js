"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Tuxbar = void 0;
const TuxbarMessage_1 = require("./TuxbarMessage");
const AddWidgetButton_1 = require("./AddWidgetButton");
const ChangeLayoutButton_1 = require("./ChangeLayoutButton");
const RefreshButton_1 = require("./RefreshButton");
class Tuxbar {
    constructor(parent, selector = null) {
        this.parent = parent;
        this.tuxbarSelector = ".tuxbar";
        this.controls = [];
        this.tuxbarSelector = selector || this.tuxbarSelector;
        this.initialize();
    }
    getDom() {
        return document.querySelector(this.tuxbarSelector);
    }
    getTuxboard() {
        return this.parent;
    }
    initialize() {
        this.controls.push(new AddWidgetButton_1.AddWidgetButton(this));
        this.controls.push(new ChangeLayoutButton_1.ChangeLayoutButton(this));
        this.controls.push(new RefreshButton_1.RefreshButton(this));
        this.controls.push(new TuxbarMessage_1.TuxbarMessage(this));
    }
}
exports.Tuxbar = Tuxbar;
//# sourceMappingURL=Tuxbar.js.map