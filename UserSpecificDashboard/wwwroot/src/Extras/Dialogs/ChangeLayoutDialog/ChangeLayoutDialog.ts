import { BaseDialog } from "../../../core/BaseDialog";
import { ChangeLayoutService } from "./ChangeLayoutService";
import { dataId, getDataId, isBefore, isLayoutListItem } from "../../../core/common";
import { Dropdown, Modal } from "bootstrap";
import { LayoutItem } from "./LayoutItem";
import { LayoutModel } from "./LayoutModel";
import { Tab } from "../../../core/Tab";
import { Tuxboard } from "../../../Tuxboard";

export class ChangeLayoutDialog extends BaseDialog {

    private layoutService: ChangeLayoutService = new ChangeLayoutService();
    private currentTab: Tab;

    // selectors
    private layoutDialogSelector: string = "#layout-dialog";
    private saveLayoutButtonSelector: string = ".save-layout";
    private deleteLayoutButtonSelector: string = ".layout-delete-button";
    private layoutListSelector: string = ".layout-list";
    private layoutItemSelector: string = ".layout-item";
    private dropdownToggleSelector: string = ".dropdown-toggle";
    private layoutTypesSelector: string = ".layout-types a";
    private layoutListHandleSelector: string = ".handle";
    private layoutMessageSelector: string = "#layout-message";

    public canRefresh: boolean = false;

    constructor(
        private readonly tuxboard: Tuxboard,
        dialogSelector: string = null) {

        super(dialogSelector);

        this.selector = dialogSelector || this.layoutDialogSelector;

        this.currentTab = this.tuxboard.getTab();
    }

    /* Common: Utility */
    public getLayoutOverlay() {
        return this.getLayoutDialog().querySelector(this.generalOverlaySelector);
    }
    public getLayoutDialog() { return document.querySelector(this.layoutDialogSelector); }
    public getLayoutList() { return this.getLayoutDialog().querySelector(this.layoutListSelector); }
    public getDropdown() { return this.getLayoutDialog().querySelector(this.dropdownToggleSelector); }
    public getLayoutListItems() { return this.getLayoutList().children; }
    public getLayoutItemSelector(id: string) {
        return `${this.layoutItemSelector}[${dataId}="${id}"]`
    }

    public getSaveLayoutButton() {
        const layoutDialog = this.getLayoutDialog();
        return layoutDialog.querySelector(this.saveLayoutButtonSelector);
    }

    public setLayoutDialog(body: string) {
        const modalBody = this.getLayoutDialog().querySelector(this.dialogBodySelector);
        if (modalBody) {
            modalBody.innerHTML = body;
        }
    }

    public initialize(layoutBody: string) {

        this.setLayoutDialog(layoutBody);

        // Bootstrap
        const dropdown = new Dropdown(this.getDropdown());

        this.initLayoutDragAndDrop();

        this.attachLayoutEvents();
        this.updateLayoutRowEvents();

        this.resetColumnStatus();
    }

    public hide() {
        const modal = Modal.getInstance(this.getLayoutDialog()); // Returns a Bootstrap modal instance
        if (modal) {
            modal.hide();
        }
    }

    public displayLayoutErrors(data) {
        const layoutDialog = this.getLayoutDialog();
        [].forEach.call(data.LayoutErrors,
            (item) => {
                const trow = layoutDialog.querySelector<HTMLDivElement>(`[data-id='${item.layoutRowId}']`);
                if (trow) {
                    trow.setAttribute("style", "outline: 1px solid #F00");
                } else {
                    trow.removeAttribute("style");
                }
            });
    }

    public saveCurrentLayout(ev: Event) {

        const layoutData = new Array<LayoutItem>();

        [].forEach.call(this.getLayoutListItems(),
            (liItem: HTMLElement, index: number) => {
                const rowTypeId = liItem.getAttribute("data-row-type");
                let id = liItem.getAttribute(dataId);
                if (!id) {
                    id = "0";
                }
                layoutData.push(new LayoutItem(index, id, rowTypeId));
            });

        const postData = new LayoutModel(layoutData, this.currentTab.getCurrentTabId());

        this.layoutService.saveLayoutService(postData)
            .then(() => {
                this.hide();
                this.tuxboard.refresh();
            });
    }

    public attachLayoutEvents() {

        const layoutDialog = this.getLayoutDialog();

        // Save Layout
        const saveLayoutButton = this.getSaveLayoutButton();
        saveLayoutButton.addEventListener("click", (ev:Event) => this.saveCurrentLayout(ev), { once: true });

        // Layout Types in dropdown
        const links = layoutDialog.querySelectorAll(this.layoutTypesSelector);
        [].forEach.call(links, (item) => {
            item.addEventListener("click", (ev:Event) => this.addLayoutRow(ev), { once: true });
        });
    }

    public updateLayoutRowEvents() {

        const layoutDialog = this.getLayoutDialog();

        // Delete Layout Button
        const deleteButtons = layoutDialog.querySelectorAll(this.deleteLayoutButtonSelector);
        [].forEach.call(deleteButtons,
            (el) => {
                el.addEventListener("click", (ev: Event) => this.layoutDeleteButtonClick(ev), { once: true });
            });
    }

    public addLayoutRow(ev:Event) {
        const evTarget = ev.target as HTMLElement;
        const layoutTypeId = evTarget.attributes[dataId].value;
        this.layoutService.addLayoutRow(layoutTypeId)
            .then((data: string) => {
                this.updateLayoutData(data);
            });
    }

