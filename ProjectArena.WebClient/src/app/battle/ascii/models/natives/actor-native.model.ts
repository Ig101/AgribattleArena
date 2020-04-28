import { Visualization } from '../visualization.model';

export interface ActorNative {
  nativeId: string;
  name: string;
  description: string;
  visualization: Visualization;
}
