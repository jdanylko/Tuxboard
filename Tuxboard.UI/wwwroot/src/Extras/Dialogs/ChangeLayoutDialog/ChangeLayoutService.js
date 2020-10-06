"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ChangeLayoutService = void 0;
const BaseService_1 = require("../../../core/BaseService");
const common_1 = require("../../../core/common");
class ChangeLayoutService extends BaseService_1.BaseService {
    constructor(debug = false) {
        super(debug);
        this.tuxLayoutDialogUrl = "/layoutdialog/";
        this.tuxLayoutAddRowUrl = "/layoutdialog/addlayoutrow/";
        this.tuxSaveLayoutUrl = "/layoutdialog/saveLayout/";
        this.tuxDeleteLayoutRowUrl = "/layoutdialog/DeleteLayoutRow/";
    }
    /* Service: Load Layout Dialog */
    getLayoutDialog(tabId) {
        const request = new Request(this.tuxLayoutDialogUrl + tabId, { method: "post" });
        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }
    /* Service: Add Layout Row */
    addLayoutRow(typeId) {
        const request = new Request(this.tuxLayoutAddRowUrl + typeId, { method: "post" });
        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }
    /* Service: Delete Row */
    deleteRowFromLayoutDialogService(row) {
        const id = row.getAttribute(common_1.dataId);
        if (id === "0") { // new, we can remove it.
            row.remove();
        }
        else {
            const request = new Request(this.tuxDeleteLayoutRowUrl + id, { method: 'delete' });
            return fetch(request)
                .then(this.validateResponse)
                .then(this.readResponseAsJson)
                .catch(this.logError);
        }
    }
    /* Service: Save Layout */
    saveLayoutService(bodyData) {
        const request = new Request(this.tuxSaveLayoutUrl, {
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
exports.ChangeLayoutService = ChangeLayoutService;
//# sourceMappingURL=ChangeLayoutService.js.map