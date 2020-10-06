"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.WidgetToolBar = void 0;
const WidgetCollapseButton_1 = require("./WidgetCollapseButton");
const WidgetRemoveButton_1 = require("./WidgetRemoveButton");
class WidgetToolBar {
    constructor(widgetPlacement, selector = null) {
        this.widgetPlacement = widgetPlacement;
        this.widgetToolbarSelector = ".card-tools";
        this.widgetToolDropdownSelector = ".dropdown-card-tool";
        this.dropdownRefreshOption = ".refresh-option";
        this.dropdownSettingsOption = ".settings-option";
        this.buttons = new Array();
        this.widgetToolbarSelector = selector || this.widgetToolbarSelector;
        // Default buttons
        this.addButton(new WidgetCollapseButton_1.WidgetCollapseButton(this));
        this.addButton(new WidgetRemoveButton_1.WidgetRemoveButton(this));
        // Dropdown options
        this.setupWidgetDropdown();
    }
    getDom() {
        return this.widgetPlacement
            .getDom()
            .querySelector(this.widgetToolbarSelector);
    }
    getWidgetPlacement() { return this.widgetPlacement; }
    addButton(button) {
        this.buttons.push(button);
    }
    removeButton(button) {
        const name = button.getName();
        const index = this.buttons.indexOf(name, 0);
        if (index > -1) {
            this.buttons.splice(index, 1);
        }
    }
    setupWidgetDropdown() {
        /* dropdown options */
        const settingsOption = this.widgetPlacement.getDom().querySelector(this.dropdownSettingsOption);
        if (settingsOption) {
            settingsOption.addEventListener("click", (ev) => {
                this.widgetPlacement.showWidgetSettings();
            }, false);
        }
        const refreshOption = this.widgetPlacement.getDom().querySelector(this.dropdownRefreshOption);
        if (refreshOption) {
            refreshOption.addEventListener("click", (ev) => {
                ev.preventDefault();
                this.widgetPlacement.update();
            }, false);
        }
    }
    attachSettingEvents(widget) {
        //let saveButton = widget.querySelector(tuxWidgetSettingsSave);
        //saveButton.addEventListener("click", saveSettingsClick, false);
        //let cancelButton = widget.querySelector(tuxWidgetSettingsCancel);
        //cancelButton.addEventListener("click", cancelSettingsClick, false);
    }
    cancelSettingsClick(ev) {
        //let widget = getParentByClass(ev, tuxWidgetClass);
        //if (widget) {
        //    hideWidgetSettings(widget);
        //    showWidgetBody(widget);
        //}
    }
    saveSettingsClick(ev) {
        //let widget = getParentByClass(ev, tuxWidgetClass) as HTMLDivElement;
        //if (widget) {
        //    saveSettingsService(widget);
        //}
    }
    settingsClick(ev) {
        //let widget = getParentByClass(ev, tuxWidgetClass);
        //if (widget) {
        //    getWidgetSettingsService(getPlacementId(widget));
        //}
    }
}
exports.WidgetToolBar = WidgetToolBar;
//# sourceMappingURL=WidgetToolBar.js.map