import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from "./layout/header/header";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header],
  template: `
    <div class="min-h-screen bg-slate-50 flex flex-col">
      <app-header class="z-10 relative shadow-sm bg-white" />
      <main class="flex-1 max-w-6xl w-full mx-auto p-6 md:p-8">
        <router-outlet />
      </main>
    </div>
  `,
  styles: [],
})
export class App {
  protected readonly title = signal('korp-frontend');
}
