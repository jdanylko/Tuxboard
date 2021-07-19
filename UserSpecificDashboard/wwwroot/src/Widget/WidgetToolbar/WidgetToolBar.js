import { WidgetCollapseButton } from "./WidgetCollapseButton";
import { WidgetRemoveButton } from "./WidgetRemoveButton";
export class WidgetToolBar {
    constructor(widgetPlacement, selector = null) {
        this.widgetPlacement = widgetPlacement;
        this.widgetToolbarSelector = ".card-tools";
        this.widgetToolDropdownSelector = ".dropdown-card-tool";
        this.dropdownRefreshOption = ".refresh-option";
        this.dropdownSettingsOption = ".settings-option";
        this.buttonList = new Array();
        this.widgetToolbarSelector = selector || this.widgetToolbarSelector;
        // Default buttons
        this.addButton(new WidgetCollapseButton(this));
        this.addButton(new WidgetRemoveButton(this));
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
        this.buttonList.push(button);
    }
    removeButton(button) {
        const name = button.getName();
        const index = this.buttonList.findIndex(item => item.getName() === name);
        if (index > -1) {
            this.buttonList.splice(index, 1);
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
        return;
    }
}
