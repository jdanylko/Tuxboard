import { WidgetSettings } from "./WidgetSettings";
import { WidgetToolBar } from "./WidgetToolBar";

export class Widget {

    public static widgetId: string = ".card";
    public static widgetTitleId: string = ".card-title";

    private widgetCardBody: string = ".card-body";

    private tuxOverlay = ".overlay";
    private tuxWidgetOverlay: string = this.tuxOverlay + ".loading-status"; // overlay on each dialog box/widget for loading

    private _toolbar: WidgetToolBar = new WidgetToolBar();
    private _settings: WidgetSettings = new WidgetSettings();

    constructor() { }

    showOverlay(widget) {
        const overlay = widget.querySelector(this.tuxWidgetOverlay);
        if (overlay && overlay.hasAttribute("hidden")) {
            overlay.removeAttribute("hidden");
        }
    }

    hideOverlay(widget) {
        const overlay = widget.querySelector(this.tuxWidgetOverlay);
        if (overlay && !overlay.hasAttribute("hidden")) {
            overlay.setAttribute("hidden", "");
        }
    }

    showWidgetSettings(widget) {
        const settings = widget.querySelector(WidgetSettings.tuxWidgetSettings);
        if (settings && settings.hasAttribute("hidden")) {
            settings.removeAttribute("hidden");
        }
    }

    hideWidgetSettings(widget) {
        const settings = widget.querySelector(WidgetSettings.tuxWidgetSettings);
        if (settings && !settings.hasAttribute("hidden")) {
            settings.setAttribute("hidden", "");
        }
    }

    getWidgetBody(widget) {
        return widget.querySelector(this.widgetCardBody);
    }

    hideWidgetBody(widget) {
        const body = this.getWidgetBody(widget);
        if (body) {
            body.setAttribute("hidden", "");
        }
    }

    showWidgetBody(widget) {
        const body = this.getWidgetBody(widget);
        if (body) {
            body.removeAttribute("hidden");
        }
    }

    getWidgetTools(widget) {
        return widget.getElementsByClassName(this._toolbar.getByWidget(widget)).item(0);
    }

    refreshWidget(widget) {
        getWidgetService(widget);
    }

}