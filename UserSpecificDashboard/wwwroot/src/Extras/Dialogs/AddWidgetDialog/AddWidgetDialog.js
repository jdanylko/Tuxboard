import { AddWidgetService } from "./AddWidgetService";
import { BaseDialog } from "../../../core/BaseDialog";
import { dataId, disableElement, enableElement, noPeriod } from "../../../core/common";
import { Modal, Tab } from "bootstrap";
import { TuxboardService } from "../../../Services/TuxboardService";
export class AddWidgetDialog extends BaseDialog {
    constructor(tuxboard, dialogSelector = null) {
        super(dialogSelector);
        this.tuxboard = tuxboard;
        this.service = new AddWidgetService();
        this.tuxboardService = new TuxboardService();
        this.addWidgetDialogSelector = "#widget-dialog";
        this.widgetTabGroupSelector = ".widget-tabs";
        this.widgetListItemSelector = "a.widget-item";
        this.addWidgetButtonSelector = ".add-widget";
        this.widgetSelectionSelector = ".selected";
        this.selector = dialogSelector || this.addWidgetDialogSelector;
    }
    getDataId(elem) {
        return elem.getAttribute(dataId);
    }
    getWidgetDialog() {
        return document.querySelector(this.addWidgetDialogSelector);
    }
    getWidgetList() {
        return this.getWidgetDialog().querySelectorAll(this.widgetListItemSelector);
    }
    getAddWidgetButton() {
        return this.getWidgetDialog().querySelector(this.addWidgetButtonSelector);
    }
    getWidgetTabGroups() {
        return this.getWidgetDialog().querySelectorAll(this.widgetTabGroupSelector);
    }
    getSelectedWidget() {
        return this.getWidgetDialog().querySelector(this.widgetListItemSelector + this.widgetSelectionSelector);
    }
    getSelectedSelector() {
        return noPeriod(this.widgetSelectionSelector);
    }
    setWidgetDialog(body) {
        const modalBody = this.getWidgetDialog().querySelector(this.dialogBodySelector);
        if (modalBody) {
            modalBody.innerHTML = body;
        }
    }
    hide() {
        const modal = Modal.getInstance(this.getWidgetDialog());
        if (modal) {
            modal.hide();
        }
    }
    updateAddWidget() {
        const addWidgetButton = this.getAddWidgetButton();
        const selected = this.getSelectedWidget();
        if (selected) {
            enableElement(addWidgetButton);
        }
        else {
            disableElement(addWidgetButton);
        }
    }
    resetSelectedWidgets() {
        const widgets = this.getWidgetList();
        [].forEach.call(widgets, (item) => {
            item.classList.remove(this.getSelectedSelector());
        });
    }
    selectWidget(ev) {
        const target = ev.currentTarget;
        const isSelected = target.classList.contains(this.getSelectedSelector());
        this.resetSelectedWidgets();
        if (!isSelected) {
            target.classList.add(this.getSelectedSelector());
        }
        this.updateAddWidget();
    }
    addWidgetClick(ev) {
        const widget = this.getSelectedWidget();
        const widgetId = widget.getAttribute(dataId);
        const tab = this.tuxboard.getTab();
        const layout = tab.getLayout();
        const layoutRow = layout.getFirstLayoutRow();
        const columns = layoutRow.getColumns();
        const column = columns && columns.length > 0 ? columns[0] : null;
        this.service.addWidgetService(tab.getCurrentTabId(), widgetId)
            .then((data) => {
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
                .filter((item, index) => item.getPlacementId() ===
                response.placementId);
            this.tuxboard.updateWidgets(placements);
        });
    }
    setupWidgetClicks() {
        const widgetAddButton = this.getAddWidgetButton();
        widgetAddButton.addEventListener("click", (ev) => this.addWidgetClick(ev), { once: true });
        [].forEach.call(this.getWidgetList(), (item) => {
            item.addEventListener("click", (ev) => this.selectWidget(ev), { once: true });
        });
    }
    setupWidgetTabs() {
        const tabTriggers = this.getWidgetTabGroups();
        tabTriggers.forEach((triggerEl) => {
            const tabTrigger = new Tab(triggerEl);
            triggerEl.addEventListener("click", (ev) => {
                ev.preventDefault();
                tabTrigger.show();
            });
        });
    }
    initialize(modalBody) {
        this.setWidgetDialog(modalBody);
        this.setupWidgetTabs();
        this.setupWidgetClicks();
    }
}
