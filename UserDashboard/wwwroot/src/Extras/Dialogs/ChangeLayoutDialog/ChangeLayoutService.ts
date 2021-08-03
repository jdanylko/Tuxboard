import { BaseService } from "../../../core/BaseService";
import { dataId } from "../../../core/common";
import { LayoutModel } from "./LayoutModel";

export class ChangeLayoutService extends BaseService {

    private tuxLayoutDialogUrl: string = "/layoutdialog/";
    private tuxLayoutAddRowUrl: string = "/layoutdialog/{0}?handler=AddLayoutRow";
    private tuxSaveLayoutUrl: string = "/layoutdialog/{0}?handler=SaveLayout";
    private tuxDeleteLayoutRowUrl: string = "/layoutdialog/{0}?handler=DeleteLayoutRow";

    constructor(debug: boolean = false) {
        super(debug);
    }

    /* Service: Load Layout Dialog */
    public getLayoutDialog(tabId: string) {
        const request = new Request(this.tuxLayoutDialogUrl + tabId,
            { method: "get" });

        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }

    /* Service: Add Layout Row */
    public addLayoutRow(tabId:string, typeId: string, token:string) {
        const url: string = this.tuxLayoutAddRowUrl.replace("{0}", tabId);
        const postData = {
            id: tabId,
            layoutTypeId: typeId
        };
        const request = new Request(url,
            {
                body: JSON.stringify(postData),
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token,
                },
                method: "post",
            });

        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }

    /* Service: Delete Row */
    public deleteRowFromLayoutDialogService(tabId: string, row: HTMLElement, token:string) {
        const id = row.getAttribute(dataId);
        if (id === "0") { // new, we can remove it.
            row.remove();
        } else {
            const postData = {
                id: tabId,
                layoutRowId: id
            };
            const url = this.tuxDeleteLayoutRowUrl.replace("{0}", tabId);
            const request = new Request(url,
                {
                    body: JSON.stringify(postData),
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token,
                    },
                    method: "post",
                });

            return fetch(request)
                .then(this.validateResponse)
                .then(this.readResponseAsJson)
                .catch(this.logError);
        }
    }

    /* Service: Save Layout */
    public saveLayoutService(tabId: string, bodyData: LayoutModel, token:string) {
        const url = this.tuxSaveLayoutUrl.replace("{0}", tabId);
        const request = new Request(url,
            {
                method: 'post',
                body: JSON.stringify(bodyData),
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token,
                }
            });

        return fetch(request)
            .then(this.validateResponse)
            // .then(this.readResponseAsJson)
            .catch(this.logError);
    }
}
