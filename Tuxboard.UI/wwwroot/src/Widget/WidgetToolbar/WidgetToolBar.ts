import { WidgetCollapseButton } from "./WidgetCollapseButton";
import { WidgetPlacement } from "../WidgetPlacement";
import { WidgetRemoveButton } from "./WidgetRemoveButton";
import { WidgetToolbarButton } from "./WidgetToolbarButton";

export class WidgetToolBar {

    private widgetToolbarSelector: string = ".card-tools";
    private widgetToolDropdownSelector: string = ".dropdown-card-tool";

    private dropdownRefreshOption: string = ".refresh-option";
    private dropdownSettingsOption: string = ".settings-option";

    private buttonList = new Array<WidgetToolbarButton>();

    constructor(
        private readonly widgetPlacement: WidgetPlacement,
        selector: string = null,
    ) {
        this.widgetToolbarSelector = selector || this.widgetToolbarSelector;

        // Default buttons
        this.addButton(new WidgetCollapseButton(this));
        this.addButton(new WidgetRemoveButton(this));

        // Dropdown options
        this.setupWidgetDropdown();
    }

    public getDom() {
        return this.widgetPlacement
            .getDom()
            .querySelector(this.widgetToolbarSelector) as HTMLElement;
    }

    public getWidgetPlacement() { return this.widgetPlacement; }

    public addButton(button: WidgetToolbarButton) {
        this.buttonList.push(button);
    }

    public removeButton(button: WidgetToolbarButton) {
        const name = button.getName();
        const index = this.buttonList.findIndex(item=> item.getName() === name);
        if (index > -1) {
            this.buttonList.splice(index, 1);
        }
    }

    private setupWidgetDropdown() {

        /* dropdown options */
        const settingsOption = this.widgetPlacement.getDom().querySelector(this.dropdownSettingsOption);
        if (settingsOption) {
            settingsOption.addEventListener("click", (ev: Event) => {
                this.widgetPlacement.showWidgetSettings();
            }, false);
        }

        const refreshOption = this.widgetPlacement.getDom().querySelector(this.dropdownRefreshOption);
        if (refreshOption) {
            refreshOption.addEventListener("click", (ev: Event) => {
                ev.preventDefault();
                this.widgetPlacement.update();
            }, false);
        }

        return;
    }
}
