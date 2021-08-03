import { ITuxbarControl } from "./ITuxbarControl";
import { Tuxbar } from "./Tuxbar";

export class TuxbarButton implements ITuxbarControl {

    constructor(
        protected readonly tuxBar: Tuxbar,
        protected selector: string) {
    }
}
