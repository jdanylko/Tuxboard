import { BaseDialog } from "../../../core/BaseDialog";
import { dataId, enableElement, disableElement, noPeriod } from "../../../core/common";
import { AddWidgetService } from "./AddWidgetService";
import { Tuxboard } from "../../../Tuxboard";
import { WidgetPlacement } from "../../../Widget/WidgetPlacement";
import { TuxboardService } from "../../../Services/TuxboardService";
import { Tab, Modal } from "../../../../lib/bootstrap/dist/js/bootstrap.bundle.js";

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

    getDataId(elem: HTMLElement) { return elem.getAttribute(dataId) }
    getWidgetDialog() { return document.querySelector(this.addWidgetDialogSelector); }
    getWidgetList() { return this.getWidgetDialog().querySelectorAll(this.widgetListItemSelector); }
    getAddWidgetButton() { return this.getWidgetDialog().querySelector(this.addWidgetButtonSelector); }
    getWidgetTabGroups() { return this.getWidgetDialog().querySelectorAll(this.widgetTabGroupSelector); }
    getSelectedWidget() { return this.getWidgetDialog().querySelector(this.widgetListItemSelector + this.widgetSelectionSelector); }
    getSelectedSelector() { return noPeriod(this.widgetSelectionSelector); }

    setWidgetDialog(body: string) {
        const modalBody = this.getWidgetDialog().querySelector(this.dialogBodySelector);
        if (modalBody) modalBody.innerHTML = body;
    }

    hide() {
        const modal = Modal.getInstance(this.getWidgetDialog()); // Returns a Bootstrap modal instance
        if (modal) {
            modal.hide();
        }
    }

    updateAddWidget() {
        const addWidgetButton = this.getAddWidgetButton(),
            selected = this.getSelectedWidget();
        if (selected) {
            enableElement(addWidgetButton);
        } else {
            disableElement(addWidgetButton);
        }
    }

    resetSelectedWidgets() {
        const widgets = this.getWidgetList();
        [].forEach.call(widgets,
            (item: HTMLElement) => {
                item.classList.remove(this.getSelectedSelector());
            });
    }

    selectWidget(ev: Event) {
        var target = ev.currentTarget as HTMLDivElement;
        const isSelected = target.classList.contains(this.getSelectedSelector());

        this.resetSelectedWidgets();

        if (!isSelected) {
            target.classList.add(this.getSelectedSelector());
        }

        this.updateAddWidget();
    }

    addWidgetClick(ev: Event) {
        const widget = this.getSelectedWidget();
        const widgetId = widget.getAttribute(dataId),
            tab = this.tuxboard.getTab();
        const layout = tab.getLayout();
        const layoutRow = layout.getFirstLayoutRow();
        const columns = layoutRow.getColumns();
        const column = columns && columns.length > 0 ? columns[0] : null;

        this.service.addWidgetService(tab.getCurrentTabId(), widgetId)
            .then( (data:string) => {
                if (!data) {
                    return;
                }

                const response = JSON.parse(data);
                if (!response.success) return;

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


    setupWidgetClicks() {
        const widgetAddButton = this.getAddWidgetButton();
        widgetAddButton.addEventListener("click",
            (ev: Event) => this.addWidgetClick(ev),
            { once: true });

        [].forEach.call(this.getWidgetList(),
            (item: HTMLElement) => {
                item.addEventListener("click", (ev: Event) => this.selectWidget(ev), { once: true } );
            });
    }

    setupWidgetTabs() {
        const tabTriggers = this.getWidgetTabGroups();

        tabTriggers.forEach(triggerEl => {

            var tabTrigger = new Tab(triggerEl);

            triggerEl.addEventListener('click',
                ev => {
                    ev.preventDefault();
                    tabTrigger.show();
                });
        });
    }

    initialize(modalBody: string) {
        this.setWidgetDialog(modalBody);
        this.setupWidgetTabs();
        this.setupWidgetClicks();
    }

}