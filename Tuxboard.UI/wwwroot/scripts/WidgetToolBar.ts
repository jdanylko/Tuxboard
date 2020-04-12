import { WidgetToolbarButton } from "./WidgetToolbarButton";

export class WidgetToolBar {
    public static tuxWidgetTools = "card-tools"; // buttons on each widget
    public static tuxWidgetToolDropdown = "dropdown-card-tool"; // Widget's tool dropdown button

    _buttons: WidgetToolbarButton[];

    constructor() { }

    getByWidget(widget) { return widget.getElementsByClassName(WidgetToolBar.tuxWidgetTools).item(0); }

    setupWidgetToolbar(elem) {

        const tools = this.getByWidget(elem);
        if (!tools) return;

        const removeButton = tools.getElementsByClassName(tuxWidgetToolRemove).item(0);

        if (removeButton) {
            removeButton.addEventListener("click", removeWidgetService, false);
        }

        const minimizeButton = tools.getElementsByClassName(tuxWidgetToolCollapse).item(0);
        if (minimizeButton) {
            minimizeButton.addEventListener("click", minimizeWidget, false);
        }

        /* dropdown options */
        const settingsOption = elem.getElementsByClassName(noPeriod(tuxSettingsOption)).item(0);
        if (settingsOption) {
            settingsOption.addEventListener("click", settingsClick, false);
        }

        const refreshOption = elem.getElementsByClassName(noPeriod(tuxRefreshOption)).item(0);
        if (refreshOption) {
            refreshOption.addEventListener("click", refreshClick, false);
        }
    }

    minimizeWidget(ev) {
        const widget = getParentByClass(ev, tuxWidgetClass);
        if (widget) {
            const minimized = widget.classList.contains(tuxWidgetCollapsed);
            if (minimized) {
                widget.classList.remove(tuxWidgetCollapsed);
                showWidgetBody(widget);
                updateCollapsedWidgetService(widget, 0);
            } else {
                widget.classList.add(tuxWidgetCollapsed);
                hideWidgetBody(widget);
                updateCollapsedWidgetService(widget, 1);
            }
        }
    }

    attachSettingEvents(widget: HTMLDivElement) {

        let saveButton = widget.querySelector(tuxWidgetSettingsSave);
        saveButton.addEventListener("click", saveSettingsClick, false);

        let cancelButton = widget.querySelector(tuxWidgetSettingsCancel);
        cancelButton.addEventListener("click", cancelSettingsClick, false);
    }

    cancelSettingsClick(ev: Event) {
        let widget = getParentByClass(ev, tuxWidgetClass);
        if (widget) {
            hideWidgetSettings(widget);
            showWidgetBody(widget);
        }
    }

    saveSettingsClick(ev: Event) {
        let widget = getParentByClass(ev, tuxWidgetClass) as HTMLDivElement;
        if (widget) {
            saveSettingsService(widget);
        }
    }

    settingsClick(ev) {
        let widget = getParentByClass(ev, tuxWidgetClass);
        if (widget) {
            getWidgetSettingsService(getPlacementId(widget));
        }
    }

    refreshClick(ev) {
        const widget = this.getParentByClass(ev, tuxWidgetClass);
        if (widget) {
            getWidgetService(widget);
        }
    }

    
}