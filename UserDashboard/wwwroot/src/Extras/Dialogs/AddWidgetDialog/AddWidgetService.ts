import { BaseService } from "../../../core/BaseService";

export class AddWidgetService extends BaseService {

    private tuxWidgetDialogUrl: string = "/widgetdialog/";
    private tuxWidgetAddWidgetUrl: string = "/widgetdialog?handler=AddWidget";

    constructor(debug: boolean = false) {
        super(debug);
    }

    public getWidgetDialogService() {
        const request = new Request(this.tuxWidgetDialogUrl);
        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);
    }

    public addWidgetService(tabId: string, widgetId: string, token:string) {
        const request = new Request(this.tuxWidgetAddWidgetUrl,
            {
                method: "post",
                body: JSON.stringify({
                    TabId: tabId,
                    WidgetId: widgetId
                }),
                headers: {
                    "Content-Type": "application/json",
                    'RequestVerificationToken': token,
                }
            }
        );
        return fetch(request)
            .then(this.validateResponse)
            .then(this.readResponseAsText)
            .catch(this.logError);

    }

}