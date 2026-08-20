import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-empty-state',
  imports: [MatIconModule],
  template: `
    <div class="flex flex-col items-center justify-center py-6 text-slate-500">
      <mat-icon class="!w-12 !h-12 !text-3xl text-slate-300 mb-2">search_off</mat-icon>
      <p class="text-sm font-medium">{{ message }}</p>
    </div>
  `,
})
export class EmptyState {
  @Input() message = 'Não encontramos nenhum resultado.';
}
