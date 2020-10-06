import { BaseService } from "../../../core/BaseService";
import { dataId } from "../../../core/common";
import { LayoutModel } from "./LayoutModel";

export class ChangeLayoutService extends BaseService {

    private tuxLayoutDialogUrl: string = "/layoutdialog/";
    private tuxLayoutAddRowUrl: string = "/layoutdialog/addlayoutrow/";
    private tuxSaveLayoutUrl: string = "/layoutdialog/saveLayout/";
    private tuxDeleteLayoutRowUrl: string = "/layoutdialog/DeleteLayoutRow/";

    constructor(debug: boolean = false) {
        super(debug);
    }

    /* Service: Load Layout Dialog */
    getLayoutDialog(tabId: string) {
        const request = new Request(this.tuxLayoutDialogUrl + tabId,
            { method: "post" });

        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }

    /* Service: Add Layout Row */
    addLayoutRow(typeId: string) {
        const request = new Request(this.tuxLayoutAddRowUrl + typeId, { method: "post" });

        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }

    /* Service: Delete Row */
    deleteRowFromLayoutDialogService(row: HTMLElement) {
        const id = row.getAttribute(dataId);
        if (id === "0") { // new, we can remove it.
            row.remove();
        } else {
            const request = new Request(this.tuxDeleteLayoutRowUrl + id, { method: 'delete' });

            return fetch(request)
                .then(this.validateResponse)
                .then(this.readResponseAsJson)
                .catch(this.logError);
        }
    }

    /* Service: Save Layout */
    saveLayoutService(bodyData: LayoutModel) {
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