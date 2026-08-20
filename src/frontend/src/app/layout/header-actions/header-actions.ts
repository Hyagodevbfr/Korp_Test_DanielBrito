import { Component } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-header-actions',
  imports: [MatButton, RouterLink],
  template: `
    <div class="flex items-center gap-2">
      <button matButton routerLink="/products">
        Produtos
      </button>
      <button matButton routerLink="/invoices">
        Notas Fiscais
      </button>
    </div>
  `,
  styles: ``,
})
export class HeaderActions {}
