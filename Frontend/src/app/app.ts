import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
  styleUrls: ['./app.scss'],
})
export class App {
  public readonly title = signal('Frontend');
  public readonly theme = signal<'light' | 'dark'>(
    (localStorage.getItem('theme') as 'light' | 'dark') ||
      (typeof window !== 'undefined' &&
      window.matchMedia &&
      window.matchMedia('(prefers-color-scheme: dark)').matches
        ? 'dark'
        : 'light')
  );

  constructor() {
    // apply initial theme
    try {
      document.documentElement.setAttribute('data-theme', this.theme());
    } catch {}
  }

  public toggleTheme(): void {
    const next = this.theme() === 'dark' ? 'light' : 'dark';
    this.theme.set(next);
    try {
      document.documentElement.setAttribute('data-theme', next);
    } catch {}
    try {
      localStorage.setItem('theme', next);
    } catch {}
  }
}
