export class TuxboardServices {

    tuxWidgetDialogUrl: string = "/widgetdialog/";
    tuxWidgetSettingsUrl: string = "/widgetsettings/";
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

    /* Services */

    /* Service: Save Widget Placement */
    updateWidgetPlacementStatus(/* use data */) {}

    saveWidgetPlacementService(ev, previousColumn, previousLayout) {

        const widgetProperties = getWidgetProperties(ev);

        const postData = {
            Column: widgetProperties.Column,
            PreviousColumn: previousColumn,
            PreviousLayout: previousLayout,
            PlacementId: widgetProperties.PlacementId,
            LayoutRowId: widgetProperties.LayoutRowId,
            PlacementList: widgetProperties.PlacementList
        };

        fetch(this.tuxWidgetPlacementUrl,
                {
                    method: "put",
                    body: JSON.stringify(postData),
                    headers: {
                        "Content-Type": "application/json"
                    }
                })
            .then(this.validateResponse)
            .then(this.readResponseAsJson)
            .then(this.updateWidgetPlacementStatus)
            .catch(this.logError);
    }

/* Service: Remove Widget */
    removeWidgetFromDashboard(data) {
        if (data.success) {
            const widget = getDashboard().querySelector("[data-id='" + data.message.id + "']");
            if (widget) {
                widget.remove();
            }
        }
    }

    removeWidgetService(ev) {
        const button = getClosestByTag(ev, "button"),
            placementId = getDataId(button),
            tabId = getCurrentTab().value;

        const postData = {
            TabId: tabId,
            PlacementId: placementId
        };

        fetch(this.tuxWidgetRemoveWidgetUrl,
                {
                    method: 'delete',
                    body: JSON.stringify(postData),
                    headers: {
                        'Content-Type': 'application/json'
                    }
                })
            .then(this.validateResponse)
            .then(this.readResponseAsJson)
            .then(this.removeWidgetFromDashboard)
            .catch(this.logError);
    }

/* Service: Update Collapsed Widget */
    updateCollapsedStatus(/* use data */) {}

    updateCollapsedWidgetService(widget, collapsed) {

        const postData = {
            Id: getDataId(widget),
            Collapsed: collapsed
        };

        fetch(this.tuxToolCollapseUrl,
                {
                    method: "post",
                    body: JSON.stringify(postData),
                    headers: {
                        "Content-Type": "application/json"
                    }
                })
            .then(this.validateResponse)
            .then(this.readResponseAsJson)
            .then(this.updateCollapsedStatus)
            .catch(this.logError);

    }

/* Service: Get Widget */
    updateWidgetContents(id, data) {
        const widget = getDomWidget(id);
        if (widget) {
            const body = widget.querySelector(".card-body");
            if (body) {
                body.innerHTML = data;
            }
        }
    }

    getWidgetService(widget) {

        const id = getDataId(widget);
        if (!id) return;

        const postData = {
            WidgetPlacementId: id
        };

        showOverlay(widget);

        fetch(this.tuxWidgetContentUrl + getDataId(widget),
                {
                    method: "get"
                })
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .then(function(data) {
                if (this._debug) console.log(data);
                this.updateWidgetContents(postData.WidgetPlacementId, data);
                this.hideOverlay(widget);
            })
            .catch(this.logError);
    }

/* Service: Widgets Dialog Box */
    displayWidgetDialog(data) {
        const widgetDialog = document.getElementById(tuxWidgetDialog),
            overlay = widgetDialog.querySelector(tuxOverlay);

        setWidgetDialog(data);

        setWidgetEvents();

        overlay.setAttribute("style", "display:none");
    }

    getWidgetDialogService() {
        fetch(this.tuxWidgetDialogUrl,
                {
                    method: "get"
                })
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .then(this.displayWidgetDialog)
            .catch(this.logError);
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

/* Service: Refresh Tuxboard */
    updateTuxboard(data) {
        const dashboard = getDashboard();
        dashboard.innerHTML = data;

        initialize();
    }

    refreshTuxboardService() {

        fetch(this.tuxRefreshTuxboardUrl,
                {
                    method: 'get'
                })
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .then(this.updateTuxboard)
            .catch(this.logError);
    }

/* Service: Widget Settings */
    displayWidgetSettings(widget, data) {
        if (this._debug) console.log(data);

        const settings = getWidgetSettings(widget);
        if (settings) {
            settings.innerHTML = data;
            hideWidgetBody(widget);
            showWidgetSettings(widget);
            attachSettingEvents(widget);
        } else {
            showWidgetBody(widget);
            hideWidgetSettings(widget);
        }

        hideOverlay(widget);

        // initialize();
    }

    getWidgetSettingsService(placementId) {

        const widget = getDomWidget(placementId);
        hideWidgetBody(widget);
        showOverlay(widget);

        fetch(this.tuxWidgetSettingsUrl + placementId,
                {
                    method: "get"
                })
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .then(function(data) {
                if (this._debug) console.log(data);
                displayWidgetSettings(widget, data);
            })
            .catch(this.logError);
    }

/* Service: Save Widget Settings */
    saveSettingsResponse(data) {
        if (this._debug) console.log(data);

        const placementId = data[0].id;
        const widget = getDomWidget(placementId);

        const setting = Array.from(data).filter((elem) => elem.name === "widgettitle")[0];
        if (setting) {
            setWidgetTitle(widget, setting);
        }

        hideWidgetSettings(widget);
        if (!isCollapsed(widget)) {
            showWidgetBody(widget);
        }

        refreshWidget(widget);
    }

    setWidgetTitle(widget: HTMLDivElement, setting) {
        const widgetTitle = widget.querySelector(tuxWidgetTitle);
        if (widgetTitle) {
            widgetTitle.innerHTML = setting.value;
        }
    }

    saveSettingsService(widget: HTMLDivElement) {
        let postData = {
            Settings: getSettingValues(widget)
        }

        fetch(this.tuxWidgetSaveSettingsUrl,
                {
                    method: 'post',
                    body: JSON.stringify(postData),
                    headers: {
                        'Content-Type': 'application/json'
                    }
                })
            .then(this.readResponseAsJson)
            .then(this.saveSettingsResponse)
            .catch(this.logError);
    }

}