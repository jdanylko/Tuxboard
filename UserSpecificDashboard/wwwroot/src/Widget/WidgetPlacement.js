"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.WidgetPlacement = void 0;
const common_1 = require("../core/common");
const TuxboardService_1 = require("../Services/TuxboardService");
const WidgetProperties_1 = require("../Models/WidgetProperties");
const WidgetSettings_1 = require("./WidgetSettings");
const WidgetToolBar_1 = require("./WidgetToolbar/WidgetToolBar");
class WidgetPlacement {
    constructor(parent, selector = null) {
        this.parent = parent;
        this.widgetSelector = ".card";
        this.widgetTitleSelector = ".card-title";
        this.widgetBodySelector = ".card-body";
        this.generalOverlaySelector = ".overlay";
        this.loadingSelector = ".loading-status";
        this.widgetOverlaySelector = this.generalOverlaySelector + this.loadingSelector;
        this.service = new TuxboardService_1.TuxboardService();
        this.widgetSelector = selector || this.widgetSelector;
        this.settings = new WidgetSettings_1.WidgetSettings(this);
    }
    isCollapsed() {
        return this.getDom().classList.contains(common_1.collapsedToggleSelector);
    }
    isStatic() {
        return this.getDom().getAttribute(common_1.isStaticAttribute) === "true";
    }
    getDom() {
        return this.parent.querySelector(this.getSelector());
    }
    getAttributeName() {
        return common_1.dataId;
    }
    setPlacementId(value) {
        this.placementId = value;
    }
    getPlacementId() {
        return this.placementId;
    }
    setIndex(value) {
        this.index = value;
    }
    getIndex() {
        return this.index;
    }
    setColumnIndex(value) {
        this.columnIndex = value;
    }
    getColumnIndex() {
        return this.columnIndex;
    }
    getSelector() {
        return this.getSelectorWithId(this.getPlacementId());
    }
    getSelectorWithId(placementId) {
        // ".card[data-id='blahblahblah']"
        return `${this.widgetSelector}[${this.getAttributeName()}='${placementId}']`;
    }
    getBody() {
        return this.getDom().querySelector(this.widgetBodySelector);
    }
    hideBody() {
        const body = this.getBody();
        if (body && body.getAttribute('hidden') === null) {
            body.setAttribute('hidden', '');
        }
    }
    showBody() {
        const body = this.getBody();
        if (body && body.getAttribute('hidden') !== null) {
            body.removeAttribute('hidden');
        }
    }
    getOverlay() {
        return this.getDom().querySelector(this.widgetOverlaySelector);
    }
    showOverlay() {
        const overlay = this.getOverlay();
        if (overlay && overlay.getAttribute('hidden') !== null) {
            overlay.removeAttribute('hidden');
        }
    }
    hideOverlay() {
        const overlay = this.getOverlay();
        if (overlay && overlay.getAttribute('hidden') === null) {
            overlay.setAttribute('hidden', '');
        }
    }
    getWidgetBodySelector() {
        return this.widgetBodySelector;
    }
    setWidgetBodySelector(value) {
        this.widgetBodySelector = value;
    }
    getWidgetOverlay() {
        return this.widgetOverlaySelector;
    }
    setWidgetOverlay(value) {
        this.widgetOverlaySelector = value;
    }
    setBody(html) {
        const widget = this.getDom();
        if (widget) {
            const modalBody = widget.querySelector(this.widgetBodySelector);
            if (modalBody) {
                common_1.clearNodes(modalBody);
                modalBody.insertAdjacentHTML('afterbegin', html);
            }
        }
    }
    setTitle(title) {
        const widgetTitle = this.getDom().querySelector(this.widgetTitleSelector);
        if (widgetTitle) {
            widgetTitle.innerHTML = title;
        }
    }
    showWidgetSettings() {
        this.settings.displaySettings();
    }
    updateWidgetToolbar() {
        if (typeof this.toolbar != "object") {
            this.toolbar = new WidgetToolBar_1.WidgetToolBar(this);
        }
    }
    update() {
        if (this.isStatic()) {
            return;
        }
        this.showOverlay();
        this.service.getWidgetService(this.placementId)
            .then((data) => {
            this.setBody(data);
            if (!this.isCollapsed()) {
                this.showBody();
            }
            this.hideOverlay();
            this.updateWidgetToolbar();
        });
    }
    getProperties() {
        return new WidgetProperties_1.WidgetProperties(this.placementId, this.getColumnIndex(), this.getIndex(), this.parent.getAttribute(common_1.dataId));
    }
}
exports.WidgetPlacement = WidgetPlacement;
//# sourceMappingURL=WidgetPlacement.js.map