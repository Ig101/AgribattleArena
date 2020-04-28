import { Visualization } from '../visualization.model';

export interface DecorationNative {
  nativeId: string;
  name: string;
  description: string;
  visualization: Visualization;
  action: Animation;
  onDeathAction: Animation;
}
