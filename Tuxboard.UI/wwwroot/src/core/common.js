"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.isLayoutListItem = exports.createFromHtml = exports.isBefore = exports.getWidgetIndex = exports.getColumnIndexByDragInfo = exports.getColumnByPlacement = exports.getWidgetSnapshot = exports.getClosestByClass = exports.isWidget = exports.clearNodes = exports.disableElement = exports.enableElement = exports.noPeriod = exports.getPlacementId = exports.getDomWidget = exports.getDataId = exports.collapsedToggleSelector = exports.isStaticAttribute = exports.dataId = void 0;
const PlacementItem_1 = require("../Models/PlacementItem");
exports.dataId = "data-id";
exports.isStaticAttribute = "data-static";
exports.collapsedToggleSelector = "collapsed";
/* Widget Class (commonly used) */
//export let widgetSelector: string = ".card";
//export function setWidgetSelector(value:string) {
//    widgetSelector = value;
//}
//export function getWidgetSelector() {
//    return widgetSelector;
//}
function getDataId(elem) { return elem.getAttribute(exports.dataId); }
exports.getDataId = getDataId;
function getDomWidget(id) {
    const dId = `[${exports.dataId}='${id}']`;
    return document.querySelector(dId);
}
exports.getDomWidget = getDomWidget;
function getPlacementId(widget) { return getDataId(widget); }
exports.getPlacementId = getPlacementId;
function noPeriod(id) { return id.startsWith(".") ? id.replace(".", "") : id; }
exports.noPeriod = noPeriod;
function enableElement(elem) { elem.classList.remove("disabled"); elem.removeAttribute("disabled"); }
exports.enableElement = enableElement;
function disableElement(elem) { elem.classList.add("disabled"); elem.setAttribute("disabled", "disabled"); }
exports.disableElement = disableElement;
function clearNodes(node) {
    // node.innerHTML = "";
    while (node.firstChild)
        node.firstChild.remove();
}
exports.clearNodes = clearNodes;
function isWidget(target, widgetSelector) {
    return (target.tagName.toLowerCase() === "div" &&
        target.classList.contains(noPeriod(widgetSelector)) &&
        getDataId(target) !== "");
}
exports.isWidget = isWidget;
function getClosestByClass(element, classToSearch) {
    while (element) {
        if (element.classList.contains(classToSearch)) {
            return element;
        }
        element = element.parentElement;
    }
}
exports.getClosestByClass = getClosestByClass;
function getWidgetSnapshot(dragInfo) {
    const widgetList = Array.from(document.querySelectorAll(".card")); // TODO: DefaultSelector.getInstance().layoutRowSelector));
    return widgetList.map((elem) => {
        const placementId = getDataId(elem);
        const rowTemplate = getClosestByClass(elem, noPeriod(".layout-row")); // TODO: DefaultSelector.getInstance().layoutRowSelector));
        const widgetIndex = getWidgetIndex(dragInfo, placementId);
        const columnIndex = getColumnIndexByDragInfo(dragInfo, placementId);
        const isStatic = elem.getAttribute(exports.isStaticAttribute) === "true";
        return new PlacementItem_1.PlacementItem(getDataId(elem), widgetIndex, rowTemplate.getAttribute(exports.dataId), columnIndex, isStatic);
    });
}
exports.getWidgetSnapshot = getWidgetSnapshot;
function getColumnByPlacement(dragInfo, placementId) {
    const placement = getDomWidget(placementId);
    return getClosestByClass(placement, noPeriod(".column")); // TODO: DefaultSelector.getInstance().columnSelector));
}
exports.getColumnByPlacement = getColumnByPlacement;
function getColumnIndexByDragInfo(dragInfo, placementId) {
    const column = getColumnByPlacement(dragInfo, placementId);
    return Array.from(column.parentElement.querySelectorAll(".column")) // TODO: DefaultSelector.getInstance().columnSelector))
        .findIndex((column) => column.querySelector(`[${exports.dataId}='${placementId}']`) != null);
}
exports.getColumnIndexByDragInfo = getColumnIndexByDragInfo;
function getWidgetIndex(dragInfo, placementId) {
    const column = getColumnByPlacement(dragInfo, placementId);
    const columnWidgets = column.querySelectorAll(".card"); // TODO: DefaultSelector.getInstance().columnSelector))
    return Array.from(columnWidgets)
        .findIndex((widget, index) => {
        return widget.getAttribute(exports.dataId) === placementId;
    });
}
exports.getWidgetIndex = getWidgetIndex;
function isBefore(el1, el2) {
    var cur;
    if (el2.parentNode === el1.parentNode) {
        for (cur = el1.previousSibling; cur; cur = cur.previousSibling) {
            if (cur === el2)
                return true;
        }
    }
    return false;
}
exports.isBefore = isBefore;
function createFromHtml(htmlString) {
    var div = document.createElement('div');
    div.innerHTML = htmlString.trim();
    return Array.from(div.children);
}
exports.createFromHtml = createFromHtml;
function isLayoutListItem(elem) {
    return elem && elem.tagName.toLowerCase() === "li" && elem.classList.contains("layout-item"); // TODO: Layout-Item
}
exports.isLayoutListItem = isLayoutListItem;
//# sourceMappingURL=common.js.map