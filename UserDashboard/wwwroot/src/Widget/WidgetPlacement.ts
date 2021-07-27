import { dataId, isStaticAttribute, clearNodes, collapsedToggleSelector } from "../core/common";
import { TuxboardService } from "../Services/TuxboardService";
import { WidgetProperties } from "../Models/WidgetProperties";
import { WidgetSettings } from "./WidgetSettings";
import { WidgetToolBar } from "./WidgetToolbar/WidgetToolBar";

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
    private token: string;

    constructor(
        private readonly parent: HTMLElement,
        selector: string = null
    ) {
        this.widgetSelector = selector || this.widgetSelector;
        this.settings = new WidgetSettings(this);
    }

    public isCollapsed() {
        return this.getDom().classList.contains(collapsedToggleSelector);
    }

    public isStatic() {
        return this.getDom().getAttribute(isStaticAttribute) === "true";
    }

    public getDom(): HTMLElement {
        return this.parent.querySelector(this.getSelector())
    }
    public getAttributeName() {
        return dataId
    }

    public setToken(value: string) {
        this.token = value
    }

    public getToken() {
        return this.token
    }

    public setPlacementId(value: string) {
        this.placementId = value
    }
    public getPlacementId(): string {
        return this.placementId
    }
    public setIndex(value: number) {
        this.index = value
    }
    public getIndex() {
        return this.index
    }
    public setColumnIndex(value: number) {
        this.columnIndex = value
    }
    public getColumnIndex() {
        return this.columnIndex
    }

    public getSelector() {
        return this.getSelectorWithId(this.getPlacementId());
    }

    public getSelectorWithId(placementId: string) {
        // ".card[data-id='blahblahblah']"
        return `${this.widgetSelector}[${this.getAttributeName()}='${placementId}']`;
    }

    public getBody() {
        return this.getDom().querySelector(this.widgetBodySelector);
    }

    public hideBody() {
        const body = this.getBody();
        if (body && body.getAttribute('hidden') === null) {
            body.setAttribute('hidden', '');
        }
    }

    public showBody() {
        const body = this.getBody();
        if (body && body.getAttribute('hidden') !== null) {
            body.removeAttribute('hidden');
        }
    }

    public getOverlay() {
        return this.getDom().querySelector(this.widgetOverlaySelector);
    }

    public showOverlay() {
        const overlay = this.getOverlay();
        if (overlay && overlay.getAttribute('hidden') !== null) {
            overlay.removeAttribute('hidden');
        }
    }

    public hideOverlay() {
        const overlay = this.getOverlay();
        if (overlay && overlay.getAttribute('hidden') === null) {
            overlay.setAttribute('hidden', '');
        }
    }

    public getWidgetBodySelector() {
        return this.widgetBodySelector;
    }
    public setWidgetBodySelector(value: string) {
        this.widgetBodySelector = value;
    }

    public getWidgetOverlay() {
        return this.widgetOverlaySelector;
    }
    public setWidgetOverlay(value: string) {
        this.widgetOverlaySelector = value;
    }

    public setBody(html: string) {
        const widget = this.getDom();
        if (widget) {
            const modalBody = widget.querySelector(this.widgetBodySelector);
            if (modalBody) {
                clearNodes(modalBody);
                modalBody.insertAdjacentHTML('afterbegin', html);
            }
        }
    }

    public setTitle(title: string) {
        const widgetTitle = this.getDom().querySelector(this.widgetTitleSelector);
        if (widgetTitle) {
            widgetTitle.innerHTML = title;
        }
    }

    public showWidgetSettings() {
        this.settings.displaySettings();
    }

    public updateWidgetToolbar() {
        if (typeof this.toolbar != "object") {
            this.toolbar = new WidgetToolBar(this);
        }
    }

    public update() {

        if (this.isStatic()) {
            return;
        }

        this.showOverlay();

        this.service.getWidgetService(this.placementId)
            .then((data: string) => {
                this.setBody(data);

                if (!this.isCollapsed()) {
                    this.showBody();
                }
                this.hideOverlay();

                this.updateWidgetToolbar();
            });
    }

    public getProperties() {
        return new WidgetProperties(
            this.placementId,
            this.getColumnIndex(),
            this.getIndex(),
            this.parent.getAttribute(dataId)
        );
    }
}
