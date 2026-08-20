import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';

import { Product } from '../../../../core/models/product.model';

@Component({
  selector: 'app-invoice-item-row',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
  ],
  template: `
    <div
      [formGroup]="group"
      class="group relative flex items-start gap-3 p-3 bg-slate-50/70 rounded-xl border border-slate-200/80 hover:border-slate-300 transition-all">

      <span class="text-xs font-bold text-slate-400 mt-3 w-4 text-center">{{ index + 1 }}</span>

      <div class="flex flex-col flex-grow gap-1">
        <div class="flex items-start gap-3">
          <mat-form-field appearance="outline" class="flex-grow min-w-0 density-compact-lg mb-0!">
            <mat-label>Produto</mat-label>
            <mat-select formControlName="productId" placeholder="Selecione um produto">
              <mat-option *ngFor="let product of products" [value]="product.id">
                <span class="font-mono text-xs text-slate-500 mr-2">[{{ product.code }}]</span>
                <span>{{ product.description }}</span>
                <span class="text-xs text-slate-400"> (saldo: {{ product.balance }})</span>
              </mat-option>
            </mat-select>
            <mat-error *ngIf="group.get('productId')?.hasError('required')">
              Obrigatório.
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline" class="w-20! shrink-0 density-compact-lg mb-0!">
            <mat-label>Qtd</mat-label>
            <input matInput type="number" formControlName="quantity" min="1">
            <mat-error *ngIf="group.get('quantity')?.hasError('required')">
              Obrigatório.
            </mat-error>
            <mat-error *ngIf="group.get('quantity')?.hasError('min')">
              Mín. 1
            </mat-error>
          </mat-form-field>
        </div>

        <div *ngIf="exceedsBalance" class="flex items-center gap-1.5 text-xs font-medium text-amber-700 rounded-md px-2 py-1">
          <span>Quantidade acima do saldo disponível ({{ productBalance }} un).</span>
        </div>
      </div>

      <button
        type="button"
        matIconButton
        color="warn"
        class="mt-1 opacity-75 group-hover:opacity-100 transition-opacity"
        (click)="remove.emit()"
        [disabled]="!removable || disabled"
        matTooltip="Remover item">
        <mat-icon>delete_outline</mat-icon>
      </button>
    </div>
  `,
})
export class InvoiceItemRow {
  @Input({ required: true }) group!: FormGroup;
  @Input({ required: true }) index = 0;
  @Input() products: Product[] = [];
  @Input() disabled = false;
  @Input() removable = true;
  @Output() remove = new EventEmitter<void>();

  private get selectedProduct(): Product | undefined {
    return this.products.find((p) => p.id === this.group.get('productId')?.value);
  }

  get productBalance(): number | null {
    return this.selectedProduct ? this.selectedProduct.balance : null;
  }

  get exceedsBalance(): boolean {
    const product = this.selectedProduct;
    const quantity = this.group.get('quantity')?.value;

    if (!product || !quantity) {
      return false;
    }

    return Number(quantity) > product.balance;
  }
}
