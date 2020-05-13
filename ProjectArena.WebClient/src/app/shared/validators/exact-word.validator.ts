import { FormControl } from '@angular/forms';

export function exactWordValidator(controlName: string, exactWord: string) {
  return (control: FormControl) => {
    const error = control.value !== exactWord;
    return {
      maxLength: {
        result: !error,
        extendedText: $localize`:@@validators.exact-word:You have to type exactly a ${exactWord} in the ${controlName}.`
      }
    };
  };
}
