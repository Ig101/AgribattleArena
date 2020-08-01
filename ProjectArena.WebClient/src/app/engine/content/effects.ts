export const move = 1;
export const physicalDamage = 2;

export type Effect =
  typeof move |
  typeof physicalDamage;
