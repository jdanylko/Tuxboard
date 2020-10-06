import { Tuxbar } from "./Tuxbar";
import {ITuxbarControl} from "./ITuxbarControl";

export class TuxbarButton implements ITuxbarControl {

    constructor(protected readonly tuxBar: Tuxbar,
        protected selector: string) { }
}