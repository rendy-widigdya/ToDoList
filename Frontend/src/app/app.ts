import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
  styleUrls: ['./app.scss'],
})
export class App {
  public readonly title = 'Welcome to the To Do List App';
  public readonly theme = signal<'light' | 'dark'>(this.getInitialTheme());

  constructor() {
    this.applyTheme(this.theme());
  }

  private getInitialTheme(): 'light' | 'dark' {
    if (typeof window === 'undefined') return 'dark';

    const stored = localStorage.getItem('theme') as 'light' | 'dark' | null;
    if (stored === 'light' || stored === 'dark') return stored;

    return window.matchMedia?.('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  }

  private applyTheme(theme: 'light' | 'dark'): void {
    try {
      document.documentElement.setAttribute('data-theme', theme);
    } catch {
      // Ignore errors in test environments
    }
  }

  public toggleTheme(): void {
    const next = this.theme() === 'dark' ? 'light' : 'dark';
    this.theme.set(next);
    this.applyTheme(next);

    try {
      localStorage.setItem('theme', next);
    } catch {
      // Ignore localStorage errors
    }
  }
}
