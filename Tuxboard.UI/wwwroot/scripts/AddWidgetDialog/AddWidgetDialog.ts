import { BaseDialog } from "../BaseDialog";
import { Tab } from "../Tab";

export class AddWidgetDialog extends BaseDialog {

    public static tuxWidgetDialog: string = "widget-dialog"; // default is #widget-dialog
    public static tuxWidgetTabGroup: string = ".widget-tabs"; // left-side group in #widget-dialog
    public static tuxWidgetListItem: string = "a.widget-item"; // each widget on right in #widget-dialog
    public static tuxWidgetAdd: string = ".add-widget"; // Add Widget button
    public static tuxWidgetTools: string = "card-tools"; // buttons on each widget
    public static tuxWidgetSelection: string = ".selected"; // selected widgets

    public tuxWidgetOverlay: string = this.tuxOverlay + ".loading-status"; // overlay on each dialog box/widget for loading

    constructor() { super(); }

    getDataId(elem: HTMLElement) { return elem.getAttribute("data-id") }
    getWidgetDialog() { return document.getElementById(AddWidgetDialog.tuxWidgetDialog); }
    getWidgetList() { return this.getWidgetDialog().querySelectorAll<HTMLDivElement>(AddWidgetDialog.tuxWidgetListItem); }
    getAddWidgetButton() { return this.getWidgetDialog().querySelector<HTMLInputElement>(AddWidgetDialog.tuxWidgetAdd); }
    getWidgetTabGroups() { return this.getWidgetDialog().querySelector<HTMLDivElement>(AddWidgetDialog.tuxWidgetTabGroup); }
    getSelectedWidgets() { return this.getWidgetDialog().querySelectorAll<HTMLDivElement>(AddWidgetDialog.tuxWidgetListItem + AddWidgetDialog.tuxWidgetSelection); }
    enableElement(elem: HTMLElement) { elem.classList.remove("disabled"); elem.removeAttribute("disabled"); }
    disableElement(elem: HTMLElement) { elem.classList.add("disabled"); elem.setAttribute("disabled", "disabled"); }

    setWidgetDialog(body) {
        const modalBody = this.getWidgetDialog().querySelector<HTMLElement>(this.modalBody);
        modalBody.innerHTML = body;
    }

    updateAddWidget() {
        const addWidgetButton = this.getAddWidgetButton(),
            selected = this.getSelectedWidgets();
        if (selected.length > 0) {
            this.enableElement(addWidgetButton);
        } else {
            this.disableElement(addWidgetButton);
        }
    }

    resetSelectedWidgets() {
        const widgets = this.getSelectedWidgets();
        [].forEach.call(widgets,
            (item) => {
                item.classList.remove("selected");
            });
    }

    selectWidget(ev: Event) {
        var target = ev.currentTarget as HTMLDivElement;
        const isSelected = target.classList.contains("selected");

        this.resetSelectedWidgets();

        if (!isSelected) {
            target.classList.add("selected");
        }

        this.updateAddWidget();
    }

    addWidgetClick() {
        const widget = this.getSelectedWidgets();
        const widgetId = this.getDataId(widget[0]),
            tab = Tab.getCurrentTab();

        this.addWidgetService(tab.value, widgetId);
    }

    setupWidgetClicks() {
        const widgetAddButton = this.getAddWidgetButton();
        widgetAddButton.addEventListener("click", this.addWidgetClick);

        [].forEach.call(this.getWidgetList(),
            (item) => {
                item.addEventListener("click", this.selectWidget);
            });
    }

    setupWidgetTabs() {
        const widgetTabs = this.getWidgetTabGroups(),
            tabTriggers = Array.from(widgetTabs.getElementsByTagName("A"));

        for (let i = 0; i < tabTriggers.length; i++) {
            new Tab(tabTriggers[i], {});
        }
    }

    setWidgetEvents() {
        this.setupWidgetTabs();
        this.setupWidgetClicks();
    }

}