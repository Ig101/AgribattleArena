import { Random } from '../random/random';
import { BiomEnum } from '../models/enum/biom.enum';
import { asciiBioms } from './ascii-bioms.natives';

export function getRandomBiom(random: Random, biomEnum: BiomEnum) {
  const biom = asciiBioms[biomEnum];
  const probability = biom.reduce((a, b) => a as number + b.probability || 0, 0);
  const value = random.next(0, probability);
  let i = 0;
  let sumProbability = 0;
  while (biom[i].probability + sumProbability < value) {
    sumProbability += biom[i].probability;
    if (!biom[i + 1]) {
      break;
    }
    i++;
  }
  return biom[i];
}
