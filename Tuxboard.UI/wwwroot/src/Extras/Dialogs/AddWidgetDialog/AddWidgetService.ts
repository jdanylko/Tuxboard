import { BaseService } from "../../../core/BaseService";

export class AddWidgetService extends BaseService {

    tuxWidgetDialogUrl: string = "/widgetdialog/";
    tuxWidgetAddWidgetUrl: string = "/widgetdialog/addwidget/";

    constructor(debug: boolean = false) {
        super(debug);
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

    addWidgetService(tabId:string, widgetId:string) {
        const request = new Request(this.tuxWidgetAddWidgetUrl,
            {
                method: "post",
                body: JSON.stringify({
                    TabId: tabId,
                    WidgetId: widgetId
                }),
                headers: {
                    'Content-Type': 'application/json'
                }
            }
        );
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