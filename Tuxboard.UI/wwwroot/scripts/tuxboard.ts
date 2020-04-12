import { Tab } from "./Tab";
import { Column } from "./Column";
import { Widget } from "./Widget";
import { RowTemplate } from "./RowTemplate";
import { Tuxbar } from "./Tuxbar";

export class Tuxboard {
    public dashboardId: string = ".dashboard";

    private _debug: boolean;

    private _tabs: Tab[];
    private _tuxbar: Tuxbar = new Tuxbar();

    //private _layoutDialog: ChangeLayoutDialog = new ChangeLayoutDialog();
    //private _addWidgetDialog: AddWidgetDialog = new AddWidgetDialog();

    constructor(debug: boolean = false) {
        this._debug = debug;
    }

    getDashboard() { return document.querySelector<HTMLDivElement>(this.dashboardId); }
    getDashboardColumns() { return getDashboard().querySelectorAll<HTMLDivElement>(Column.columnId); }

    initialize() {
        initDashboardDragAndDrop();

        this.setupDialogs();

        this.attachWidgetEvents();

        this.refreshWidgets();
    }

    setupDialogs() {

        // Layout - onShow event
        const layoutDialog = this.getLayoutDialog();
        layoutDialog.addEventListener("show.bs.modal", getLayoutDialogService, false);

        // Widget - onShow event
        const widgetDialog = document.getElementById(tuxWidgetDialog);
        widgetDialog.addEventListener("show.bs.modal", getWidgetDialogService, false);

        // Show dialogs on button trigger
        const layoutButton = document.getElementById(tuxLayoutButton);
        if (layoutButton) {
            layoutButton.addEventListener("click", showDialog, false);
        }

        const widgetButton = document.getElementById(tuxWidgetButton);
        if (widgetButton) {
            widgetButton.addEventListener("click", showDialog, false);
        }
    }

    initDashboardDragAndDrop() {
        for (const container of this.getDashboardColumns()) {
            container.addEventListener('dragstart', dragStart, false);
            container.addEventListener("dragover", dragover, false);
            container.addEventListener("dragenter", dragenter, false);
            container.addEventListener("dragleave", dragLeave, false);
            container.addEventListener("drop", drop, false);
            container.addEventListener("dragend", dragEnd, false);
        }

        function dragStart(ev: DragEvent) {

            if (ev.stopPropagation) ev.stopPropagation();

            ev.dataTransfer.effectAllowed = 'move';

            widgetDragEl = ev.target as HTMLDivElement;
            if (!previousColumn) {
                previousColumn = getColumnIndex(getColumn(ev));
            }
            if (!previousLayout) {
                previousLayout = getCurrentLayoutId(ev);
            }

            const id = widgetDragEl.attributes['data-id'].value;

            ev.dataTransfer.setData('text', id);
        }

        function dragover(ev) {
            if (ev.preventDefault) ev.preventDefault();
            if (ev.stopPropagation) ev.stopPropagation();

            ev.dataTransfer.dropEffect = 'move';

            return isWidget(ev.target);
        }

        function dragenter(ev) {
            if (ev.preventDefault) ev.preventDefault();
            if (ev.stopPropagation) ev.stopPropagation();

            ev.target.classList.add('over');
        }

        function dragLeave(ev) {
            if (ev.preventDefault) ev.preventDefault();
            if (ev.stopPropagation) ev.stopPropagation();

            ev.target.classList.remove("over");
        }

        function drop(ev) {
            if (ev.preventDefault) ev.preventDefault();
            if (ev.stopPropagation) ev.stopPropagation();

            const targetElement = ev.target; // the element under the dragged element.

            const column = getClosestByClass(ev.target, noPeriod(Column.columnId));
            if (column) {
                if (isWidget(targetElement)) {
                    column.insertBefore(widgetDragEl, targetElement);
                } else {
                    const widget = getClosestByClass(targetElement, noPeriod(Widget.widgetId));
                    if (widget) {
                        column.insertBefore(widgetDragEl, widget);
                    } else {
                        column.append(widgetDragEl);
                    }
                }
            }
        }

        function dragEnd(ev: DragEvent) {
            if (ev.preventDefault) ev.preventDefault();
            if (ev.stopPropagation) ev.stopPropagation();

            var target = ev.target as HTMLDivElement;
            target.setAttribute("style", "");

            const containers = document.getElementsByClassName(noPeriod(Column.columnId));
            [].forEach.call(containers,
                function (col) {
                    col.classList.remove('over');
                });

            this.saveWidgetPlacementService(ev);

            ev.dataTransfer.clearData();
            widgetDragEl = null;
            previousColumn = null;
            previousLayout = null;
        }
    }

