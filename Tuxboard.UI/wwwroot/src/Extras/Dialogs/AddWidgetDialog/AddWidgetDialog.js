"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.AddWidgetDialog = void 0;
const BaseDialog_1 = require("../../../core/BaseDialog");
const common_1 = require("../../../core/common");
const AddWidgetService_1 = require("./AddWidgetService");
const TuxboardService_1 = require("../../../Services/TuxboardService");
const bootstrap_1 = require("bootstrap");
class AddWidgetDialog extends BaseDialog_1.BaseDialog {
    constructor(tuxboard, dialogSelector = null) {
        super(dialogSelector);
        this.tuxboard = tuxboard;
        this.service = new AddWidgetService_1.AddWidgetService();
        this.tuxboardService = new TuxboardService_1.TuxboardService();
        this.addWidgetDialogSelector = "#widget-dialog";
        this.widgetTabGroupSelector = ".widget-tabs";
        this.widgetListItemSelector = "a.widget-item";
        this.addWidgetButtonSelector = ".add-widget";
        this.widgetSelectionSelector = ".selected";
        this.selector = dialogSelector || this.addWidgetDialogSelector;
    }
    getDataId(elem) { return elem.getAttribute(common_1.dataId); }
    getWidgetDialog() { return document.querySelector(this.addWidgetDialogSelector); }
    getWidgetList() { return this.getWidgetDialog().querySelectorAll(this.widgetListItemSelector); }
    getAddWidgetButton() { return this.getWidgetDialog().querySelector(this.addWidgetButtonSelector); }
    getWidgetTabGroups() { return this.getWidgetDialog().querySelectorAll(this.widgetTabGroupSelector); }
    getSelectedWidget() { return this.getWidgetDialog().querySelector(this.widgetListItemSelector + this.widgetSelectionSelector); }
    getSelectedSelector() { return common_1.noPeriod(this.widgetSelectionSelector); }
    setWidgetDialog(body) {
        const modalBody = this.getWidgetDialog().querySelector(this.dialogBodySelector);
        if (modalBody)
            modalBody.innerHTML = body;
    }
    hide() {
        const modal = bootstrap_1.Modal.getInstance(this.getWidgetDialog()); // Returns a Bootstrap modal instance
        if (modal) {
            modal.hide();
        }
    }
    updateAddWidget() {
        const addWidgetButton = this.getAddWidgetButton(), selected = this.getSelectedWidget();
        if (selected) {
            common_1.enableElement(addWidgetButton);
        }
        else {
            common_1.disableElement(addWidgetButton);
        }
    }
    resetSelectedWidgets() {
        const widgets = this.getWidgetList();
        [].forEach.call(widgets, (item) => {
            item.classList.remove(this.getSelectedSelector());
        });
    }
    selectWidget(ev) {
        var target = ev.currentTarget;
        const isSelected = target.classList.contains(this.getSelectedSelector());
        this.resetSelectedWidgets();
        if (!isSelected) {
            target.classList.add(this.getSelectedSelector());
        }
        this.updateAddWidget();
    }
    addWidgetClick(ev) {
        const widget = this.getSelectedWidget();
        const widgetId = widget.getAttribute(common_1.dataId), tab = this.tuxboard.getTab();
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
            if (!response.success)
                return;
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
        tabTriggers.forEach(triggerEl => {
            var tabTrigger = new bootstrap_1.Tab(triggerEl);
            triggerEl.addEventListener('click', ev => {
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
exports.AddWidgetDialog = AddWidgetDialog;
//# sourceMappingURL=AddWidgetDialog.js.map