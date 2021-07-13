import { clearNodes, getDataId } from "../core/common";
import { SettingValue } from "../Models/SettingValue";
import { TuxboardService } from "../Services/TuxboardService";
import { WidgetPlacement } from "./WidgetPlacement";
import { WidgetSettingValue } from "../Models/WidgetSettingValue";

export class WidgetSettings {

    private widgetSettingsSelector: string = ".widget-settings";

    private widgetSettingsCancelButtonSelector: string = ".settings-cancel";
    private widgetSettingsSaveButtonSelector: string = ".settings-save";
    private widgetSettingInputsSelector: string = ".setting-value";

    private service: TuxboardService = new TuxboardService();

    constructor(
        private readonly widget: WidgetPlacement,
        private readonly selector: string = null) {

        this.widgetSettingsSelector = selector || this.widgetSettingsSelector;
    }

    public getDom() {
        return this.widget.getDom().querySelector(this.widgetSettingsSelector);
    }

    public showWidgetSettings() {
        const settings = this.getDom();
        if (settings && settings.getAttribute("hidden") !== null) {
            settings.removeAttribute("hidden");
        }
        this.widget.hideBody();
    }

    public hideWidgetSettings() {
        const settings = this.getDom();
        if (settings && settings.getAttribute("hidden") === null) {
            settings.removeAttribute("hidden");
        }
        this.widget.showBody();
    }

    public attachSettingsEvent() {
        const saveButton = this.getDom().querySelector(this.widgetSettingsSaveButtonSelector);
        saveButton.addEventListener("click", (ev: Event) => {
            this.saveSettingsClick(ev);
        }, false);

        const cancelButton = this.getDom().querySelector(this.widgetSettingsCancelButtonSelector);
        cancelButton.addEventListener("click", () => {
            this.hideWidgetSettings();
            this.widget.showBody();
        }, false);
    }

    public getSettingValues() {
        const inputs = this.getDom().querySelectorAll<HTMLInputElement>(this.widgetSettingInputsSelector);
        return Array.from(inputs).map((elem:HTMLInputElement, index:number) => {
            return new SettingValue(getDataId(elem), elem.value);
        });
    }

    public saveSettingsClick(ev: Event) {
        const values = this.getSettingValues();
        this.service.saveSettings(values)
            .then((response:WidgetSettingValue[]) => {

                // Find the title if there is one
                const setting = Array.from(response).filter((elem) => elem.name.toLowerCase() === "widgettitle")[0];
                if (setting) {
                    this.widget.setTitle(setting.value);
                }

                this.hideWidgetSettings();

                this.widget.update();
            });
    }

    public displaySettings() {
        this.widget.showOverlay();

        this.service.getWidgetSettings(this.widget.getPlacementId())
            .then((data) => {
                const settings = this.getDom();
                if (settings) {

                    clearNodes(settings);
                    settings.insertAdjacentHTML("beforeend", String(data));

                    this.widget.hideBody();
                    this.widget.hideOverlay();
                    this.showWidgetSettings();
                    this.attachSettingsEvent();
                } else {
                    this.widget.showBody();
                    this.hideWidgetSettings();
                }
            });
    }
}
