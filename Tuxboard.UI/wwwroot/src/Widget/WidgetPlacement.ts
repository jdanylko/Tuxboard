import { dataId, isStaticAttribute, clearNodes, collapsedToggleSelector } from "../core/common";
import { WidgetSettings } from "./WidgetSettings";
import { WidgetToolBar } from "./WidgetToolbar/WidgetToolBar";
import { TuxboardService } from "../Services/TuxboardService";
import { WidgetProperties} from "../Models/WidgetProperties";

export class WidgetPlacement {

    private widgetSelector: string = ".card";
    private widgetTitleSelector: string = ".card-title";
    private widgetBodySelector: string = ".card-body";

    private generalOverlaySelector: string = ".overlay";
    private loadingSelector: string = ".loading-status";
    private widgetOverlaySelector: string = this.generalOverlaySelector + this.loadingSelector;
    
    // Comment out for no toolbar
    private toolbar: WidgetToolBar;

    private settings: WidgetSettings;
    private service: TuxboardService = new TuxboardService();

    private placementId: string;
    private index: number;
    private columnIndex: number;

    constructor(
        private readonly parent: HTMLElement,
        selector: string = null
    ) {
        this.widgetSelector = selector || this.widgetSelector;
        this.settings = new WidgetSettings(this);
    }

    isCollapsed() {
        return this.getDom().classList.contains(collapsedToggleSelector);
    }

    isStatic() {
        return this.getDom().getAttribute(isStaticAttribute) === "true";
    }

    getDom():HTMLElement { return this.parent.querySelector(this.getSelector()) }
    getAttributeName() { return dataId }
    setPlacementId(value: string) { this.placementId = value }
    getPlacementId():string { return this.placementId }
    setIndex(value: number) { this.index = value }
    getIndex() { return this.index }
    setColumnIndex(value: number) { this.columnIndex = value }
    getColumnIndex() { return this.columnIndex }

    getSelector() { return this.getSelectorWithId(this.getPlacementId()); }

    getSelectorWithId(placementId: string) {
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

    getWidgetBodySelector() { return this.widgetBodySelector; }
    setWidgetBodySelector(value: string) { this.widgetBodySelector = value; }

    getWidgetOverlay() { return this.widgetOverlaySelector; }
    setWidgetOverlay(value: string) { this.widgetOverlaySelector = value; }

    setBody(html: string) {
        const widget = this.getDom();
        if (widget) {
            const modalBody = widget.querySelector(this.widgetBodySelector);
            if (modalBody) {
                clearNodes(modalBody);
                modalBody.insertAdjacentHTML('afterbegin', html);
            }
        }
    }

    setTitle(title: string) {
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
            this.toolbar = new WidgetToolBar(this);
        }
    }

    async update() {

        if (this.isStatic())
            return;

        this.showOverlay();

        await this.service.getWidgetService(this.placementId)
            .then((data: string) => {
                this.setBody(data);

                if (!this.isCollapsed()) {
                    this.showBody();
                }
                this.hideOverlay();

                this.updateWidgetToolbar();
            });
    }

    getProperties() {
        return new WidgetProperties(
            this.placementId,
            this.getColumnIndex(),
            this.getIndex(),
            this.parent.getAttribute(dataId)
        );
    }

}