    attachWidgetEvents() {
        const dashboard = getDashboard();
        const cardTools = dashboard.getElementsByClassName(tuxWidgetToolDropdown);
        [].forEach.call(cardTools,
            (elem) => {
                // re-attach events for new Bootstrap elements.
                // from https://github.com/thednp/bootstrap.native/wiki/FAQs
                new Dropdown(elem);
            });

        const widgetsOnDashboard = getWidgetsOnDashboard();
        [].forEach.call(widgetsOnDashboard,
            (elem) => {
                setupWidgetToolbar(elem);
            });
    }

    refreshWidgets() {
        const widgetsOnDashboard = getWidgetsOnDashboard();
        [].forEach.call(widgetsOnDashboard,
            (elem) => {
                refreshWidget(elem);
            });
    }

    getWidgetProperties(ev: Event) {
        const column = getParentByClass(ev, noPeriod(Column.columnId)) as HTMLDivElement,
            widgets = column.getElementsByClassName(Widget.widgetId),
            layoutRow = getParentByClass(ev, noPeriod(RowTemplate.rowTemplateId));

        var target = ev.target as HTMLElement;
        return {
            PlacementId: getDataId(target),
            Column: getColumnIndex(column),
            LayoutRowId: getDataId(layoutRow),
            PlacementList: Array.from(widgets)
                .map(function (elem: HTMLElement, index: number) {
                    return {
                        PlacementId: getDataId(elem),
                        Index: index
                    }
                })
        };
    }

}

const debug = false;

// const tuxDashboardColumn = ".column";
// const tuxDashboardTab = ".dashboard-tab";
// const tuxRowTemplate = ".row-template";

// const tuxWidgetClass = "card";
// const tuxWidgetTitle = ".card-title";

// const tuxLayoutButton = "layout-button"; // button to trigger the layout dialog.
// const tuxWidgetButton = "widget-button"; // button to trigger the widget dialog.

// const cssModalBody = ".modal-body"; // for Bootstrap, the class to replace HTML in a dialog box.

/* Layout Dialog */
//const tuxLayoutDialog = "layout-dialog"; // default is #layout-dialog
//const tuxSaveLayoutButton = ".save-layout"; // save layout button.
//const tuxLayoutDeleteButton = ".layout-delete-button";
//const tuxLayoutList = ".layout-list";
//const tuxLayoutItem = "layout-item";
//const tuxLayoutTypes = ".layout-types a";
//const tuxLayoutListHandle = ".handle";
//const tuxLayoutMessage = "#layout-message";

/* Widget Dialog */
//const tuxWidgetDialog = "widget-dialog"; // default is #widget-dialog
//const tuxWidgetTabGroup = ".widget-tabs"; // left-side group in #widget-dialog
//const tuxWidgetListItem = "a.widget-item"; // each widget on right in #widget-dialog
//const tuxWidgetAdd = ".add-widget"; // Add Widget button
//const tuxWidgetTools = "card-tools"; // buttons on each widget

const tuxOverlay = ".overlay";
const tuxWidgetOverlay = ".overlay.loading-status"; // overlay on each dialog box/widget for loading

const tuxLayoutDialogDropdown = "btnLayoutDropdown"; // Layout dropdown button
//const tuxWidgetToolDropdown = "dropdown-card-tool"; // Widget's tool dropdown button
//const tuxWidgetSelection = ".selected"; // class added to a widget when selected

/* Widget Settings */
//const tuxWidgetSettings = ".widget-settings"; // classname
//const tuxWidgetSettingsCancel = ".settings-cancel"; // cancel button
//const tuxWidgetSettingsSave = ".settings-save"; // save button
//const tuxWidgetInputs = ".setting-value"; // input classes

/* Service Urls */
//const tuxLayoutDialogUrl = "/layoutdialog/";
//const tuxLayoutAddRowUrl = "/layoutdialog/addlayoutrow/";
//const tuxSaveLayoutUrl = "/layoutdialog/saveLayout/";
//const tuxDeleteLayoutRowUrl = "/layoutdialog/DeleteLayoutRow/";

