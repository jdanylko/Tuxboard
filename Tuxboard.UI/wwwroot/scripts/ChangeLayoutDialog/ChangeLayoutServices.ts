export class ChangeLayoutServices {

    private tuxLayoutDialogUrl: string = "/layoutdialog/";
    private tuxLayoutAddRowUrl: string = "/layoutdialog/addlayoutrow/";
    private tuxSaveLayoutUrl: string = "/layoutdialog/saveLayout/";
    private tuxDeleteLayoutRowUrl: string = "/layoutdialog/DeleteLayoutRow/";

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

    /* Service: Load Layout Dialog */
    displayLayoutDialog(data) {
        if (this._debug) console.log(data);
        initLayoutDialog(data);
    }

    getLayoutDialogService() {
        fetch(this.tuxLayoutDialogUrl + getCurrentTab().value,
            {
                method: "post"
            })
            .then(this.readResponseAsText)
            .then(this.displayLayoutDialog)
            .catch(this.logError);
    }

    /* Service: Add Row */
    addRowToLayoutDialog(data) {
        if (this._debug) console.log(data);
        updateLayoutData(data);
    }

    addLayoutRowOnLayoutDialogService(typeId) {
        fetch(this.tuxLayoutAddRowUrl + typeId,
            {
                method: "post"
            })
            .then(this.readResponseAsText)
            .then(this.addRowToLayoutDialog)
            .catch(this.logError);
    }

    /* Service: Delete Row */
    deleteRowFromLayoutDialogService(row) {
        const id = row.attributes["data-id"].value;
        if (id === "0") { // new, we can remove it.
            row.remove();
        } else {
            fetch(this.tuxDeleteLayoutRowUrl + id,
                {
                    method: 'delete'
                })
                .then(this.readResponseAsJson)
                .then(function (data) {
                    if (this._debug) console.log(data);
                    if (data.success) {
                        resetColumnStatus();
                        row.remove();
                    } else {
                        setColumnStatus(id, data);
                    }
                })
                .catch(this.logError);
        }
    }

    /* Service: Save Layout */
    processSaveLayoutResponse(data) {
        if (this._debug) console.log(data);
        if (!data.success) {
            displayLayoutErrors(data);
        } else {
            layoutDialogInstance.hide();
            refreshTuxboardService();
        }
    }

    saveLayoutService(bodyData) {
        fetch(this.tuxSaveLayoutUrl,
            {
                method: 'post',
                body: JSON.stringify(bodyData),
                headers: {
                    'Content-Type': 'application/json'
                }
            })
            .then(this.readResponseAsJson)
            .then(this.processSaveLayoutResponse)
            .catch(this.logError);
    }

}