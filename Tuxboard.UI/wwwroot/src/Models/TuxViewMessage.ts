import {TuxMessageType} from "./TuxMessageType";

export class TuxViewMessage {
    constructor(
        public id: string,
        public success: boolean,
        public text: string,
        public type: TuxMessageType
    ) {}
}