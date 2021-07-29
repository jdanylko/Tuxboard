import { Tuxboard } from "../src/Tuxboard"

describe("create a tuxboard", function () {
    const tb = new Tuxboard();

    it("instantiated", function () {

        expect(tb).toNotBe(undefined);
    });
});