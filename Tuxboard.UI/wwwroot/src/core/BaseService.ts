export class BaseService {

    protected debug: boolean;

    constructor(debugParam: boolean = false) {
        this.debug = debugParam;
    }

    protected validateResponse(response:Response) {
        console.log(response);
        if (!response.ok) {
            const status = `${response.status} - ${response.statusText}`;
            throw Error(status);
        }
        return response;
    }

    protected readResponseAsJson<T>(response:Response) {
        console.log(response);
        return response.json();
    }

    protected readResponseAsText(response:Response) {
        console.log(response);
        return response.text();
    }

    protected logError(error:Error) {
        console.log('Issue w/ fetch call: \n', error);
    }
}