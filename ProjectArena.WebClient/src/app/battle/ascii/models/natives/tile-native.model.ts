import { Visualization } from '../visualization.model';

export interface TileNative {
  nativeId: string;
  name: string;
  description: string;
  visualization: Visualization;
  backgroundColor: { r: number, g: number, b: number };
  bright: boolean;
  action: Animation;
  onStepAction: Animation;
}
