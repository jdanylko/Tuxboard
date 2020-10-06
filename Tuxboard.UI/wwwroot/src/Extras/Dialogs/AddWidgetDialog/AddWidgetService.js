"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.AddWidgetService = void 0;
const BaseService_1 = require("../../../core/BaseService");
class AddWidgetService extends BaseService_1.BaseService {
    constructor(debug = false) {
        super(debug);
        this.tuxWidgetDialogUrl = "/widgetdialog/";
        this.tuxWidgetAddWidgetUrl = "/widgetdialog/addwidget/";
    }
    ///* Service: Add Widget */
    //addWidgetToDashboard(data) {
    //    if (data.success) {
    //        if (widgetDialogInstance) {
    //            widgetDialogInstance.hide();
    //        }
    //        refreshTuxboardService();
    //    }
    //}
    addWidgetService(tabId, widgetId) {
        const request = new Request(this.tuxWidgetAddWidgetUrl, {
            method: "post",
            body: JSON.stringify({
                TabId: tabId,
                WidgetId: widgetId
            }),
            headers: {
                'Content-Type': 'application/json'
            }
        });
        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
        //return this.fetchJson(new Request(this.tuxWidgetAddWidgetUrl,
        //    {
        //        method: "post",
        //        body: JSON.stringify({
        //            TabId: tabId,
        //            WidgetId: widgetId
        //        })
        //    }
        //));
    }
    /* Service: Widgets Dialog Box */
    //displayWidgetDialog(data) {
    //    const widgetDialog = document.getElementById(tuxWidgetDialog),
    //        overlay = widgetDialog.querySelector(tuxOverlay);
    //    setWidgetDialog(data);
    //    setWidgetEvents();
    //    overlay.setAttribute("style", "display:none");
    //}
    getWidgetDialogService() {
        const request = new Request(this.tuxWidgetDialogUrl);
        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }
}
exports.AddWidgetService = AddWidgetService;
//# sourceMappingURL=AddWidgetService.js.map