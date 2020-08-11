export interface SmartAction {
  hotKey: string;
  keyVisualization: string;
  title: string;
  pressed: boolean;
  action: () => void;
}
