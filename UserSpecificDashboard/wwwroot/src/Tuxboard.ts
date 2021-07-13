import { Column } from "./core/Column";
import {
    clearNodes,
    createFromHtml,
    dataId,
    getClosestByClass,
    getDataId,
    getWidgetSnapshot,
    isWidget,
    noPeriod } from "./core/common";
import { DragWidgetInfo } from "./Models/DragWidgetInfo";
import { Layout } from "./core/Layout";
import { LayoutRow } from "./core/LayoutRow";
import { PlacementItem } from "./Models/PlacementItem";
import { Tab } from "./core/Tab";
import { Tuxbar } from "./Extras/Tuxbar/Tuxbar";
import { TuxboardService } from "./Services/TuxboardService";
import { WidgetPlacement } from "./Widget/WidgetPlacement";

export class Tuxboard {

    private dragInfo: DragWidgetInfo;
    private service: TuxboardService = new TuxboardService();

    private tab: Tab;
    private tuxboardSelector: string = ".dashboard";

    constructor(selector: string = null) {

        this.tuxboardSelector = selector || this.tuxboardSelector;

        this.initialize();
        this.updateAllWidgets();
    }

    public getDom(): HTMLElement {
        return document.querySelector<HTMLElement>(this.tuxboardSelector);
    }

    public getDashboardId(): string {
        return getDataId(this.getDom());
    }

    public initialize() {
        this.attachDragAndDropEvents();
    }

    public getTab(reload: boolean = false) {
        if (!this.tab || reload) {
            if (this.tab) {
                delete this.tab;
            }
            this.tab = new Tab(this.getDom());
        }
        return this.tab;
    }

    public addWidget(placementId: string) {
        this.service.getWidgetTemplate(placementId)
            .then((data: string) => {
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
                    .filter((item: WidgetPlacement, index: number) => item.getPlacementId() === placementId);
                if (widgetList) {
                    this.updateWidgets(widgetList);
                }
            });

    }

    public async refresh() {
        await this.service.refreshService()
            .then((data: string) => {
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
    }

    public updateAllWidgets() {
        const widgetPlacements = this.getWidgetsByTab(this.getTab(true));
        this.updateWidgets(widgetPlacements);
    }

    public updateWidgets(widgets: WidgetPlacement[]) {
        Array.from(widgets).map((placement: WidgetPlacement) => {
            placement.update();
        });
    }

    public getWidgetsByTab(tab: Tab) {
        return tab.getLayout().getWidgetPlacements();
    }

    public getLayoutByTab(tab: Tab) {
        return tab.getLayout();
    }

    public getLayoutRowsByLayout(layout: Layout) {
        return layout.getLayoutRows();
    }

    public getColumnsByLayoutRow(layoutRow: LayoutRow) {
        return layoutRow.getColumns();
    }

    public getWidgetsByColumn(column: Column) {
        return column.getWidgetCollection();
    }

    public hasWidgets(tab: Tab) {
        return this.getWidgetsByTab(tab).length > 0;
    }

    public addWidgetToColumn(column: Column, template: string) {
        if (column) {
            column.getDom().insertAdjacentHTML("beforeend", template);
        }
    }

    public getFirstColumn() {
        const layout = this.getLayoutByTab(this.getTab());
        const columns = this.getColumnsByLayoutRow(layout.layoutRows[0]);
        return columns && columns.length > 0 ? columns[0] : null;
    }




    ////////////////////
    // Drag and Drop
    /////////////////////

    public attachDragAndDropEvents() {
        const layout = this.getTab().getLayout();
        const columns = layout.getColumns();

        for (const column of columns) {
            column.getDom().addEventListener("dragstart", (ev: DragEvent) => {
                this.dragStart(ev, column, this);
            }, false);
            column.getDom().addEventListener("dragover", this.dragover, false);
            column.getDom().addEventListener("dragenter", this.dragenter, false);
            column.getDom().addEventListener("dragleave", this.dragLeave, false);
            column.getDom().addEventListener("drop", (ev: DragEvent) => { this.drop(ev, this) }, false);
            column.getDom().addEventListener("dragend", (ev: DragEvent) => { this.dragEnd(ev, this); }, false);
        }
    }

    public dragStart(ev: DragEvent, column: Column, self: Tuxboard) {

        if (ev.stopPropagation) {
            ev.stopPropagation();
        }

        ev.dataTransfer.effectAllowed = 'move';

        const elem = ev.target as HTMLElement;

        self.dragInfo = new DragWidgetInfo(
            elem.getAttribute(dataId),
            column.getIndex(),
            column.layoutRowId,
            column.getIndex(),
            column.layoutRowId);

        ev.dataTransfer.setData('text', JSON.stringify(self.dragInfo));
    }

    public dragover(ev: DragEvent) {
        if (ev.preventDefault) {
            ev.preventDefault();
        }
        if (ev.stopPropagation) {
            ev.stopPropagation();
        }

        ev.dataTransfer.dropEffect = 'move';

        const target = ev.target as HTMLElement;

        return isWidget(target, ".card"); // TODO: DefaultSelector.getInstance().widgetSelector);
    }

    public dragenter(ev: DragEvent) {
        if (ev.preventDefault) {
            ev.preventDefault();
        }
        if (ev.stopPropagation) {
            ev.stopPropagation();
        }

        const target = ev.target as HTMLElement;
        if (target) {
            target.classList.add('over');
        }
    }

    public dragLeave(ev: DragEvent) {
        if (ev.preventDefault) {
            ev.preventDefault();
        }
        if (ev.stopPropagation) {
            ev.stopPropagation();
        }

        const target = ev.target as HTMLElement;
        if (target) {
            target.classList.remove("over");
        }
    }

    public drop(ev: DragEvent, self: Tuxboard) {
        if (ev.preventDefault) {
            ev.preventDefault();
        }
        if (ev.stopPropagation) {
            ev.stopPropagation();
        }

        const targetElement = ev.target as HTMLElement; // .column or .card

        self.dragInfo = JSON.parse(ev.dataTransfer.getData("text"));

        const draggedWidget = document.querySelector(`[${dataId}='${self.dragInfo.placementId}'`);

        if (isWidget(targetElement, ".card")) { // TODO: DefaultSelector.getInstance().widgetSelector)}) {
            targetElement.insertBefore(draggedWidget, targetElement);
            // TODO: } else if (targetElement.classList.contains(noPeriod(DefaultSelector.getInstance().columnSelector))) {
        } else if (targetElement.classList.contains(noPeriod(".column"))) {
            const closestWidget: HTMLElement = getClosestByClass(targetElement,
                // TODO: noPeriod(DefaultSelector.getInstance().widgetSelector));
                noPeriod(".card"));
            if (closestWidget) {
                targetElement.insertBefore(draggedWidget, closestWidget);
            } else {
                targetElement.append(draggedWidget);
            }
        }
    }

    public dragEnd(ev: DragEvent, self: Tuxboard) {
        if (ev.preventDefault) {
            ev.preventDefault();
        }
        if (ev.stopPropagation) {
            ev.stopPropagation();
        }

        document.querySelectorAll(".column") // TODO: DefaultSelector.getInstance().columnSelector)
            .forEach((elem: HTMLElement) => elem.classList.remove("over"));

        const id = self.dragInfo.placementId;

        self.dragInfo.placementList = getWidgetSnapshot(self.dragInfo);

        const selected = self.dragInfo.placementList
            .filter((elem: PlacementItem) => elem.PlacementId === id);
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
    const dashboard: Tuxboard = new Tuxboard();
    const tuxbar: Tuxbar = new Tuxbar(dashboard);
});

