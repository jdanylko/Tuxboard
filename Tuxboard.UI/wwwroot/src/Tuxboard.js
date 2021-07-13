var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import { clearNodes, createFromHtml, dataId, getClosestByClass, getDataId, getWidgetSnapshot, isWidget, noPeriod } from "./core/common";
import { DragWidgetInfo } from "./Models/DragWidgetInfo";
import { Tab } from "./core/Tab";
import { Tuxbar } from "./Extras/Tuxbar/Tuxbar";
import { TuxboardService } from "./Services/TuxboardService";
export class Tuxboard {
    constructor(selector = null) {
        this.service = new TuxboardService();
        this.tuxboardSelector = ".dashboard";
        this.tuxboardSelector = selector || this.tuxboardSelector;
        this.initialize();
        this.updateAllWidgets();
    }
    getDom() {
        return document.querySelector(this.tuxboardSelector);
    }
    getDashboardId() {
        return getDataId(this.getDom());
    }
    initialize() {
        this.attachDragAndDropEvents();
    }
    getTab(reload = false) {
        if (!this.tab || reload) {
            if (this.tab) {
                delete this.tab;
            }
            this.tab = new Tab(this.getDom());
        }
        return this.tab;
    }
    addWidget(placementId) {
        this.service.getWidgetTemplate(placementId)
            .then((data) => {
            if (!data) {
                return;
            }
            const column = this.getFirstColumn();
            if (!column) {
                return;
            }
            this.addWidgetToColumn(column, data);
            const widgets = this.getWidgetsByColumn(column);
            const widgetList = widgets.getWidgets()
                .filter((item, index) => item.getPlacementId() === placementId);
            if (widgetList) {
                this.updateWidgets(widgetList);
            }
        });
    }
    refresh() {
        return __awaiter(this, void 0, void 0, function* () {
            yield this.service.refreshService()
                .then((data) => {
                const db = this.getDom();
                if (db) {
                    clearNodes(db);
                    const nodes = createFromHtml(data);
                    nodes.forEach((node) => db.insertAdjacentElement("beforeend", node)); // Layout Rows
                }
            })
                .catch((err) => console.log(err));
            this.initialize();
            this.updateAllWidgets();
        });
    }
    updateAllWidgets() {
        const widgetPlacements = this.getWidgetsByTab(this.getTab(true));
        this.updateWidgets(widgetPlacements);
    }
    updateWidgets(widgets) {
        Array.from(widgets).map((placement) => {
            placement.update();
        });
    }
    getWidgetsByTab(tab) {
        return tab.getLayout().getWidgetPlacements();
    }
    getLayoutByTab(tab) {
        return tab.getLayout();
    }
    getLayoutRowsByLayout(layout) {
        return layout.getLayoutRows();
    }
    getColumnsByLayoutRow(layoutRow) {
        return layoutRow.getColumns();
    }
    getWidgetsByColumn(column) {
        return column.getWidgetCollection();
    }
    hasWidgets(tab) {
        return this.getWidgetsByTab(tab).length > 0;
    }
    addWidgetToColumn(column, template) {
        if (column) {
            column.getDom().insertAdjacentHTML("beforeend", template);
        }
    }
    getFirstColumn() {
        const layout = this.getLayoutByTab(this.getTab());
        const columns = this.getColumnsByLayoutRow(layout.layoutRows[0]);
        return columns && columns.length > 0 ? columns[0] : null;
    }
    ////////////////////
    // Drag and Drop
    /////////////////////
    attachDragAndDropEvents() {
        const layout = this.getTab().getLayout();
        const columns = layout.getColumns();
        for (const column of columns) {
            column.getDom().addEventListener("dragstart", (ev) => {
                this.dragStart(ev, column, this);
            }, false);
            column.getDom().addEventListener("dragover", this.dragover, false);
            column.getDom().addEventListener("dragenter", this.dragenter, false);
            column.getDom().addEventListener("dragleave", this.dragLeave, false);
            column.getDom().addEventListener("drop", (ev) => { this.drop(ev, this); }, false);
            column.getDom().addEventListener("dragend", (ev) => { this.dragEnd(ev, this); }, false);
        }
    }
    dragStart(ev, column, self) {
        if (ev.stopPropagation) {
            ev.stopPropagation();
        }
        ev.dataTransfer.effectAllowed = 'move';
        const elem = ev.target;
        self.dragInfo = new DragWidgetInfo(elem.getAttribute(dataId), column.getIndex(), column.layoutRowId, column.getIndex(), column.layoutRowId);
        ev.dataTransfer.setData('text', JSON.stringify(self.dragInfo));
    }
    dragover(ev) {
        if (ev.preventDefault) {
            ev.preventDefault();
        }
        if (ev.stopPropagation) {
            ev.stopPropagation();
        }
        ev.dataTransfer.dropEffect = 'move';
        const target = ev.target;
        return isWidget(target, ".card"); // TODO: DefaultSelector.getInstance().widgetSelector);
    }
    dragenter(ev) {
        if (ev.preventDefault) {
            ev.preventDefault();
        }
        if (ev.stopPropagation) {
            ev.stopPropagation();
        }
        const target = ev.target;
        if (target) {
            target.classList.add('over');
        }
    }
    dragLeave(ev) {
        if (ev.preventDefault) {
            ev.preventDefault();
        }
        if (ev.stopPropagation) {
            ev.stopPropagation();
        }
        const target = ev.target;
        if (target) {
            target.classList.remove("over");
        }
    }
    drop(ev, self) {
        if (ev.preventDefault) {
            ev.preventDefault();
        }
        if (ev.stopPropagation) {
            ev.stopPropagation();
        }
        const targetElement = ev.target; // .column or .card
        self.dragInfo = JSON.parse(ev.dataTransfer.getData("text"));
        const draggedWidget = document.querySelector(`[${dataId}='${self.dragInfo.placementId}'`);
        if (isWidget(targetElement, ".card")) { // TODO: DefaultSelector.getInstance().widgetSelector)}) {
            targetElement.insertBefore(draggedWidget, targetElement);
            // TODO: } else if (targetElement.classList.contains(noPeriod(DefaultSelector.getInstance().columnSelector))) {
        }
        else if (targetElement.classList.contains(noPeriod(".column"))) {
            const closestWidget = getClosestByClass(targetElement, 
            // TODO: noPeriod(DefaultSelector.getInstance().widgetSelector));
            noPeriod(".card"));
            if (closestWidget) {
                targetElement.insertBefore(draggedWidget, closestWidget);
            }
            else {
                targetElement.append(draggedWidget);
            }
        }
    }
    dragEnd(ev, self) {
        if (ev.preventDefault) {
            ev.preventDefault();
        }
        if (ev.stopPropagation) {
            ev.stopPropagation();
        }
        document.querySelectorAll(".column") // TODO: DefaultSelector.getInstance().columnSelector)
            .forEach((elem) => elem.classList.remove("over"));
        const id = self.dragInfo.placementId;
        self.dragInfo.placementList = getWidgetSnapshot(self.dragInfo);
        const selected = self.dragInfo.placementList
            .filter((elem) => elem.PlacementId === id);
        if (selected && selected.length > 0) {
            self.dragInfo.currentLayoutRowId = selected[0].LayoutRowId;
            self.dragInfo.currentColumnIndex = selected[0].ColumnIndex;
        }
        this.service.saveWidgetPlacementService(ev, self.dragInfo)
            .then((result) => console.log("Saved. Message: " + result));
        ev.dataTransfer.clearData();
    }
}
window.addEventListener("DOMContentLoaded", () => {
    const dashboard = new Tuxboard();
    const tuxbar = new Tuxbar(dashboard);
});
