import { Component } from '@angular/core';
import { MatToolbar } from '@angular/material/toolbar';
import { HeaderActions } from '../header-actions/header-actions';

@Component({
  selector: 'app-header',
  imports: [MatToolbar, HeaderActions],
  template: `
    <header class="w-full">
      <mat-toolbar class="w-full elevated brand-toolbar py-2">
        <div class="max-w-[1200px] mx-auto w-full flex items-center justify-between">
          <span class="brand-title">Korp ERP</span>

          <app-header-actions />
        </div>
      </mat-toolbar>
      <div class="brand-accent-bar"></div>
    </header>
  `,
  styles: [`
    @use '@angular/material' as mat;

    .brand-toolbar {
      // container-text-color também é o que os botões do Material dentro do toolbar
      // (ver header-actions.ts) usam para a cor do próprio texto/estado de hover.
      @include mat.toolbar-overrides((
        container-background-color: #204552,
        container-text-color: #ffffff,
      ));
    }

    .brand-title {
      font-weight: 600;
      letter-spacing: 0.02em;
    }

    .brand-accent-bar {
      height: 3px;
      background: linear-gradient(90deg, #204552 0%, #58253b 55%, #e4004b 100%);
    }
  `],
})
export class Header {}
