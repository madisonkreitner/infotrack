import rgbaToHexString from "./helpers"

test.each([
    { r: 255, g: 255, b: 255, a: 1, expected: "#ffffffff" },
    { r: 255, g: 255, b: 255, a: 0.5, expected: "#ffffff80" },
    { r: 255, g: 255, b: 255, a: 0, expected: "#ffffff00" },
    { r: 255, g: 255, b: 0, a: 0, expected: "#ffff0000" },
    { r: 255, g: 0, b: 255, a: 0, expected: "#ff00ff00" },
    { r: 0, g: 255, b: 255, a: 0, expected: "#00ffff00" },
])('.rgbaToHexString($r, $g, $b, $a)', ({ r, g, b, a, expected }) => {
    expect(rgbaToHexString(r, g, b, a)).toBe(expected);
});