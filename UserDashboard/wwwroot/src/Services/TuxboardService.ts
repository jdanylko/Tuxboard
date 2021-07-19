import { BaseService } from "../core/BaseService";
import { DragWidgetInfo as DragInfo } from "../Models/DragWidgetInfo";
import { SettingValue } from "../Models/SettingValue";
import { WidgetSettingValue } from "../Models/WidgetSettingValue";

export class TuxboardService extends BaseService {

    private tuxWidgetDialogUrl: string = "/widgetdialog/";
    private tuxWidgetSettingsUrl: string = "/widgetsettings/";
    private tuxWidgetAddWidgetUrl: string = "/widgetdialog/addwidget/";

    private tuxRefreshTuxboardUrl: string = "/Tuxboard/Get/";
    private tuxToolCollapseUrl: string = "?/Tuxboard/CollapseWidget/";
    private tuxWidgetPlacementUrl: string = "/Tuxboard/Put/";
    private tuxWidgetRemoveWidgetUrl: string = "/Tuxboard/removewidget/";

    private tuxWidgetContentUrl: string = "/Widget/";
    private tuxWidgetTemplateUrl: string = "/Widget/{0}?handler=Template";

    private tuxWidgetSaveSettingsUrl: string = "/WidgetSettings/Save/";

    constructor(debugParam: boolean = false) {
        super(debugParam);
    }

    /* Services */

    /* Service: Save Widget Placement */
    public updateWidgetPlacementStatus(/* use data */) { }

    public saveWidgetPlacementService(ev: Event, dragInfo: DragInfo) {

        const postData = {
            Column: dragInfo.currentColumnIndex,
            LayoutRowId: dragInfo.currentLayoutRowId,
            PreviousColumn: dragInfo.previousColumnIndex,
            PreviousLayout: dragInfo.previousLayoutRowId,
            PlacementId: dragInfo.placementId,
            PlacementList: dragInfo.placementList,
        };

        const request = new Request(this.tuxWidgetPlacementUrl,
            {
                method: "put",
                body: JSON.stringify(postData),
                headers: {
                    'Content-Type': 'application/json'
                }
            });

        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }

    /* Service: Remove Widget */

    public removeWidgetService(placementId: string) {

        const postData = {
            TabId: "",
            PlacementId: placementId
        };

        const request = new Request(this.tuxWidgetRemoveWidgetUrl,
            {
                method: 'delete',
                body: JSON.stringify(postData),
                headers: {
                    'Content-Type': 'application/json'
                }
            });

        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsJson)
            .catch(this.logError);
    }

    /* Service: Update Collapsed Widget */

    public updateCollapsedWidgetService(widgetId: string, collapsed: boolean) {

        const postData = {
            Id: widgetId,
            Collapsed: collapsed
        };

        const request = new Request(this.tuxToolCollapseUrl,
            {
                method: 'post',
                body: JSON.stringify(postData)
            });

        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsJson)
            .catch(this.logError);
    }

    /* Service: Get Widget Template */

    public getWidgetTemplate(placementId: string) {

        const request = new Request(this.tuxWidgetTemplateUrl.replace("{0}", placementId),
            { method: "get" });

        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }

    /* Service: Get Widget */

    public getWidgetService(placementId: string) {

        const request = new Request(this.tuxWidgetContentUrl + placementId,
            { method: "get" });

        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }

    /* Service: Refresh Tuxboard */

    public refreshService() {

        const request = new Request(this.tuxRefreshTuxboardUrl,
            { method: "get" });

        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }

    public getWidgetSettings(placementId: string) {

        const request = new Request(this.tuxWidgetSettingsUrl + placementId,
            { method: "get" });

        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }

    /* Service: Save Widget Settings */

    public saveSettings(values: SettingValue[]) {
        const postData = {
            Settings: Array.from(values).map((item: SettingValue) => {
                return {
                    WidgetSettingId: item.WidgetSettingId,
                    Value: item.Value
                }
            })
        };

        const request = new Request(this.tuxWidgetSaveSettingsUrl,
            {
                method: "POST",
                body: JSON.stringify(postData),
                headers: {
                    "Content-Type": "application/json"
                }
            });

        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsJson)
            .catch(this.logError);
    }
}
