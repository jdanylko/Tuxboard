export class WidgetToolbarButton {
    constructor(parent, selector = null) {
        this.parent = parent;
        this.selector = selector;
    }
    getDom() { return this.parent.getDom().querySelector(this.selector); }
    setName(name) { this.name = name; }
    getName() { return name; }
    getSelector() { return this.selector; }
}
