export class TuxboardServices {

    tuxWidgetDialogUrl: string = "/widgetdialog/";
    tuxWidgetAddWidgetUrl: string = "/widgetdialog/addwidget/";

    tuxRefreshTuxboardUrl: string = "/Tuxboard/Get/";
    tuxToolCollapseUrl: string = "/Tuxboard/PostCollapse/";
    tuxWidgetPlacementUrl: string = "/Tuxboard/Put/";
    tuxWidgetRemoveWidgetUrl: string = "/Tuxboard/removewidget/";
    tuxWidgetContentUrl: string = "/Widget/";
    tuxWidgetSaveSettingsUrl: string = "/WidgetSettings/Save/";

    private _debug: boolean;

    constructor(debug: boolean = false) {
        this._debug = debug;
    }


    validateResponse(response) {
        if (this._debug) console.log(response);
        if (!response.ok) {
            throw Error(response.statusText);
        }
        return response;
    }

    readResponseAsJson(response) {
        if (this._debug) console.log(response);
        return response.json();
    }

    readResponseAsText(response) {
        if (this._debug) console.log(response);
        return response.text();
    }

    logError(error) {
        if (this._debug) console.log('Issue w/ fetch call: \n', error);
    }

/* Service: Add Widget */
    addWidgetToDashboard(data) {
        if (data.success) {
            if (widgetDialogInstance) {
                widgetDialogInstance.hide();
            }
            refreshTuxboardService();
        }
    }

    addWidgetService(tabId, widgetId) {

        const postData = {
            TabId: tabId,
            WidgetId: widgetId
        };
        fetch(this.tuxWidgetAddWidgetUrl,
                {
                    method: "post",
                    body: JSON.stringify(postData),
                    headers: {
                        "Content-Type": "application/json"
                    }
                })
            .then(this.validateResponse)
            .then(this.readResponseAsJson)
            .then(this.addWidgetToDashboard)
            .catch(this.logError);
    }

}