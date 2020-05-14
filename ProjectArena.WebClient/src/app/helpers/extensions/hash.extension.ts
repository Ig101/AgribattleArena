export function getHashFromString(value: string) {
  let hash = 0;
  let i: number;
  let chr: number;
  for (i = 0; i < value.length; i++) {
    chr   = value.charCodeAt(i);
    // tslint:disable-next-line: no-bitwise
    hash  = ((hash << 5) - hash) + chr;
    // tslint:disable-next-line: no-bitwise
    hash |= 0;
  }
  return Math.floor(hash / 2);
}
