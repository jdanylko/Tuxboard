"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.BaseService = void 0;
class BaseService {
    constructor(debugParam = false) {
        this.debug = debugParam;
    }
    validateResponse(response) {
        // console.log(response);
        if (!response.ok) {
            const status = `${response.status} - ${response.statusText}`;
            throw Error(status);
        }
        return response;
    }
    readResponseAsJson(response) {
        // console.log(response);
        return response.json();
    }
    readResponseAsText(response) {
        // console.log(response);
        return response.text();
    }
    logError(error) {
        // console.log("Issue w/ fetch call: \n", error);
    }
}
exports.BaseService = BaseService;
//# sourceMappingURL=BaseService.js.map