import { DragWidgetInfo } from "../Models/DragWidgetInfo";
import { PlacementItem } from "../Models/PlacementItem";

export const dataId = "data-id";
export const isStaticAttribute = "data-static";
export const collapsedToggleSelector = "collapsed";

/* Widget Class (commonly used) */
//export let widgetSelector: string = ".card";

//export function setWidgetSelector(value:string) {
//    widgetSelector = value;
//}
//export function getWidgetSelector() {
//    return widgetSelector;
//}

export function getDataId(elem: HTMLElement) { return elem.getAttribute(dataId) }
export function getDomWidget(id: string) {
    const dId = `[${dataId}='${id}']`;
    return document.querySelector<HTMLDivElement>(dId);
}
export function getPlacementId(widget: HTMLDivElement) { return getDataId(widget); }
export function noPeriod(id: string) { return id.startsWith(".") ? id.replace(".", "") : id; }

export function disableElement(elem: Element) {
    elem.classList.add("disabled");
    elem.setAttribute("disabled", "disabled");
}
export function enableElement(elem: Element) {
    elem.classList.remove("disabled");
    elem.removeAttribute("disabled");
}
export function clearNodes(node: Element) {
    while (node.firstChild) {
        node.firstChild.remove();
    }
}

export function isWidget(target: HTMLElement, widgetSelector: string) {
    return (target.tagName.toLowerCase() === "div" &&
        target.classList.contains(noPeriod(widgetSelector)) &&
        getDataId(target) !== "");
}

export function getClosestByClass(element: HTMLElement, classToSearch: string) {
    while (element) {
        if (element.classList.contains(classToSearch)) {
            return element;
        }
        element = element.parentElement;
    }
}

export function getWidgetSnapshot(dragInfo: DragWidgetInfo) {
    // TODO: DefaultSelector.getInstance().layoutRowSelector));
    const widgetList = Array.from(document.querySelectorAll(".card"));
    return widgetList.map((elem: HTMLElement) => {
        const placementId = getDataId(elem);
        // TODO: DefaultSelector.getInstance().layoutRowSelector));
        const rowTemplate = getClosestByClass(elem, noPeriod(".layout-row"));
        const widgetIndex = getWidgetIndex(dragInfo, placementId);
        const columnIndex = getColumnIndexByDragInfo(dragInfo, placementId);
        const isStatic = elem.getAttribute(isStaticAttribute) === "true";
        return new PlacementItem(
            placementId,
            widgetIndex,
            rowTemplate.getAttribute(dataId),
            columnIndex,
            isStatic);
    });
}

export function getColumnByPlacement(dragInfo: DragWidgetInfo, placementId: string) {
    const placement = getDomWidget(placementId);
    return getClosestByClass(placement, noPeriod(".column")); // TODO: DefaultSelector.getInstance().columnSelector));
}

export function getColumnIndexByDragInfo(dragInfo: DragWidgetInfo, placementId: string) {
    const column = getColumnByPlacement(dragInfo, placementId);
    // TODO: DefaultSelector.getInstance().columnSelector))
    return Array.from(column.parentElement.querySelectorAll(".column"))
        .findIndex((column: HTMLElement) =>
            column.querySelector(`[${dataId}='${placementId}']`) != null);
}

export function getWidgetIndex(dragInfo: DragWidgetInfo, placementId:string) {
    const column = getColumnByPlacement(dragInfo, placementId);
    const columnWidgets = column.querySelectorAll(".card"); // TODO: DefaultSelector.getInstance().columnSelector))
    return Array.from(columnWidgets)
        .findIndex((widget: HTMLElement, index: number) => {
            return widget.getAttribute(dataId) === placementId;
        });
}

export function isBefore(el1: HTMLDivElement, el2: HTMLDivElement) {
    if (el2.parentNode === el1.parentNode) {
        let cur;
        for (cur = el1.previousSibling; cur; cur = cur.previousSibling) {
            if (cur === el2) {
                return true;
            }
        }
    }
    return false;
}

export function createFromHtml(htmlString: string) {
    const div = document.createElement("div");
    div.innerHTML = htmlString.trim();
    return Array.from(div.children);
}

export function isLayoutListItem(elem: HTMLElement) {
    return elem && elem.tagName.toLowerCase() === "li"
        && elem.classList.contains("layout-item"); // TODO: Layout-Item
}


