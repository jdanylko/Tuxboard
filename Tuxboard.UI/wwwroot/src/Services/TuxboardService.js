"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.TuxboardService = void 0;
const BaseService_1 = require("../core/BaseService");
class TuxboardService extends BaseService_1.BaseService {
    constructor(debugParam = false) {
        super(debugParam);
        this.tuxWidgetDialogUrl = "/widgetdialog/";
        this.tuxWidgetSettingsUrl = "/widgetsettings/";
        this.tuxWidgetAddWidgetUrl = "/widgetdialog/addwidget/";
        this.tuxWidgetTemplateUrl = "/WidgetTemplate/";
        this.tuxRefreshTuxboardUrl = "/Tuxboard/Get/";
        this.tuxToolCollapseUrl = "/Tuxboard/CollapseWidget/";
        this.tuxWidgetPlacementUrl = "/Tuxboard/Put/";
        this.tuxWidgetRemoveWidgetUrl = "/Tuxboard/removewidget/";
        this.tuxWidgetContentUrl = "/Widget/";
        this.tuxWidgetSaveSettingsUrl = "/WidgetSettings/Save/";
    }
    /* Services */
    /* Service: Save Widget Placement */
    updateWidgetPlacementStatus( /* use data */) { }
    saveWidgetPlacementService(ev, dragInfo) {
        const postData = {
            Column: dragInfo.currentColumnIndex,
            LayoutRowId: dragInfo.currentLayoutRowId,
            PreviousColumn: dragInfo.previousColumnIndex,
            PreviousLayout: dragInfo.previousLayoutRowId,
            PlacementId: dragInfo.placementId,
            PlacementList: dragInfo.placementList
        };
        const request = new Request(this.tuxWidgetPlacementUrl, {
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
    removeWidgetService(placementId) {
        const postData = {
            TabId: "",
            PlacementId: placementId
        };
        const request = new Request(this.tuxWidgetRemoveWidgetUrl, {
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
    updateCollapsedWidgetService(widgetId, collapsed) {
        const postData = {
            Id: widgetId,
            Collapsed: collapsed
        };
        const request = new Request(this.tuxToolCollapseUrl, {
            method: 'post',
            body: JSON.stringify(postData)
        });
        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsJson)
            .catch(this.logError);
    }
    /* Service: Get Widget Template */
    getWidgetTemplate(placementId) {
        const request = new Request(this.tuxWidgetTemplateUrl + placementId, { method: "get" });
        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }
    /* Service: Get Widget */
    getWidgetService(placementId) {
        return __awaiter(this, void 0, void 0, function* () {
            const request = new Request(this.tuxWidgetContentUrl + placementId, { method: "get" });
            return yield fetch(request)
                .then(this.validateResponse)
                .then(this.readResponseAsText)
                .catch(this.logError);
        });
    }
    /* Service: Refresh Tuxboard */
    refreshService() {
        return __awaiter(this, void 0, void 0, function* () {
            const request = new Request(this.tuxRefreshTuxboardUrl, { method: "get" });
            return yield fetch(request)
                .then(this.validateResponse)
                .then(this.readResponseAsText)
                .catch(this.logError);
        });
    }
    getWidgetSettings(placementId) {
        const request = new Request(this.tuxWidgetSettingsUrl + placementId, { method: "get" });
        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }
    /* Service: Save Widget Settings */
    saveSettings(values) {
        let postData = {
            Settings: Array.from(values).map((item) => {
                return {
                    WidgetSettingId: item.WidgetSettingId,
                    Value: item.Value
                };
            })
        };
        const request = new Request(this.tuxWidgetSaveSettingsUrl, {
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
exports.TuxboardService = TuxboardService;
//# sourceMappingURL=TuxboardService.js.map