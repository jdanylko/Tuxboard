import { AddWidgetService } from "./AddWidgetService";
import { BaseDialog } from "../../../core/BaseDialog";
import { dataId, disableElement, enableElement, noPeriod } from "../../../core/common";
import { Modal, Tab } from "bootstrap";
import { Tuxboard } from "../../../Tuxboard";
import { TuxboardService } from "../../../Services/TuxboardService";
import { WidgetPlacement } from "../../../Widget/WidgetPlacement";

export class AddWidgetDialog extends BaseDialog {

    private service: AddWidgetService = new AddWidgetService();
    private tuxboardService: TuxboardService = new TuxboardService();

    private addWidgetDialogSelector: string = "#widget-dialog";
    private widgetTabGroupSelector: string = ".widget-tabs";
    private widgetListItemSelector: string = "a.widget-item";
    private addWidgetButtonSelector: string = ".add-widget";
    private widgetSelectionSelector: string = ".selected";

    constructor(
        private readonly tuxboard: Tuxboard,
        dialogSelector: string = null) {

        super(dialogSelector);

        this.selector = dialogSelector || this.addWidgetDialogSelector;
    }

    public getDataId(elem: HTMLElement) {
        return elem.getAttribute(dataId)
    }
    public getWidgetDialog() {
        return document.querySelector(this.addWidgetDialogSelector);
    }

    public getWidgetList() {
        return this.getWidgetDialog().querySelectorAll(this.widgetListItemSelector);
    }

    public getAddWidgetButton() {
        return this.getWidgetDialog().querySelector(this.addWidgetButtonSelector);
    }

    public getWidgetTabGroups() {
        return this.getWidgetDialog().querySelectorAll(this.widgetTabGroupSelector);
    }

    public getSelectedWidget() {
        return this.getWidgetDialog().querySelector(
            this.widgetListItemSelector + this.widgetSelectionSelector);
    }

    public getSelectedSelector() {
        return noPeriod(this.widgetSelectionSelector);
    }

    public setWidgetDialog(body: string) {
        const modalBody = this.getWidgetDialog().querySelector(this.dialogBodySelector);
        if (modalBody) {
            modalBody.innerHTML = body;
        }
    }

    public getToken(): string {
        const dialog = this.getWidgetDialog();
        return dialog.querySelector('input[name="__RequestVerificationToken"]').getAttribute("value");
    }


    public hide() {
        const modal = Modal.getInstance(this.getWidgetDialog());
        if (modal) {
            modal.hide();
        }
    }

    public updateAddWidget() {
        const addWidgetButton = this.getAddWidgetButton();
        const selected = this.getSelectedWidget();
        if (selected) {
            enableElement(addWidgetButton);
        } else {
            disableElement(addWidgetButton);
        }
    }

    public resetSelectedWidgets() {
        const widgets = this.getWidgetList();
        [].forEach.call(widgets,
            (item: HTMLElement) => {
                item.classList.remove(this.getSelectedSelector());
            });
    }

    public selectWidget(ev: Event) {
        const target = ev.currentTarget as HTMLDivElement;
        const isSelected = target.classList.contains(this.getSelectedSelector());

        this.resetSelectedWidgets();

        if (!isSelected) {
            target.classList.add(this.getSelectedSelector());
        }

        this.updateAddWidget();
    }

    public addWidgetClick(ev: Event) {
        const widget = this.getSelectedWidget();
        const widgetId = widget.getAttribute(dataId);
        const tab = this.tuxboard.getTab();
        const layout = tab.getLayout();
        const layoutRow = layout.getFirstLayoutRow();
        const columns = layoutRow.getColumns();
        const column = columns && columns.length > 0 ? columns[0] : null;
        const token = this.getToken();

        this.service.addWidgetService(tab.getCurrentTabId(), widgetId, token)
            .then( (data:string) => {
                if (!data) {
                    return;
                }

                const response = JSON.parse(data);
                if (!response.success) {
                    return;
                }

                this.hide();
                const columnDom = column.getDom();
                columnDom.insertAdjacentHTML("beforeend", response.template);
                const placements = tab.getLayout()
                    .getWidgetPlacements()
                    .filter((item: WidgetPlacement, index: number) => item.getPlacementId() ===
                        response.placementId);
                this.tuxboard.updateWidgets(placements);
            });
    }


    public setupWidgetClicks() {
        const widgetAddButton = this.getAddWidgetButton();
        widgetAddButton.addEventListener("click",
            (ev: Event) => this.addWidgetClick(ev),
            { once: true });

        [].forEach.call(this.getWidgetList(),
            (item: HTMLElement) => {
                item.addEventListener("click", (ev: Event) => this.selectWidget(ev) );
            });
    }

    public setupWidgetTabs() {
        const tabTriggers = this.getWidgetTabGroups();

        tabTriggers.forEach((triggerEl): void => {

            const tabTrigger = new Tab(triggerEl);

            triggerEl.addEventListener("click",
                (ev:Event): void => {
                    ev.preventDefault();
                    tabTrigger.show();
                });
        });
    }

    public initialize(modalBody: string) {
        this.setWidgetDialog(modalBody);
        this.setupWidgetTabs();
        this.setupWidgetClicks();
    }

}