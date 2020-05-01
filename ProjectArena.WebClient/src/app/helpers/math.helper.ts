export function rangeBetween(x1: number, y1: number, x2: number, y2: number) {
  return Math.sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
}
export function rangeBetweenShift(sX: number, sY: number) {
  return Math.sqrt((sX * sX) + (sY * sY));
}
