import { BaseService } from "../core/BaseService";
import { DragWidgetInfo as DragInfo } from "../Models/DragWidgetInfo";
import { SettingValue } from "../Models/SettingValue";
import { WidgetSettingValue } from "../Models/WidgetSettingValue";

export class TuxboardService extends BaseService {

    tuxWidgetDialogUrl: string = "/widgetdialog/";
    tuxWidgetSettingsUrl: string = "/widgetsettings/";
    tuxWidgetAddWidgetUrl: string = "/widgetdialog/addwidget/";

    tuxWidgetTemplateUrl: string = "/WidgetTemplate/";

    tuxRefreshTuxboardUrl: string = "/Tuxboard/Get/";
    tuxToolCollapseUrl: string = "/Tuxboard/CollapseWidget/";
    tuxWidgetPlacementUrl: string = "/Tuxboard/Put/";
    tuxWidgetRemoveWidgetUrl: string = "/Tuxboard/removewidget/";
    tuxWidgetContentUrl: string = "/Widget/";
    tuxWidgetSaveSettingsUrl: string = "/WidgetSettings/Save/";

    constructor(debugParam: boolean = false) {
        super(debugParam);
    }

    /* Services */

    /* Service: Save Widget Placement */
    updateWidgetPlacementStatus(/* use data */) { }

    saveWidgetPlacementService(ev: Event, dragInfo: DragInfo) {

        const postData = {
            Column: dragInfo.currentColumnIndex,
            LayoutRowId: dragInfo.currentLayoutRowId,
            PreviousColumn: dragInfo.previousColumnIndex,
            PreviousLayout: dragInfo.previousLayoutRowId,
            PlacementId: dragInfo.placementId,
            PlacementList: dragInfo.placementList
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

    removeWidgetService(placementId: string) {

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

    updateCollapsedWidgetService(widgetId: string, collapsed: boolean) {

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

    getWidgetTemplate(placementId: string) {

        const request = new Request(this.tuxWidgetTemplateUrl + placementId,
            { method: "get" });

        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }

    /* Service: Get Widget */

    async getWidgetService(placementId: string) {

        const request = new Request(this.tuxWidgetContentUrl + placementId,
            { method: "get" });

        return await fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }

    /* Service: Refresh Tuxboard */

    async refreshService() {

        const request = new Request(this.tuxRefreshTuxboardUrl,
            { method: "get" });

        return await fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }

    getWidgetSettings(placementId: string) {

        const request = new Request(this.tuxWidgetSettingsUrl + placementId,
            { method: "get" });

        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }

    /* Service: Save Widget Settings */

    saveSettings(values: SettingValue[]) {
        let postData = {
            Settings: Array.from(values).map((item: SettingValue) => {
                return {
                    WidgetSettingId: item.WidgetSettingId,
                    Value: item.Value
                }
            })
        }

        const request = new Request(this.tuxWidgetSaveSettingsUrl,
            {
                method: "POST",
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

}