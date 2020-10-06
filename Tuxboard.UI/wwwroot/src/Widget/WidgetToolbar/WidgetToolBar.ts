import { WidgetToolbarButton } from "./WidgetToolbarButton";
import { WidgetCollapseButton } from "./WidgetCollapseButton";
import { WidgetRemoveButton } from "./WidgetRemoveButton";
import { WidgetPlacement } from "../WidgetPlacement";
import { WidgetSettings } from "../WidgetSettings";

export class WidgetToolBar {

    private widgetToolbarSelector: string = ".card-tools";
    private widgetToolDropdownSelector: string = ".dropdown-card-tool";

    private dropdownRefreshOption: string = ".refresh-option";
    private dropdownSettingsOption: string = ".settings-option";

    private buttons = new Array<WidgetToolbarButton>();

    constructor(
        private readonly widgetPlacement: WidgetPlacement,
        selector: string = null
    ) {
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
            .querySelector(this.widgetToolbarSelector) as HTMLElement;
    }

    getWidgetPlacement() { return this.widgetPlacement; }

    addButton(button: WidgetToolbarButton) {
        this.buttons.push(button);
    }

    removeButton(button: WidgetToolbarButton) {
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
    }

    attachSettingEvents(widget: HTMLDivElement) {

        //let saveButton = widget.querySelector(tuxWidgetSettingsSave);
        //saveButton.addEventListener("click", saveSettingsClick, false);

        //let cancelButton = widget.querySelector(tuxWidgetSettingsCancel);
        //cancelButton.addEventListener("click", cancelSettingsClick, false);
    }

    cancelSettingsClick(ev: Event) {
        //let widget = getParentByClass(ev, tuxWidgetClass);
        //if (widget) {
        //    hideWidgetSettings(widget);
        //    showWidgetBody(widget);
        //}
    }

    saveSettingsClick(ev: Event) {
        //let widget = getParentByClass(ev, tuxWidgetClass) as HTMLDivElement;
        //if (widget) {
        //    saveSettingsService(widget);
        //}
    }

    settingsClick(ev: Event) {
        //let widget = getParentByClass(ev, tuxWidgetClass);
        //if (widget) {
        //    getWidgetSettingsService(getPlacementId(widget));
        //}
    }

    
}