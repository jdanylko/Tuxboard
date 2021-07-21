import { BaseService } from "../../../core/BaseService";
import { dataId } from "../../../core/common";
import { LayoutModel } from "./LayoutModel";

export class ChangeLayoutService extends BaseService {

    private tuxLayoutDialogUrl: string = "/layoutdialog/";
    private tuxLayoutAddRowUrl: string = "/layoutdialog/{0}?handler=AddLayoutRow/";
    private tuxSaveLayoutUrl: string = "/layoutdialog/?handler=SaveLayout/";
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
    public addLayoutRow(typeId: string) {
        var url = this.tuxLayoutAddRowUrl.replace("{0}", typeId);
        const request = new Request(url, { method: "post" });

        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }

    /* Service: Delete Row */
    public deleteRowFromLayoutDialogService(row: HTMLElement) {
        const id = row.getAttribute(dataId);
        if (id === "0") { // new, we can remove it.
            row.remove();
        } else {
            var url = this.tuxDeleteLayoutRowUrl.replace("{0}", id);
            const request = new Request(url, { method: 'post' });

            return fetch(request)
                .then(this.validateResponse)
                .then(this.readResponseAsJson)
                .catch(this.logError);
        }
    }

    /* Service: Save Layout */
    public saveLayoutService(bodyData: LayoutModel) {
        const request = new Request(this.tuxSaveLayoutUrl,
            {
                method: 'post',
                body: JSON.stringify(bodyData),
                headers: {
                    'Content-Type': 'application/json'
                }
            });

        return fetch(request)
            .then(this.validateResponse)
            // .then(this.readResponseAsJson)
            .catch(this.logError);
    }
}