//const tuxWidgetDialogUrl = "/widgetdialog/";
//const tuxWidgetSettingsUrl = "/widgetsettings/";
//const tuxWidgetAddWidgetUrl = "/widgetdialog/addwidget/";

// API calls
//const tuxRefreshTuxboardUrl = "/Tuxboard/Get/";
//const tuxToolCollapseUrl = "/Tuxboard/PostCollapse/";
//const tuxWidgetPlacementUrl = "/Tuxboard/Put/";
//const tuxWidgetRemoveWidgetUrl = "/Tuxboard/removewidget/";
//const tuxWidgetContentUrl = "/Widget/";
//const tuxWidgetSaveSettingsUrl = "/WidgetSettings/Save/";


/* Widget header toolbar */
//const tuxWidgetToolCollapse = "collapse-widget";
//const tuxWidgetToolRemove = "remove-widget";
const tuxWidgetCollapsed = "collapsed"; // IsCollapsed

const tuxRefreshOption = ".refresh-option";
const tuxSettingsOption = ".settings-option";

let layoutDragEl = null;
let widgetDragEl = null;
let previousColumn = null;
let previousLayout = null;

let layoutDialogInstance = null;
let widgetDialogInstance = null;

/*
 * **********************************
 * Common getters
 * */
function getDashboard() { return document.querySelector<HTMLDivElement>(".dashboard"); }
function getDataId(elem: HTMLElement) { return elem.getAttribute("data-id") }
function getDomWidget(id: string) { return document.querySelector<HTMLDivElement>(`[data-id='${id}']`); }
function getWidgetSettings(widget: HTMLDivElement) { return widget.querySelector<HTMLDivElement>(tuxWidgetSettings) }
function getPlacementId(widget: HTMLDivElement) { return getDataId(widget); }
//function getCurrentTab() { return document.querySelector<HTMLDivElement>(tuxDashboardTab + "[data-active='true']").attributes["data-id"]; }
//function getDashboardColumns() { return getDashboard().querySelectorAll<HTMLDivElement>(tuxDashboardColumn); }
function getCurrentLayoutId(ev: Event) { var layoutRow = getParentByClass(ev, noPeriod(tuxRowTemplate)); return getDataId(layoutRow); }
function getColumnIndex(column: HTMLDivElement) { return parseInt(column.getAttribute("data-column")); }
function getColumn(ev: Event) { return getParentByClass(ev, noPeriod(tuxDashboardColumn)); }
function getWidgetsOnDashboard() { return getDashboard().querySelectorAll<HTMLDivElement>(tuxWidgetClass); }
//function getLayoutDialog() { return document.getElementById(tuxLayoutDialog); }
//function getLayoutList() { return getLayoutDialog().querySelector(tuxLayoutList); }
//function getLayoutListItems() { return getLayoutList().children; }
//function getLayoutOverlay() { return getLayoutDialog().querySelector<HTMLDivElement>(tuxOverlay); }
//function getWidgetTabGroups() { return getWidgetDialog().querySelector<HTMLDivElement>(tuxWidgetTabGroup); }
//function getWidgetDialog() { return document.getElementById(tuxWidgetDialog); }
//function getAddWidgetButton() { return getWidgetDialog().querySelector<HTMLInputElement>(tuxWidgetAdd); }
function isCollapsed(widget: HTMLDivElement) { return widget.classList.contains(tuxWidgetCollapsed) }
function getWidgetList() { return getWidgetDialog().querySelectorAll<HTMLDivElement>(tuxWidgetListItem); }
//function getSelectedWidgets() { return getWidgetDialog().querySelectorAll<HTMLDivElement>(tuxWidgetListItem + tuxWidgetSelection); }
function toId(id) { return id.startsWith("#") ? id : "#" + id; }
function toClass(id) { return id.startsWith(".") ? id : "." + id; }
function noPeriod(id: string) { return id.startsWith(".") ? id.replace(".", "") : id; }

function getSettingValues(widget: HTMLDivElement) {
    let inputs = widget.querySelectorAll<HTMLDivElement>(tuxWidgetInputs);
    return Array.from(inputs).map((elem: HTMLInputElement, index: number) => {
        return {
            WidgetSettingId: getDataId(elem),
            Value: elem.value
        }
    });
}

