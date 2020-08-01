export const effectMove = 1;
export const effectPhysicalDamage = 2;

export type Effect =
  typeof effectMove |
  typeof effectPhysicalDamage;
