(function () {

    const debug = false;

    const tuxDashboardColumn = ".column";
    const tuxDashboardTab = ".dashboard-tab";
    const tuxRowTemplate = ".row-template";

    const tuxWidgetClass = "card";
    const tuxWidgetTitle = ".card-title";

    const tuxLayoutButton = "layout-button"; // button to trigger the layout dialog.
    const tuxWidgetButton = "widget-button"; // button to trigger the widget dialog.

    const cssModalBody = ".modal-body"; // for Bootstrap, the class to replace HTML in a dialog box.

    /* Layout Dialog */
    const tuxLayoutDialog = "layout-dialog"; // default is #layout-dialog
    const tuxSaveLayoutButton = ".save-layout"; // save layout button.
    const tuxLayoutDeleteButton = ".layout-delete-button";
    const tuxLayoutList = ".layout-list";
    const tuxLayoutItem = "layout-item";
    const tuxLayoutTypes = ".layout-types a";
    const tuxLayoutListHandle = ".handle";
    const tuxLayoutMessage = "#layout-message";

    /* Widget Dialog */
    const tuxWidgetDialog = "widget-dialog"; // default is #widget-dialog
    const tuxWidgetTabGroup = ".widget-tabs"; // left-side group in #widget-dialog
    const tuxWidgetListItem = "a.widget-item"; // each widget on right in #widget-dialog
    const tuxWidgetAdd = ".add-widget"; // Add Widget button
    const tuxWidgetTools = "card-tools"; // buttons on each widget

    const tuxOverlay = ".overlay";
    const tuxWidgetOverlay = ".overlay.loading-status"; // overlay on each dialog box/widget for loading

    const tuxLayoutDialogDropdown = "btnLayoutDropdown"; // Layout dropdown button
    const tuxWidgetToolDropdown = "dropdown-card-tool"; // Widget's tool dropdown button
    const tuxWidgetSelection = ".selected"; // class added to a widget when selected

    /* Widget Settings */
    const tuxWidgetSettings = ".widget-settings"; // classname
    const tuxWidgetSettingsCancel = ".settings-cancel"; // cancel button
    const tuxWidgetSettingsSave = ".settings-save"; // save button
    const tuxWidgetInputs = ".setting-value"; // input classes

    /* Service Urls */
    const tuxLayoutDialogUrl = "/layoutdialog/";
    const tuxLayoutAddRowUrl = "/layoutdialog/addlayoutrow/";
    const tuxSaveLayoutUrl = "/layoutdialog/saveLayout/";
    const tuxDeleteLayoutRowUrl = "/layoutdialog/DeleteLayoutRow/";
    
    const tuxWidgetDialogUrl = "/widgetdialog/";
    const tuxWidgetSettingsUrl = "/widgetsettings/";
    const tuxWidgetAddWidgetUrl = "/widgetdialog/addwidget/";

    // API calls
    const tuxRefreshTuxboardUrl = "/Tuxboard/Get/";
    const tuxToolCollapseUrl = "/Tuxboard/PostCollapse/";
    const tuxWidgetPlacementUrl = "/Tuxboard/Put/";
    const tuxWidgetRemoveWidgetUrl = "/Tuxboard/removewidget/";
    const tuxWidgetContentUrl = "/Widget/";
    const tuxWidgetSaveSettingsUrl = "/Widget/savesettings/";


    /* Widget header toolbar */
    const tuxWidgetToolCollapse = "collapse-widget";
    const tuxWidgetToolRemove = "remove-widget";
    const tuxWidgetCollapsed = "collapsed";

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
    function getDataId(elem) { return elem.getAttribute("data-id") }
    function getDomWidget(id) { return document.querySelector(`[data-id='${id}']`); }
    function getWidgetSettings(widget) { return widget.querySelector(tuxWidgetSettings) }
    function getPlacementId(widget) { return getDataId(widget); }
    function getCurrentTab() { return document.querySelector(tuxDashboardTab + "[data-active='true']").attributes["data-id"]; }
    function getDashboard() { return document.getElementsByClassName("dashboard").item(0); }
    function getDashboardColumns() { return getDashboard().getElementsByClassName(noPeriod(tuxDashboardColumn)); }
    function getCurrentLayoutId(ev) { var layoutRow = getParentByClass(ev, noPeriod(tuxRowTemplate)); return getDataId(layoutRow); }
    function getColumnIndex(column) { return parseInt(column.getAttribute("data-column")); }
    function getColumn(ev) { return getParentByClass(ev, noPeriod(tuxDashboardColumn)); }
    function getWidgetsOnDashboard() { return getDashboard().getElementsByClassName(tuxWidgetClass); }
    function getLayoutDialog() { return document.getElementById(tuxLayoutDialog); }
    function getLayoutList() { return getLayoutDialog().querySelector(tuxLayoutList); }
    function getLayoutListItems() { return getLayoutList().children; }
    function getLayoutOverlay() { return getLayoutDialog().querySelector(tuxOverlay); }
    function getWidgetTabGroups() { return getWidgetDialog().querySelector(tuxWidgetTabGroup); }
    function getWidgetDialog() { return document.getElementById(tuxWidgetDialog); }
    function getAddWidgetButton() { return getWidgetDialog().querySelector(tuxWidgetAdd); }
    function isCollapsed(widget) { return widget.classList.contains(tuxWidgetCollapsed) }
    function getWidgetList() { return getWidgetDialog().querySelectorAll(tuxWidgetListItem); }
    function getSelectedWidgets() { return getWidgetDialog().querySelectorAll(tuxWidgetListItem + tuxWidgetSelection); }
    function toId(id) { return id.startsWith("#") ? id : "#" + id; }
    function toClass(id) { return id.startsWith(".") ? id : "." + id; }
    function noPeriod(id) { return id.startsWith(".") ? id.replace(".", "") : id; }

    function getSettingValues(widget) {
        let inputs = widget.querySelectorAll(tuxWidgetInputs);
        return Array.from(inputs).map((elem, index) => {
            return {
                WidgetSettingId: getDataId(elem),
                Value: elem.value
            }
        });
    }

    function isBefore(el1, el2) {
        var cur;
        if (el2.parentNode === el1.parentNode) {
            for (cur = el1.previousSibling; cur; cur = cur.previousSibling) {
                if (cur === el2) return true;
            }
        }
        return false;
    }

    function isTargetListItem(ev) {
        return ev.target.tagName.toLowerCase() === "li"
            && ev.target.classList.contains(tuxLayoutItem);
    }

    function getWidgetProperties(ev) {
        const column = getParentByClass(ev, noPeriod(tuxDashboardColumn)),
            widgets = column.getElementsByClassName(tuxWidgetClass),
            layoutRow = getParentByClass(ev, noPeriod(tuxRowTemplate));

        return {
            PlacementId: getDataId(ev.target),
            Column: getColumnIndex(column),
            LayoutRowId: getDataId(layoutRow),
            PlacementList: Array.from(widgets)
                .map(function (elem, index) {
                    return {
                        PlacementId: getDataId(elem),
                        Index: index
                    }
                })
        };
    }

    function getSaveLayoutButton() {
        const layoutDialog = getLayoutDialog();
        return layoutDialog.querySelector(tuxSaveLayoutButton);
    }

    function getWidgetTools(widgetElem) {
        return widgetElem.getElementsByClassName(tuxWidgetTools).item(0);
    }

    function setLayoutDialog(body) {
        const modalBody = getLayoutDialog().querySelector(cssModalBody);
        modalBody.innerHTML = body;
    }

    function setWidgetDialog(body) {
        const modalBody = getWidgetDialog().querySelector(cssModalBody);
        modalBody.innerHTML = body;
    }

    function enableElement(elem) {
        elem.classList.remove("disabled");
        elem.removeAttribute("disabled");
    }

    function disableElement(elem) {
        elem.classList.add("disabled");
        elem.setAttribute("disabled", "disabled");
    }

    function getParentByClass(ev, selector) {
        let current = ev.target;
        if (current.classList.contains(selector))
            return current;

        while (current.parentNode !== null && !current.classList.contains(selector)) {

            current = current.parentNode;
        }
        return current;
    }

    function getClosestByClass(element, classToSearch) {
        if (!element) {
            return false;
        } else if (element.classList.contains(classToSearch)) {
            return element;
        } else {
            return getClosestByClass(element.parentElement, classToSearch);
        }
    }

    function getClosestByTag(ev, targetTagName) {
        let current = ev.target;
        if (current.tagName.toLowerCase() === targetTagName.toLowerCase())
            return current;

        while (current.parentNode !== null && current.tagName.toLowerCase() !== targetTagName.toLowerCase()) {

            current = current.parentNode;
        }
        return current;
    }

    function isWidget(elem) {
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

        function dragStart(ev) {

            if (ev.stopPropagation) ev.stopPropagation();

            ev.dataTransfer.effectAllowed = 'move';

            widgetDragEl = ev.target;
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

        function dragEnd(ev) {
            if (ev.preventDefault) ev.preventDefault();
            if (ev.stopPropagation) ev.stopPropagation();

            ev.target.style.opacity = "";

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

    /* Service Utilities */
    function validateResponse(response) {
        if (debug) console.log(response);
        if (!response.ok) {
            throw Error(response.statusText);
        }
        return response;
    }

    function readResponseAsJson(response) {
        if (debug) console.log(response);
        return response.json();
    }

    function readResponseAsText(response) {
        if (debug) console.log(response);
        return response.text();
    }

    function logError(error) {
        if (debug) console.log('Issue w/ fetch call: \n', error);
    }

    /* Services */

    /* Service: Save Widget Placement */
    function updateWidgetPlacementStatus(/* use data */) { }

    function saveWidgetPlacementService(ev) {

        const widgetProperties = getWidgetProperties(ev);

        const postData = {
            Column: widgetProperties.Column,
            PreviousColumn: previousColumn,
            PreviousLayout: previousLayout,
            PlacementId: widgetProperties.PlacementId,
            LayoutRowId: widgetProperties.LayoutRowId,
            PlacementList: widgetProperties.PlacementList
        };

        fetch(tuxWidgetPlacementUrl,
            {
                method: "put",
                body: JSON.stringify(postData),
                headers: {
                    "Content-Type": "application/json"
                }
            })
            .then(validateResponse)
            .then(readResponseAsJson)
            .then(updateWidgetPlacementStatus)
            .catch(logError);
    }

    /* Service: Remove Widget */
    function removeWidgetFromDashboard(data) {
        if (data.success) {
            const widget = getDashboard().querySelector("[data-id='" + data.message.id + "']");
            if (widget) {
                widget.remove();
            }
        }
    }

    function removeWidgetService(ev) {
        const button = getClosestByTag(ev, "button"),
            placementId = getDataId(button),
            tabId = getCurrentTab().value;

        const postData = {
            TabId: tabId,
            PlacementId: placementId
        };

        fetch(tuxWidgetRemoveWidgetUrl,
            {
                method: 'delete',
                body: JSON.stringify(postData),
                headers: {
                    'Content-Type': 'application/json'
                }
            })
            .then(validateResponse)
            .then(readResponseAsJson)
            .then(removeWidgetFromDashboard)
            .catch(logError);
    }

    /* Service: Update Collapsed Widget */
    function updateCollapsedStatus(/* use data */) { }

    function updateCollapsedWidgetService(widget, collapsed) {

        const postData = {
            Id: getDataId(widget),
            Collapsed: collapsed
        };

        fetch(tuxToolCollapseUrl,
            {
                method: "post",
                body: JSON.stringify(postData),
                headers: {
                    "Content-Type": "application/json"
                }
            })
            .then(validateResponse)
            .then(readResponseAsJson)
            .then(updateCollapsedStatus)
            .catch(logError);

    }

    /* Service: Get Widget */
    function updateWidgetContents(id, data) {
        const widget = getDomWidget(id);
        if (widget) {
            const body = widget.querySelector(".card-body");
            if (body) {
                body.innerHTML = data;
            }
        }
    }

    function getWidgetService(widget) {

        const id = getDataId(widget);
        if (!id) return;

        const postData = {
            WidgetPlacementId: id
        };

        showOverlay(widget);

        fetch(tuxWidgetContentUrl + getDataId(widget),
                {
                    method: "get"
                })
            .then(validateResponse)
            .then(readResponseAsText)
            .then(function(data) {
                if (debug) console.log(data);
                updateWidgetContents(postData.WidgetPlacementId, data);
                hideOverlay(widget);
            })
            .catch(logError);
    }

    /* Service: Widgets Dialog Box */
    function displayWidgetDialog(data) {
        const widgetDialog = document.getElementById(tuxWidgetDialog),
            overlay = widgetDialog.querySelector(tuxOverlay);

        setWidgetDialog(data);

        setWidgetEvents();

        overlay.style = "display:none";
    }

    function getWidgetDialogService() {
        fetch(tuxWidgetDialogUrl,
            {
                method: "get"
            })
            .then(validateResponse)
            .then(readResponseAsText)
            .then(displayWidgetDialog)
            .catch(logError);
    }

    /* Service: Add Widget */
    function addWidgetToDashboard(data) {
        if (data.success) {
            if (widgetDialogInstance) {
                widgetDialogInstance.hide();
            }
            refreshTuxboardService();
        }
    }

    function addWidgetService(tabId, widgetId) {

        const postData = {
            TabId: tabId,
            WidgetId: widgetId
        };
        fetch(tuxWidgetAddWidgetUrl,
            {
                method: "post",
                body: JSON.stringify(postData),
                headers: {
                    "Content-Type": "application/json"
                }
            })
            .then(validateResponse)
            .then(readResponseAsJson)
            .then(addWidgetToDashboard)
            .catch(logError);
    }

    /* Service: Refresh Tuxboard */
    function updateTuxboard(data) {
        const dashboard = getDashboard();
        dashboard.innerHTML = data;

        initialize();
    }

    function refreshTuxboardService() {

        fetch(tuxRefreshTuxboardUrl,
            {
                method: 'get'
            })
            .then(validateResponse)
            .then(readResponseAsText)
            .then(updateTuxboard)
            .catch(logError);
    }

    /* Service: Load Layout Dialog */
    function displayLayoutDialog(data) {
        if (debug) console.log(data);
        initLayoutDialog(data);
    }

    function getLayoutDialogService() {
        fetch(tuxLayoutDialogUrl + getCurrentTab().value,
            {
                method: "post"
            })
            .then(readResponseAsText)
            .then(displayLayoutDialog)
            .catch(logError);
    }

    /* Service: Add Row */
    function addRowToLayoutDialog(data) {
        if (debug) console.log(data);
        updateLayoutData(data);
    }

    function addLayoutRowOnLayoutDialogService(typeId) {
        fetch(tuxLayoutAddRowUrl + typeId,
            {
                method: "post"
            })
            .then(readResponseAsText)
            .then(addRowToLayoutDialog)
            .catch(logError);
    }

    /* Service: Widget Settings */
    function displayWidgetSettings(widget, data) {
        if (debug) console.log(data);

        const settings = getWidgetSettings(widget);
        if (settings) {
            settings.innerHTML = data;
            hideWidgetBody(widget);
            showWidgetSettings(widget);
            attachSettingEvents(widget);
        } else {
            showWidgetBody(widget);
            hideWidgetSettings(widget);
        }

        hideOverlay(widget);
        
        // initialize();
    }

    function getWidgetSettingsService(placementId) {

        const widget = getDomWidget(placementId);
        hideWidgetBody(widget);
        showOverlay(widget);

        fetch(tuxWidgetSettingsUrl + placementId,
                {
                    method: "get"
                })
            .then(validateResponse)
            .then(readResponseAsText)
            .then(function(data) {
                if (debug) console.log(data);
                displayWidgetSettings(widget, data);
            })
            .catch(logError);
    }

    /* Service: Delete Row */
    function deleteRowFromLayoutDialogService(row) {
        const id = row.attributes["data-id"].value;
        if (id === "0") { // new, we can remove it.
            row.remove();
        } else {
            fetch(tuxDeleteLayoutRowUrl + id,
                {
                    method: 'delete'
                })
                .then(readResponseAsJson)
                .then(function (data) {
                    if (debug) console.log(data);
                    if (data.success) {
                        resetColumnStatus();
                        row.remove();
                    } else {
                        setColumnStatus(id, data);
                    }
                })
                .catch(logError);
        }
    }

    /* Service: Save Layout */
    function processSaveLayoutResponse(data) {
        if (debug) console.log(data);
        if (!data.success) {
            displayLayoutErrors(data);
        } else {
            layoutDialogInstance.hide();
            refreshTuxboardService();
        }
    }

    function saveLayoutService(bodyData) {
        fetch(tuxSaveLayoutUrl,
            {
                method: 'post',
                body: JSON.stringify(bodyData),
                headers: {
                    'Content-Type': 'application/json'
                }
            })
            .then(readResponseAsJson)
            .then(processSaveLayoutResponse)
            .catch(logError);
    }

    /* Service: Save Widget Settings */
    function saveSettingsResponse(data) {
        if (debug) console.log(data);

        const placementId = data[0].id;
        const widget = getDomWidget(placementId);

        const setting = Array.from(data).filter((elem) => elem.name === "widgettitle")[0];
        if (setting) {
            setWidgetTitle(widget, setting);
        }

        hideWidgetSettings(widget);
        if (!isCollapsed(widget)) {
            showWidgetBody(widget);
        }

        refreshWidget(widget);
    }

    function setWidgetTitle(widget, setting) {
        const widgetTitle = widget.querySelector(tuxWidgetTitle);
        if (widgetTitle) {
            widgetTitle.innerHTML = setting.value;
        }
    }

    function saveSettingsService(widget) {
        let postData = {
            Settings: getSettingValues(widget)
        }

        fetch(tuxWidgetSaveSettingsUrl,
            {
                method: 'post',
                body: JSON.stringify(postData),
                headers: {
                    'Content-Type': 'application/json'
                }
            })
            .then(readResponseAsJson)
            .then(saveSettingsResponse)
            .catch(logError);
    }

    /************************
     * Add Widget Dialog 
     *************************/
    function updateAddWidget() {
        const addWidgetButton = getAddWidgetButton(),
            selected = getSelectedWidgets();
        if (selected.length > 0) {
            enableElement(addWidgetButton);
        } else {
            disableElement(addWidgetButton);
        }
    }

    function resetSelectedWidgets() {
        const widgets = getSelectedWidgets();
        [].forEach.call(widgets,
            (item) => {
                item.classList.remove("selected");
            });
    }

    function selectWidget(ev) {
        const isSelected = ev.currentTarget.classList.contains("selected");

        resetSelectedWidgets();

        if (!isSelected) {
            ev.currentTarget.classList.add("selected");
        }

        updateAddWidget();
    }

    function setupWidgetTabs() {
        const widgetTabs = getWidgetTabGroups(),
            tabTriggers = widgetTabs.getElementsByTagName("A");

        for (let i = 0; i < tabTriggers.length; i++) {
            new Tab(tabTriggers[i], {});
        }
    }

    function addWidgetClick() {
        const widget = getSelectedWidgets();
        const widgetId = getDataId(widget[0]),
            tab = getCurrentTab();

        addWidgetService(tab.value, widgetId);
    }

    function setupWidgetClicks() {
        const widgetAddButton = getAddWidgetButton();
        widgetAddButton.addEventListener("click", addWidgetClick);

        [].forEach.call(getWidgetList(),
            (item) => {
                item.addEventListener("click", selectWidget);
            });
    }

    function setWidgetEvents() {
        setupWidgetTabs();
        setupWidgetClicks();
    }

    /************************
     * Layout Dialog 
     *************************/

    function initLayoutDialog(layoutBody) {

        const overlay = getLayoutOverlay();

        setLayoutDialog(layoutBody);

        // re-attach events for new Bootstrap elements.
        // from https://github.com/thednp/bootstrap.native/wiki/FAQs
        new Dropdown(document.getElementById(tuxLayoutDialogDropdown));

        initLayoutDragAndDrop();

        attachLayoutEvents();

        resetColumnStatus();

        overlay.style = "display:none";
    }

    function displayLayoutErrors(data) {
        const layoutDialog = getLayoutDialog();
        [].forEach.call(data.LayoutErrors,
            (item) => {
                const trow = layoutDialog.querySelector(`[data-id='${item.layoutRowId}']`);
                if (trow) {
                    trow.style = "outline: 1px solid #F00";
                } else {
                    trow.style = "";
                }
            });
    }

    function saveCurrentLayout() {
        layoutDragEl = null;
        const layoutData = [];

        [].forEach.call(getLayoutListItems(),
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

        saveLayoutService(postData);
    }

    function attachLayoutEvents() {
        const layoutDialog = getLayoutDialog();

        const saveLayoutButton = getSaveLayoutButton();
        saveLayoutButton.addEventListener("click", saveCurrentLayout, false);

        const links = layoutDialog.querySelectorAll(tuxLayoutTypes);
        [].forEach.call(links,
            function (el) {
                el.addEventListener("click", addLayoutRow);
            });

        const deleteButtons = layoutDialog.querySelectorAll(tuxLayoutDeleteButton);
        [].forEach.call(deleteButtons,
            function (el) {
                el.addEventListener("click", layoutDeleteButtonClick);
            });
    }

    function addLayoutRow(ev) {
        const layoutTypeId = ev.target.attributes["data-id"].value;
        addLayoutRowOnLayoutDialogService(layoutTypeId);
    }

    function updateLayoutData(html) {
        resetColumnStatus();
        const columnElement = getLayoutList();
        columnElement.insertAdjacentHTML("beforeend", html);

        initLayoutDragAndDrop();

        attachLayoutEvents();
    }

    function resetColumnStatus() {
        const dialog = getLayoutDialog();
        const liList = getLayoutListItems();
        [].forEach.call(liList,
            function (liItem) {
                liItem.style = "";
            });
        const span = dialog.querySelector(tuxLayoutMessage);
        if (span) {
            span.innerHTML = "";
        }
    }

    function setColumnStatus(id, data) {
        const dialog = getLayoutDialog();
        const item = dialog.querySelector(`li[data-id='${id}']`);
        const span = dialog.querySelector(tuxLayoutMessage);
        span.innerHTML = data.text;
        item.style = "outline: 1px solid #F00";
    }

    function layoutDeleteButtonClick(ev) {
        const item = getClosestByTag(ev, "li");
        if (item) {
            deleteRowFromLayoutDialogService(item);
        }
    }

    function initLayoutDragAndDrop() {
        const layoutList = getLayoutList();
        const liList = getLayoutListItems();

        [].forEach.call(liList,
            function (liItem) {
                const handle = liItem.querySelector(tuxLayoutListHandle);
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

            layoutDragEl = ev.target;

            ev.dataTransfer.setData('text/html', null);

        }

        function layoutDragover(ev) {
            if (ev.preventDefault) ev.preventDefault();
            if (ev.stopPropagation) ev.stopPropagation();

            ev.dataTransfer.dropEffect = 'move';
            if (isTargetListItem(ev)) {

                if (isBefore(layoutDragEl, ev.target)) {
                    ev.target.parentNode.insertBefore(layoutDragEl, ev.target);
                } else {
                    ev.target.parentNode.insertBefore(layoutDragEl, ev.target.nextSibling);
                }

                layoutDragEl.classList.remove("over");
            }

            return false;
        }

        function layoutDragEnd(ev) {
            ev.target.setAttribute("draggable", "false");
            layoutDragEl = null;
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

    function refreshWidget(widget) {
        getWidgetService(widget);
    }

    function refreshWidgets() {
        const widgetsOnDashboard = getWidgetsOnDashboard();
        [].forEach.call(widgetsOnDashboard,
            (elem) => {
                refreshWidget(elem);
            });
    }

    /******************************
     * Widget Setup/Settings
     */

    function setupWidgetToolbar(elem) {

        const tools = getWidgetTools(elem);
        if (!tools) return;

        const removeButton = tools.getElementsByClassName(tuxWidgetToolRemove).item(0);

        if (removeButton) {
            removeButton.addEventListener("click", removeWidgetService, false);
        }

        const minimizeButton = tools.getElementsByClassName(tuxWidgetToolCollapse).item(0);
        if (minimizeButton) {
            minimizeButton.addEventListener("click", minimizeWidget, false);
        }

        /* dropdown options */
        const settingsOption = elem.getElementsByClassName(noPeriod(tuxSettingsOption)).item(0);
        if (settingsOption) {
            settingsOption.addEventListener("click", settingsClick, false);
        }

        const refreshOption = elem.getElementsByClassName(noPeriod(tuxRefreshOption)).item(0);
        if (refreshOption) {
            refreshOption.addEventListener("click", refreshClick, false);
        }
    }

    function minimizeWidget(ev) {
        const widget = getParentByClass(ev, tuxWidgetClass);
        if (widget) {
            const minimized = widget.classList.contains(tuxWidgetCollapsed);
            if (minimized) {
                widget.classList.remove(tuxWidgetCollapsed);
                showWidgetBody(widget);
                updateCollapsedWidgetService(widget, 0);
            } else {
                widget.classList.add(tuxWidgetCollapsed);
                hideWidgetBody(widget);
                updateCollapsedWidgetService(widget, 1);
            }
        }
    }

    function attachSettingEvents(widget) {

        let saveButton = widget.querySelector(tuxWidgetSettingsSave);
        saveButton.addEventListener("click", saveSettingsClick, false);

        let cancelButton = widget.querySelector(tuxWidgetSettingsCancel);
        cancelButton.addEventListener("click", cancelSettingsClick, false);
    }

    function cancelSettingsClick(ev) {
        let widget = getParentByClass(ev, tuxWidgetClass);
        if (widget) {
            hideWidgetSettings(widget);
            showWidgetBody(widget);
        }
    }

    function saveSettingsClick(ev) {
        let widget = getParentByClass(ev, tuxWidgetClass);
        if (widget) {
            saveSettingsService(widget);
        }
    }

    function settingsClick(ev) {
        let widget = getParentByClass(ev, tuxWidgetClass);
        if (widget) {
            getWidgetSettingsService(getPlacementId(widget));
        }
    }

    function refreshClick(ev) {
        const widget = getParentByClass(ev, tuxWidgetClass);
        if (widget) {
            getWidgetService(widget);
        }
    }

    function attachWidgetEvents() {
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

    function hideWidgetBody(widget) {
        const body = widget.querySelector(".card-body");
        if (body) {
            body.setAttribute("hidden", "");
        }
    }

    function showWidgetBody(widget) {
        const body = widget.querySelector(".card-body");
        if (body) {
            body.removeAttribute("hidden");
        }
    }

    function showOverlay(widget) {
        const overlay = widget.querySelector(tuxWidgetOverlay);
        if (overlay && overlay.hasAttribute("hidden")) {
            overlay.removeAttribute("hidden");
        }
    }

    function hideOverlay(widget) {
        const overlay = widget.querySelector(tuxWidgetOverlay);
        if (overlay && !overlay.hasAttribute("hidden")) {
            overlay.setAttribute("hidden", "");
        }
    }

    function showWidgetSettings(widget) {
        const settings = widget.querySelector(tuxWidgetSettings);
        if (settings && settings.hasAttribute("hidden")) {
            settings.removeAttribute("hidden");
        }
    }

    function hideWidgetSettings(widget) {
        const settings = widget.querySelector(tuxWidgetSettings);
        if (settings && !settings.hasAttribute("hidden")) {
            settings.setAttribute("hidden", "");
        }
    }

    /***
     * Initialization
     */
    function setupDialogs() {

        // Layout - onShow event
        const layoutDialog = getLayoutDialog();
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

    function initialize() {
        initDashboardDragAndDrop();

        setupDialogs();

        attachWidgetEvents();

        refreshWidgets();
    }

    initialize();

})();
