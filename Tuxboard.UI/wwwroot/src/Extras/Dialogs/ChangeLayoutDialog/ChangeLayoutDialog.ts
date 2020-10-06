import { BaseDialog } from "../../../core/BaseDialog";
import { Tab } from "../../../core/Tab";
import { LayoutItem } from "./LayoutItem";
import { Tuxboard } from "../../../Tuxboard";
import { dataId, isBefore, isLayoutListItem, getDataId } from "../../../core/common";
import { LayoutModel } from "./LayoutModel";
import { ChangeLayoutService } from "./ChangeLayoutService";
import { Modal, Dropdown } from "../../../../lib/bootstrap/dist/js/bootstrap.bundle";

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
    getLayoutOverlay() { return this.getLayoutDialog().querySelector(this.generalOverlaySelector); }
    getLayoutDialog() { return document.querySelector(this.layoutDialogSelector); }
    getLayoutList() { return this.getLayoutDialog().querySelector(this.layoutListSelector); }
    getDropdown() { return this.getLayoutDialog().querySelector(this.dropdownToggleSelector); }
    getLayoutListItems() { return this.getLayoutList().children; }
    getLayoutItemSelector(id:string) { return `${this.layoutItemSelector}[${dataId}="${id}"]`} // .layout-item[data-id="id"]

    getSaveLayoutButton() {
        const layoutDialog = this.getLayoutDialog();
        return layoutDialog.querySelector(this.saveLayoutButtonSelector);
    }

    setLayoutDialog(body: string) {
        const modalBody = this.getLayoutDialog().querySelector(this.dialogBodySelector);
        if (modalBody) {
            modalBody.innerHTML = body;
        }
    }

    initialize(layoutBody: string) {

        this.setLayoutDialog(layoutBody);

        // Bootstrap
        let dropdown = new Dropdown(this.getDropdown());

        this.initLayoutDragAndDrop();

        this.attachLayoutEvents();
        this.updateLayoutRowEvents();

        this.resetColumnStatus();
    }

    hide() {
        const modal = Modal.getInstance(this.getLayoutDialog()); // Returns a Bootstrap modal instance
        if (modal) {
            modal.hide();
        }
    }

    displayLayoutErrors(data) {
        const layoutDialog = this.getLayoutDialog();
        [].forEach.call(data.LayoutErrors,
            (item) => {
                const trow = layoutDialog.querySelector<HTMLDivElement>(`[data-id='${item.layoutRowId}']`);
                if (trow) {
                    trow.setAttribute("style", "outline: 1px solid #F00");
                } else {
                    trow.setAttribute("style", "");
                }
            });
    }

    saveCurrentLayout(ev: Event) {

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


    attachLayoutEvents() {

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

    updateLayoutRowEvents() {

        const layoutDialog = this.getLayoutDialog();

        // Delete Layout Button
        const deleteButtons = layoutDialog.querySelectorAll(this.deleteLayoutButtonSelector);
        [].forEach.call(deleteButtons,
            (el) => {
                el.addEventListener("click", (ev: Event) => this.layoutDeleteButtonClick(ev), { once: true });
            });
    }

    addLayoutRow(ev:Event) {
        const evTarget = ev.target as HTMLElement;
        const layoutTypeId = evTarget.attributes[dataId].value;
        this.layoutService.addLayoutRow(layoutTypeId)
            .then((data: string) => {
                this.updateLayoutData(data);
            });
    }

    getRowByEvent(ev) {
        const evTarget = ev.target;
        const id = evTarget.attributes[dataId].value;
        return this.getLayoutDialog().querySelector(this.getLayoutItemSelector(id)) as HTMLElement;
    }

    updateLayoutData(html:string) {
        this.resetColumnStatus();
        const columnElement = this.getLayoutList();
        columnElement.insertAdjacentHTML("beforeend", html);

        this.initLayoutDragAndDrop();

        this.updateLayoutRowEvents();
    }

    resetColumnStatus() {
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

    setColumnStatus(id:string, data) {
        const dialog = this.getLayoutDialog();
        const idSelector = `li[${dataId}='${id}']`;
        const item = dialog.querySelector(idSelector);
        const span = dialog.querySelector(this.layoutMessageSelector);
        span.innerHTML = data.text;
        item.setAttribute("style", "outline: 1px solid #F00");
    }

    layoutDeleteButtonClick(ev:Event) {
        const row = this.getRowByEvent(ev);
        if (!row) return;

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

    initLayoutDragAndDrop() {
        const layoutList = this.getLayoutList();
        const liList = this.getLayoutListItems();

        [].forEach.call(liList,
            (liItem: HTMLElement) => {
                const handle = liItem.querySelector(this.layoutListHandleSelector);
                handle.addEventListener("mousedown", handleMouseDown, false);
                handle.addEventListener("mouseup", handleMouseUp, false);
                const listItem = handle.parentNode.parentNode;
                listItem.addEventListener('dragstart', layoutDragStart, false);
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

            ev.dataTransfer.effectAllowed = 'move';

            let target = ev.target as HTMLLIElement;
            if (target instanceof HTMLLIElement) {
                ev.dataTransfer.setData('text', target.getAttribute(dataId));
            }
        }

        function layoutDragEnter(ev: DragEvent) {
            if (ev.preventDefault) ev.preventDefault();

            if (ev.target instanceof HTMLUListElement) {

                return true;
            } else if (ev.target instanceof HTMLLIElement) {
                ev.target.attributes["style"] = "border-bottom: 1px dashed #F00";
                return true;
            }

            return false;
        }

        function layoutDragLeave(ev: DragEvent) {
            if (ev.preventDefault) ev.preventDefault();

            if (ev.target instanceof HTMLUListElement) {

                return true;
            } else if (ev.target instanceof HTMLLIElement) {
                ev.target.attributes["style"] = "";
                return true;
            }

            return false;
        }

        function layoutDragOver(ev: DragEvent, dialog: ChangeLayoutDialog) {
            if (ev.preventDefault) ev.preventDefault();

            ev.dataTransfer.dropEffect = 'move';

            const target = ev.target as HTMLElement;

            const id = ev.dataTransfer.getData('text');

            // TODO: fix "layout-item"
            let elem = document.querySelector(".layout-item[data-id='" + id + "']");

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
            if (ev.preventDefault) ev.preventDefault();

            const id = ev.dataTransfer.getData('text');
            let elem = document.querySelector(".layout-item[data-id='" + id + "']");

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