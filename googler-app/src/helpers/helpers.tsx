export default function rgbaToHexString(r: number, g: number, b: number, a: number) {
    let rs: string = r.toString(16);
    let gs: string = g.toString(16);
    let bs: string = b.toString(16);
    let as: string = Math.round(a * 255).toString(16);

    if (rs.length === 1)
        rs = "0" + r;
    if (gs.length === 1)
        gs = "0" + g;
    if (bs.length === 1)
        bs = "0" + b;
    if (as.length === 1)
        as = "0" + a;

    return "#" + rs + gs + bs + as;
}