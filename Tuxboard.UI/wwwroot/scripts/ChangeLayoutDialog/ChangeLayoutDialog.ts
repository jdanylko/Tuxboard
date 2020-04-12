import { BaseDialog } from "../BaseDialog";
import { Tab } from "../Tab";

export class ChangeLayoutDialog extends BaseDialog {

    public static tuxLayoutDialog: string = "layout-dialog"; // default is #layout-dialog
    public static tuxSaveLayoutButton: string = ".save-layout"; // save layout button.
    public static tuxLayoutDeleteButton: string = ".layout-delete-button";
    public static tuxLayoutList: string = ".layout-list";
    public static tuxLayoutItem: string = "layout-item";
    public static tuxLayoutTypes: string = ".layout-types a";
    public static tuxLayoutListHandle: string = ".handle";
    public static tuxLayoutMessage: string = "#layout-message";

    private layoutDragEl: HTMLElement;

    constructor() { super(); }

    /* Common: Utility */
    getLayoutOverlay() { return this.getLayoutDialog().querySelector<HTMLDivElement>(this.tuxOverlay); }
    getLayoutDialog() { return document.getElementById(ChangeLayoutDialog.tuxLayoutDialog); }
    getLayoutList() { return this.getLayoutDialog().querySelector(ChangeLayoutDialog.tuxLayoutList); }
    getLayoutListItems() { return this.getLayoutList().children; }
    getClosestByTag(ev: Event, targetTagName: string) {
        let current = ev.target as HTMLElement;
        if (current.tagName.toLowerCase() === targetTagName.toLowerCase())
            return current;

        while (current.parentNode !== null && current.tagName.toLowerCase() !== targetTagName.toLowerCase()) {

            current = current.parentNode as HTMLElement;
        }
        return current;
    }

    getSaveLayoutButton() {
        const layoutDialog = this.getLayoutDialog();
        return layoutDialog.querySelector<HTMLElement>(ChangeLayoutDialog.tuxSaveLayoutButton);
    }

    setLayoutDialog(body) {
        const modalBody = this.getLayoutDialog().querySelector<HTMLElement>(this.modalBody);
        modalBody.innerHTML = body;
    }

    initLayoutDialog(layoutBody) {

        const overlay = this.getLayoutOverlay() as HTMLDivElement;

        this.setLayoutDialog(layoutBody);

        // re-attach events for new Bootstrap elements.
        // from https://github.com/thednp/bootstrap.native/wiki/FAQs
        new Dropdown(document.getElementById(ChangeLayoutDialog.tuxLayoutDialogDropdown));

        this.initLayoutDragAndDrop();

        this.attachLayoutEvents();

        this.resetColumnStatus();

        overlay.setAttribute("style", "display:none");
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

    saveCurrentLayout() {

        this.layoutDragEl = null;

        const layoutData = [];

        [].forEach.call(this.getLayoutListItems(),
            (liItem, index) => {
                const rowTypeId = liItem.attributes["data-row-type"].value;
                let id = liItem.attributes["data-id"].value;
                if (!id) {
                    id = "0";
                }
                layoutData.push(
                    {
                        Index: index, // zero-based
                        LayoutRowId: id,
                        TypeId: rowTypeId
                    });
            });

        const postData = {
            LayoutList: layoutData,
            TabId: getCurrentTab().value
        };

        this.saveLayoutService(postData);
    }

    attachLayoutEvents() {
        const layoutDialog = this.getLayoutDialog();

        const saveLayoutButton = this.getSaveLayoutButton();
        saveLayoutButton.addEventListener("click", this.saveCurrentLayout, false);

        const links = layoutDialog.querySelectorAll(ChangeLayoutDialog.tuxLayoutTypes);
        [].forEach.call(links,
            function (el) {
                el.addEventListener("click", this.addLayoutRow);
            });

        const deleteButtons = layoutDialog.querySelectorAll(ChangeLayoutDialog.tuxLayoutDeleteButton);
        [].forEach.call(deleteButtons,
            function (el) {
                el.addEventListener("click", this.layoutDeleteButtonClick);
            });
    }

    addLayoutRow(ev) {
        const layoutTypeId = ev.target.attributes["data-id"].value;
        this.addLayoutRowOnLayoutDialogService(layoutTypeId);
    }

    updateLayoutData(html) {
        this.resetColumnStatus();
        const columnElement = this.getLayoutList();
        columnElement.insertAdjacentHTML("beforeend", html);

        this.initLayoutDragAndDrop();

        this.attachLayoutEvents();
    }

    resetColumnStatus() {
        const dialog = this.getLayoutDialog();
        const liList = this.getLayoutListItems();
        [].forEach.call(liList,
            function (liItem) {
                liItem.style = "";
            });
        const span = dialog.querySelector(ChangeLayoutDialog.tuxLayoutMessage);
        if (span) {
            span.innerHTML = "";
        }
    }

    setColumnStatus(id, data) {
        const dialog = this.getLayoutDialog();
        const item = dialog.querySelector(`li[data-id='${id}']`);
        const span = dialog.querySelector(ChangeLayoutDialog.tuxLayoutMessage);
        span.innerHTML = data.text;
        item.setAttribute("style", "outline: 1px solid #F00");
    }

    layoutDeleteButtonClick(ev) {
        const item = this.getClosestByTag(ev, "li");
        if (item) {
            this.deleteRowFromLayoutDialogService(item);
        }
    }

    initLayoutDragAndDrop() {
        const layoutList = this.getLayoutList();
        const liList = this.getLayoutListItems();

        [].forEach.call(liList,
            function (liItem) {
                const handle = liItem.querySelector(this.tuxLayoutListHandle);
                handle.addEventListener("mousedown", handleMouseDown, false);
                handle.addEventListener("mouseup", handleMouseUp, false);
            });

        layoutList.addEventListener('dragstart', layoutDragStart, false);
        layoutList.addEventListener("dragover", layoutDragover, false);
        layoutList.addEventListener("dragend", layoutDragEnd, false);

        function handleMouseDown(ev) {
            ev.target.parentNode.parentNode.setAttribute("draggable", "true");
        }

        function handleMouseUp(ev) {
            ev.target.parentNode.parentNode.setAttribute("draggable", "false");
        }

        function layoutDragStart(ev) {

            if (ev.stopPropagation) ev.stopPropagation();

            ev.dataTransfer.effectAllowed = 'move';

            this.layoutDragEl = ev.target;

            ev.dataTransfer.setData('text/html', null);

        }

        function layoutDragover(ev) {
            if (ev.preventDefault) ev.preventDefault();
            if (ev.stopPropagation) ev.stopPropagation();

            ev.dataTransfer.dropEffect = 'move';
            if (this.isTargetListItem(ev)) {

                if (this.isBefore(this.layoutDragEl, ev.target)) {
                    ev.target.parentNode.insertBefore(this.layoutDragEl, ev.target);
                } else {
                    ev.target.parentNode.insertBefore(this.layoutDragEl, ev.target.nextSibling);
                }

                this.layoutDragEl.classList.remove("over");
            }

            return false;
        }

        function layoutDragEnd(ev) {
            ev.target.setAttribute("draggable", "false");
            this.layoutDragEl = null;
        }
    }

}