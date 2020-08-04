export interface ChangeDefinition {
  time: number;
  action: () => void;
}