    public getRowByEvent(ev) {
        const evTarget = ev.target;
        const id = evTarget.attributes[dataId].value;
        return this.getLayoutDialog().querySelector(this.getLayoutItemSelector(id)) as HTMLElement;
    }

    public updateLayoutData(html:string) {
        this.resetColumnStatus();
        const columnElement = this.getLayoutList();
        columnElement.insertAdjacentHTML("beforeend", html);

        this.initLayoutDragAndDrop();

        this.updateLayoutRowEvents();
    }

    public resetColumnStatus() {
        const dialog = this.getLayoutDialog();
        const liList = this.getLayoutListItems();
        [].forEach.call(liList,
            liItem => {
                liItem.style = "";
            });

        const span = dialog.querySelector(this.layoutMessageSelector);
        if (span) {
            span.innerHTML = "";
        }
    }

    public setColumnStatus(id:string, data) {
        const dialog = this.getLayoutDialog();
        const idSelector = `li[${dataId}='${id}']`;
        const item = dialog.querySelector(idSelector);
        const span = dialog.querySelector(this.layoutMessageSelector);
        span.innerHTML = data.text;
        item.setAttribute("style", "outline: 1px solid #F00");
    }

    public layoutDeleteButtonClick(ev:Event) {
        const row = this.getRowByEvent(ev);
        if (!row) {
            return;
        }

        const id = getDataId(row);
        this.layoutService.deleteRowFromLayoutDialogService(row)
            .then((data) => {
                if (data.success) {
                    this.resetColumnStatus();
                    row.remove();
                } else {
                    this.setColumnStatus(id, data);
                }
            });
    }

    public initLayoutDragAndDrop() {
        const layoutList = this.getLayoutList();
        const liList = this.getLayoutListItems();

        [].forEach.call(liList,
            (liItem: HTMLElement) => {
                const handle = liItem.querySelector(this.layoutListHandleSelector);
                handle.addEventListener("mousedown", handleMouseDown, false);
                handle.addEventListener("mouseup", handleMouseUp, false);
                const listItem = handle.parentNode.parentNode;
                listItem.addEventListener("dragstart", layoutDragStart, false);
            });

        layoutList.addEventListener("dragover", (ev: DragEvent) => {
            layoutDragOver(ev, this);
        }, false);
        layoutList.addEventListener("dragenter", layoutDragEnter, false);
        layoutList.addEventListener("dragleave", layoutDragLeave, false);

        layoutList.addEventListener("drop", layoutDrop, false);
        layoutList.addEventListener("dragend", layoutDragEnd, false);

        function handleMouseDown(ev: MouseEvent) {
            const target = ev.target as HTMLElement;
            target.parentElement.parentElement.setAttribute("draggable", "true");
        }

        function handleMouseUp(ev: MouseEvent) {
            const target = ev.target as HTMLElement;
            target.parentElement.parentElement.setAttribute("draggable", "false");
        }

        function layoutDragStart(ev: DragEvent) {

            ev.dataTransfer.effectAllowed = "move";

            let target = ev.target as HTMLLIElement;
            if (target instanceof HTMLLIElement) {
                ev.dataTransfer.setData("text", target.getAttribute(dataId));
            }
        }

        function layoutDragEnter(ev: DragEvent) {
            if (ev.preventDefault) {
                ev.preventDefault();
            }

            if (ev.target instanceof HTMLUListElement) {

                return true;
            } else if (ev.target instanceof HTMLLIElement) {
                ev.target.attributes["style"] = "border-bottom: 1px dashed #F00";
                return true;
            }

            return false;
        }

        function layoutDragLeave(ev: DragEvent) {
            if (ev.preventDefault) {
                ev.preventDefault();
            }

            if (ev.target instanceof HTMLUListElement) {

                return true;
            } else if (ev.target instanceof HTMLLIElement) {
                ev.target.attributes["style"] = "";
                return true;
            }

            return false;
        }

        function layoutDragOver(ev: DragEvent, dialog: ChangeLayoutDialog) {
            if (ev.preventDefault) {
                ev.preventDefault();
            }

            ev.dataTransfer.dropEffect = "move";

            const target = ev.target as HTMLElement;

            const id = ev.dataTransfer.getData("text");

            // TODO: fix "layout-item"
            const elem: Element = document.querySelector(".layout-item[data-id='" + id + "']");

            if (elem && isLayoutListItem(target)) {

                if (isBefore(elem, target)) {
                    elem.insertBefore(elem, target);
                } else {
                    elem.insertBefore(elem, target.nextSibling);
                }

                target.classList.remove("over");
            }

            return false;
        }

        function layoutDrop(ev: DragEvent) {
            if (ev.preventDefault) {
                ev.preventDefault();
            }

            const id = ev.dataTransfer.getData("text");
            const elem: Element = document.querySelector(".layout-item[data-id='" + id + "']");

            let target = ev.target as HTMLElement;

            while (!isLayoutListItem(target)) {
                target = target.parentNode as HTMLElement;
            }

            if (elem && isLayoutListItem(target)) {

                const targetListItem = target.parentNode;

                if (isBefore(elem, target)) {
                    targetListItem.insertBefore(elem, target);
                } else {
                    targetListItem.insertBefore(elem, target.nextSibling);
                }

                target.classList.remove("over");
            }
        }

        function layoutDragEnd(ev: DragEvent) {
            const target = ev.target as HTMLElement;
            if (target) {
                target.setAttribute("draggable", "false");
            }
        }
    }


}