function isBefore(el1: HTMLElement, el2: HTMLElement) {
    var cur;
    if (el2.parentNode === el1.parentNode) {
        for (cur = el1.previousSibling; cur; cur = cur.previousSibling) {
            if (cur === el2) return true;
        }
    }
    return false;
}

function isTargetListItem(ev: Event) {
    var target = ev.target as HTMLElement;
    return target.tagName.toLowerCase() === "li"
        && target.classList.contains(tuxLayoutItem);
}

//function getWidgetProperties(ev: Event) {
//    const column = getParentByClass(ev, noPeriod(Column.columnId)) as HTMLDivElement,
//        widgets = column.getElementsByClassName(Widget.widgetId),
//        layoutRow = getParentByClass(ev, noPeriod(RowTemplate.rowTemplateId));

//    var target = ev.target as HTMLElement;
//    return {
//        PlacementId: getDataId(target),
//        Column: getColumnIndex(column),
//        LayoutRowId: getDataId(layoutRow),
//        PlacementList: Array.from(widgets)
//            .map(function (elem: HTMLElement, index: number) {
//                return {
//                    PlacementId: getDataId(elem),
//                    Index: index
//                }
//            })
//    };
//}

//function getSaveLayoutButton() {
//    const layoutDialog = getLayoutDialog();
//    return layoutDialog.querySelector<HTMLElement>(tuxSaveLayoutButton);
//}

//function getWidgetTools(widgetElem) {
//    return widgetElem.getElementsByClassName(tuxWidgetTools).item(0);
//}

//function setLayoutDialog(body) {
//    const modalBody = getLayoutDialog().querySelector<HTMLElement>(cssModalBody);
//    modalBody.innerHTML = body;
//}

//function setWidgetDialog(body) {
//    const modalBody = getWidgetDialog().querySelector<HTMLElement>(cssModalBody);
//    modalBody.innerHTML = body;
//}

//function enableElement(elem: HTMLElement) {
//    elem.classList.remove("disabled");
//    elem.removeAttribute("disabled");
//}

//function disableElement(elem: HTMLElement) {
//    elem.classList.add("disabled");
//    elem.setAttribute("disabled", "disabled");
//}

function getParentByClass(ev: Event, selector: string) {
    let current = ev.target as HTMLDivElement;
    if (current.classList.contains(selector))
        return current;

    while (current.parentNode !== null && !current.classList.contains(selector)) {

        current = current.parentNode as HTMLDivElement;
    }
    return current;
}

function getClosestByClass(element: HTMLElement, classToSearch: string) {
    if (!element) {
        return false;
    } else if (element.classList.contains(classToSearch)) {
        return element;
    } else {
        var elem = element.parentElement as HTMLElement;
        return getClosestByClass(elem, classToSearch);
    }
}

function getClosestByTag(ev: Event, targetTagName: string) {
    let current = ev.target as HTMLElement;
    if (current.tagName.toLowerCase() === targetTagName.toLowerCase())
        return current;

    while (current.parentNode !== null && current.tagName.toLowerCase() !== targetTagName.toLowerCase()) {

        current = current.parentNode as HTMLElement;
    }
    return current;
}

function isWidget(elem: HTMLElement) {
    if (elem) {
        return (elem.tagName.toLowerCase() === "div"
            && elem.classList.contains(tuxWidgetClass)
            && getDataId(elem) !== "");
    }
    return false;
}

/*
 * **********************************
 * Dashboard Events
 * */
