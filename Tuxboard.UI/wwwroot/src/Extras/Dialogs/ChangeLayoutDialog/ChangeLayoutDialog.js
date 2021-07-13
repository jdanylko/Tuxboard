import { BaseDialog } from "../../../core/BaseDialog";
import { ChangeLayoutService } from "./ChangeLayoutService";
import { dataId, getDataId, isBefore, isLayoutListItem } from "../../../core/common";
import { Dropdown, Modal } from "bootstrap";
import { LayoutItem } from "./LayoutItem";
import { LayoutModel } from "./LayoutModel";
export class ChangeLayoutDialog extends BaseDialog {
    constructor(tuxboard, dialogSelector = null) {
        super(dialogSelector);
        this.tuxboard = tuxboard;
        this.layoutService = new ChangeLayoutService();
        // selectors
        this.layoutDialogSelector = "#layout-dialog";
        this.saveLayoutButtonSelector = ".save-layout";
        this.deleteLayoutButtonSelector = ".layout-delete-button";
        this.layoutListSelector = ".layout-list";
        this.layoutItemSelector = ".layout-item";
        this.dropdownToggleSelector = ".dropdown-toggle";
        this.layoutTypesSelector = ".layout-types a";
        this.layoutListHandleSelector = ".handle";
        this.layoutMessageSelector = "#layout-message";
        this.canRefresh = false;
        this.selector = dialogSelector || this.layoutDialogSelector;
        this.currentTab = this.tuxboard.getTab();
    }
    /* Common: Utility */
    getLayoutOverlay() {
        return this.getLayoutDialog().querySelector(this.generalOverlaySelector);
    }
    getLayoutDialog() { return document.querySelector(this.layoutDialogSelector); }
    getLayoutList() { return this.getLayoutDialog().querySelector(this.layoutListSelector); }
    getDropdown() { return this.getLayoutDialog().querySelector(this.dropdownToggleSelector); }
    getLayoutListItems() { return this.getLayoutList().children; }
    getLayoutItemSelector(id) {
        return `${this.layoutItemSelector}[${dataId}="${id}"]`;
    }
    getSaveLayoutButton() {
        const layoutDialog = this.getLayoutDialog();
        return layoutDialog.querySelector(this.saveLayoutButtonSelector);
    }
    setLayoutDialog(body) {
        const modalBody = this.getLayoutDialog().querySelector(this.dialogBodySelector);
        if (modalBody) {
            modalBody.innerHTML = body;
        }
    }
    initialize(layoutBody) {
        this.setLayoutDialog(layoutBody);
        // Bootstrap
        const dropdown = new Dropdown(this.getDropdown());
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
        [].forEach.call(data.LayoutErrors, (item) => {
            const trow = layoutDialog.querySelector(`[data-id='${item.layoutRowId}']`);
            if (trow) {
                trow.setAttribute("style", "outline: 1px solid #F00");
            }
            else {
                trow.removeAttribute("style");
            }
        });
    }
    saveCurrentLayout(ev) {
        const layoutData = new Array();
        [].forEach.call(this.getLayoutListItems(), (liItem, index) => {
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
        saveLayoutButton.addEventListener("click", (ev) => this.saveCurrentLayout(ev), { once: true });
        // Layout Types in dropdown
        const links = layoutDialog.querySelectorAll(this.layoutTypesSelector);
        [].forEach.call(links, (item) => {
            item.addEventListener("click", (ev) => this.addLayoutRow(ev), { once: true });
        });
    }
    updateLayoutRowEvents() {
        const layoutDialog = this.getLayoutDialog();
        // Delete Layout Button
        const deleteButtons = layoutDialog.querySelectorAll(this.deleteLayoutButtonSelector);
        [].forEach.call(deleteButtons, (el) => {
            el.addEventListener("click", (ev) => this.layoutDeleteButtonClick(ev), { once: true });
        });
    }
    addLayoutRow(ev) {
        const evTarget = ev.target;
        const layoutTypeId = evTarget.attributes[dataId].value;
        this.layoutService.addLayoutRow(layoutTypeId)
            .then((data) => {
            this.updateLayoutData(data);
        });
    }
    getRowByEvent(ev) {
        const evTarget = ev.target;
        const id = evTarget.attributes[dataId].value;
        return this.getLayoutDialog().querySelector(this.getLayoutItemSelector(id));
    }
    updateLayoutData(html) {
        this.resetColumnStatus();
        const columnElement = this.getLayoutList();
        columnElement.insertAdjacentHTML("beforeend", html);
        this.initLayoutDragAndDrop();
        this.updateLayoutRowEvents();
    }
    resetColumnStatus() {
        const dialog = this.getLayoutDialog();
        const liList = this.getLayoutListItems();
        [].forEach.call(liList, liItem => {
            liItem.style = "";
        });
        const span = dialog.querySelector(this.layoutMessageSelector);
        if (span) {
            span.innerHTML = "";
        }
    }
    setColumnStatus(id, data) {
        const dialog = this.getLayoutDialog();
        const idSelector = `li[${dataId}='${id}']`;
        const item = dialog.querySelector(idSelector);
        const span = dialog.querySelector(this.layoutMessageSelector);
        span.innerHTML = data.text;
        item.setAttribute("style", "outline: 1px solid #F00");
    }
    layoutDeleteButtonClick(ev) {
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
            }
            else {
                this.setColumnStatus(id, data);
            }
        });
    }
    initLayoutDragAndDrop() {
        const layoutList = this.getLayoutList();
        const liList = this.getLayoutListItems();
        [].forEach.call(liList, (liItem) => {
            const handle = liItem.querySelector(this.layoutListHandleSelector);
            handle.addEventListener("mousedown", handleMouseDown, false);
            handle.addEventListener("mouseup", handleMouseUp, false);
            const listItem = handle.parentNode.parentNode;
            listItem.addEventListener("dragstart", layoutDragStart, false);
        });
        layoutList.addEventListener("dragover", (ev) => {
            layoutDragOver(ev, this);
        }, false);
        layoutList.addEventListener("dragenter", layoutDragEnter, false);
        layoutList.addEventListener("dragleave", layoutDragLeave, false);
        layoutList.addEventListener("drop", layoutDrop, false);
        layoutList.addEventListener("dragend", layoutDragEnd, false);
        function handleMouseDown(ev) {
            const target = ev.target;
            target.parentElement.parentElement.setAttribute("draggable", "true");
        }
        function handleMouseUp(ev) {
            const target = ev.target;
            target.parentElement.parentElement.setAttribute("draggable", "false");
        }
        function layoutDragStart(ev) {
            ev.dataTransfer.effectAllowed = "move";
            let target = ev.target;
            if (target instanceof HTMLLIElement) {
                ev.dataTransfer.setData("text", target.getAttribute(dataId));
            }
        }
        function layoutDragEnter(ev) {
            if (ev.preventDefault) {
                ev.preventDefault();
            }
            if (ev.target instanceof HTMLUListElement) {
                return true;
            }
            else if (ev.target instanceof HTMLLIElement) {
                ev.target.attributes["style"] = "border-bottom: 1px dashed #F00";
                return true;
            }
            return false;
        }
        function layoutDragLeave(ev) {
            if (ev.preventDefault) {
                ev.preventDefault();
            }
            if (ev.target instanceof HTMLUListElement) {
                return true;
            }
            else if (ev.target instanceof HTMLLIElement) {
                ev.target.attributes["style"] = "";
                return true;
            }
            return false;
        }
        function layoutDragOver(ev, dialog) {
            if (ev.preventDefault) {
                ev.preventDefault();
            }
            ev.dataTransfer.dropEffect = "move";
            const target = ev.target;
            const id = ev.dataTransfer.getData("text");
            // TODO: fix "layout-item"
            const elem = document.querySelector(".layout-item[data-id='" + id + "']");
            if (elem && isLayoutListItem(target)) {
                if (isBefore(elem, target)) {
                    elem.insertBefore(elem, target);
                }
                else {
                    elem.insertBefore(elem, target.nextSibling);
                }
                target.classList.remove("over");
            }
            return false;
        }
        function layoutDrop(ev) {
            if (ev.preventDefault) {
                ev.preventDefault();
            }
            const id = ev.dataTransfer.getData("text");
            const elem = document.querySelector(".layout-item[data-id='" + id + "']");
            let target = ev.target;
            while (!isLayoutListItem(target)) {
                target = target.parentNode;
            }
            if (elem && isLayoutListItem(target)) {
                const targetListItem = target.parentNode;
                if (isBefore(elem, target)) {
                    targetListItem.insertBefore(elem, target);
                }
                else {
                    targetListItem.insertBefore(elem, target.nextSibling);
                }
                target.classList.remove("over");
            }
        }
        function layoutDragEnd(ev) {
            const target = ev.target;
            if (target) {
                target.setAttribute("draggable", "false");
            }
        }
    }
}
