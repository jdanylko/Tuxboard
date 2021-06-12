import { BaseService } from "../../../core/BaseService";

export class AddWidgetService extends BaseService {

    tuxWidgetDialogUrl: string = "/widgetdialog/";
    tuxWidgetAddWidgetUrl: string = "/widgetdialog/addwidget/";

    constructor(debug: boolean = false) {
        super(debug);
    }

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

    }

    getWidgetDialogService() {
        const request = new Request(this.tuxWidgetDialogUrl);
        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }


}