function initDashboardDragAndDrop() {
    for (const container of getDashboardColumns()) {
        container.addEventListener('dragstart', dragStart, false);
        container.addEventListener("dragover", dragover, false);
        container.addEventListener("dragenter", dragenter, false);
        container.addEventListener("dragleave", dragLeave, false);
        container.addEventListener("drop", drop, false);
        container.addEventListener("dragend", dragEnd, false);
    }

    function dragStart(ev: DragEvent) {

        if (ev.stopPropagation) ev.stopPropagation();

        ev.dataTransfer.effectAllowed = 'move';

        widgetDragEl = ev.target as HTMLDivElement;
        if (!previousColumn) {
            previousColumn = getColumnIndex(getColumn(ev));
        }
        if (!previousLayout) {
            previousLayout = getCurrentLayoutId(ev);
        }

        const id = widgetDragEl.attributes['data-id'].value;

        ev.dataTransfer.setData('text', id);
    }

    function dragover(ev) {
        if (ev.preventDefault) ev.preventDefault();
        if (ev.stopPropagation) ev.stopPropagation();

        ev.dataTransfer.dropEffect = 'move';

        return isWidget(ev.target);
    }

    function dragenter(ev) {
        if (ev.preventDefault) ev.preventDefault();
        if (ev.stopPropagation) ev.stopPropagation();

        ev.target.classList.add('over');
    }

    function dragLeave(ev) {
        if (ev.preventDefault) ev.preventDefault();
        if (ev.stopPropagation) ev.stopPropagation();

        ev.target.classList.remove("over");
    }

    function drop(ev) {
        if (ev.preventDefault) ev.preventDefault();
        if (ev.stopPropagation) ev.stopPropagation();

        const targetElement = ev.target; // the element under the dragged element.

        const column = getClosestByClass(ev.target, noPeriod(tuxDashboardColumn));
        if (column) {
            if (isWidget(targetElement)) {
                column.insertBefore(widgetDragEl, targetElement);
            } else {
                const widget = getClosestByClass(targetElement, noPeriod(tuxWidgetClass));
                if (widget) {
                    column.insertBefore(widgetDragEl, widget);
                } else {
                    column.append(widgetDragEl);
                }
            }
        }
    }

    function dragEnd(ev: DragEvent) {
        if (ev.preventDefault) ev.preventDefault();
        if (ev.stopPropagation) ev.stopPropagation();

        var target = ev.target as HTMLDivElement;
        target.setAttribute("style", "");

        const containers = document.getElementsByClassName(noPeriod(tuxDashboardColumn));
        [].forEach.call(containers,
            function (col) {
                col.classList.remove('over');
            });

        saveWidgetPlacementService(ev);

        ev.dataTransfer.clearData();
        widgetDragEl = null;
        previousColumn = null;
        previousLayout = null;
    }
}


// Dashboard Utilities
function showDialog(ev) {
    const elem = getClosestByTag(ev, "button"),
        target = elem.getAttribute("data-target"),
        isLayout = elem.getAttribute("id") === tuxLayoutButton,
        isWidget = elem.getAttribute("id") === tuxWidgetButton,
        dialog = document.querySelector(target);

    if (dialog) {
        if (isLayout) {
            if (!layoutDialogInstance) {
                layoutDialogInstance = new Modal(dialog);
            }
            layoutDialogInstance.show();
        }
        if (isWidget) {
            if (!widgetDialogInstance) {
                widgetDialogInstance = new Modal(dialog);
            }
            widgetDialogInstance.show();
        }
    }
}

//function refreshWidget(widget) {
//    getWidgetService(widget);
//}

//function refreshWidgets() {
//    const widgetsOnDashboard = getWidgetsOnDashboard();
//    [].forEach.call(widgetsOnDashboard,
//        (elem) => {
//            refreshWidget(elem);
//        });
//}

///******************************
// * Widget Setup/Settings
// */

//function setupWidgetToolbar(elem) {

//    const tools = getWidgetTools(elem);
//    if (!tools) return;

//    const removeButton = tools.getElementsByClassName(tuxWidgetToolRemove).item(0);

//    if (removeButton) {
//        removeButton.addEventListener("click", removeWidgetService, false);
//    }

//    const minimizeButton = tools.getElementsByClassName(tuxWidgetToolCollapse).item(0);
//    if (minimizeButton) {
//        minimizeButton.addEventListener("click", minimizeWidget, false);
//    }

//    /* dropdown options */
//    const settingsOption = elem.getElementsByClassName(noPeriod(tuxSettingsOption)).item(0);
//    if (settingsOption) {
//        settingsOption.addEventListener("click", settingsClick, false);
//    }

//    const refreshOption = elem.getElementsByClassName(noPeriod(tuxRefreshOption)).item(0);
//    if (refreshOption) {
//        refreshOption.addEventListener("click", refreshClick, false);
//    }
//}

//function minimizeWidget(ev) {
//    const widget = getParentByClass(ev, tuxWidgetClass);
//    if (widget) {
//        const minimized = widget.classList.contains(tuxWidgetCollapsed);
//        if (minimized) {
//            widget.classList.remove(tuxWidgetCollapsed);
//            showWidgetBody(widget);
//            updateCollapsedWidgetService(widget, 0);
//        } else {
//            widget.classList.add(tuxWidgetCollapsed);
//            hideWidgetBody(widget);
//            updateCollapsedWidgetService(widget, 1);
//        }
//    }
//}

