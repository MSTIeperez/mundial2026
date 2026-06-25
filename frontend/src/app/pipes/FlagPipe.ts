import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'flag',
  standalone: true
})
export class FlagPipe implements PipeTransform {
  transform(countryCode: string): string {
    if (!countryCode || countryCode.length !== 2) {
      return '';
    }
    
    // Shift character codes to match regional indicator symbols
    return countryCode
      .toUpperCase()
      .replace(/./g, (char) => String.fromCodePoint(char.charCodeAt(0) + 127397));
  }
}