//function attachSettingEvents(widget: HTMLDivElement) {

//    let saveButton = widget.querySelector(tuxWidgetSettingsSave);
//    saveButton.addEventListener("click", saveSettingsClick, false);

//    let cancelButton = widget.querySelector(tuxWidgetSettingsCancel);
//    cancelButton.addEventListener("click", cancelSettingsClick, false);
//}

//function cancelSettingsClick(ev: Event) {
//    let widget = getParentByClass(ev, tuxWidgetClass);
//    if (widget) {
//        hideWidgetSettings(widget);
//        showWidgetBody(widget);
//    }
//}

//function saveSettingsClick(ev: Event) {
//    let widget = getParentByClass(ev, tuxWidgetClass) as HTMLDivElement;
//    if (widget) {
//        saveSettingsService(widget);
//    }
//}

//function settingsClick(ev) {
//    let widget = getParentByClass(ev, tuxWidgetClass);
//    if (widget) {
//        getWidgetSettingsService(getPlacementId(widget));
//    }
//}

//function refreshClick(ev) {
//    const widget = getParentByClass(ev, tuxWidgetClass);
//    if (widget) {
//        getWidgetService(widget);
//    }
//}

//function attachWidgetEvents() {
//    const dashboard = getDashboard();
//    const cardTools = dashboard.getElementsByClassName(tuxWidgetToolDropdown);
//    [].forEach.call(cardTools,
//        (elem) => {
//            // re-attach events for new Bootstrap elements.
//            // from https://github.com/thednp/bootstrap.native/wiki/FAQs
//            new Dropdown(elem);
//        });

//    const widgetsOnDashboard = getWidgetsOnDashboard();
//    [].forEach.call(widgetsOnDashboard,
//        (elem) => {
//            setupWidgetToolbar(elem);
//        });
//}

//function hideWidgetBody(widget) {
//    const body = widget.querySelector(".card-body");
//    if (body) {
//        body.setAttribute("hidden", "");
//    }
//}

//function showWidgetBody(widget) {
//    const body = widget.querySelector(".card-body");
//    if (body) {
//        body.removeAttribute("hidden");
//    }
//}

//function showOverlay(widget) {
//    const overlay = widget.querySelector(tuxWidgetOverlay);
//    if (overlay && overlay.hasAttribute("hidden")) {
//        overlay.removeAttribute("hidden");
//    }
//}

//function hideOverlay(widget) {
//    const overlay = widget.querySelector(tuxWidgetOverlay);
//    if (overlay && !overlay.hasAttribute("hidden")) {
//        overlay.setAttribute("hidden", "");
//    }
//}

//function showWidgetSettings(widget) {
//    const settings = widget.querySelector(tuxWidgetSettings);
//    if (settings && settings.hasAttribute("hidden")) {
//        settings.removeAttribute("hidden");
//    }
//}

//function hideWidgetSettings(widget) {
//    const settings = widget.querySelector(tuxWidgetSettings);
//    if (settings && !settings.hasAttribute("hidden")) {
//        settings.setAttribute("hidden", "");
//    }
//}


///***
// * Initialization
// */
//function setupDialogs() {

//    // Layout - onShow event
//    const layoutDialog = getLayoutDialog();
//    layoutDialog.addEventListener("show.bs.modal", getLayoutDialogService, false);

//    // Widget - onShow event
//    const widgetDialog = document.getElementById(tuxWidgetDialog);
//    widgetDialog.addEventListener("show.bs.modal", getWidgetDialogService, false);

//    // Show dialogs on button trigger
//    const layoutButton = document.getElementById(tuxLayoutButton);
//    if (layoutButton) {
//        layoutButton.addEventListener("click", showDialog, false);
//    }

//    const widgetButton = document.getElementById(tuxWidgetButton);
//    if (widgetButton) {
//        widgetButton.addEventListener("click", showDialog, false);
//    }
//}

//function initialize() {
//    initDashboardDragAndDrop();

//    setupDialogs();

//    attachWidgetEvents();

//    refreshWidgets();
//}

//initialize();
