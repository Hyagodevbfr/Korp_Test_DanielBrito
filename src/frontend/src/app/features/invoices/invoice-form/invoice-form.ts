import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { finalize } from 'rxjs';

import { Product } from '../../../core/models/product.model';
import { Invoice, CreateInvoiceRequest } from '../../../core/models/invoice.model';
import { InvoiceService } from '../../../core/services/invoice.service';
import { ProductService } from '../../../core/services/product.service';
import { Toaster } from '../../toaster';
import { InvoiceItemRow } from './invoice-item-row/invoice-item-row';

export interface InvoiceDialogData {
  title: string;
  invoice?: Invoice;
}

@Component({
  selector: 'app-invoice-form',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    InvoiceItemRow,
  ],
  template: `
    <div class="px-6 pt-5 pb-3 border-b border-slate-100 flex items-center justify-between">
      <h2 class="text-xl font-bold text-slate-800 tracking-tight m-0">
        {{ data.title || 'Nota Fiscal' }}
      </h2>
      <button matIconButton (click)="onCancel()" class="text-slate-400 hover:text-slate-600">
        <mat-icon>close</mat-icon>
      </button>
    </div>

    <mat-dialog-content class="!px-6 !py-4 !max-h-[70vh] overflow-x-hidden">
      <form [formGroup]="form" class="flex flex-col gap-4">

        <div class="flex items-center justify-between pb-1 border-b border-slate-200/60">
          <div class="flex items-center gap-2">
            <span class="text-sm font-semibold text-slate-700">Itens Adicionados</span>
            <span class="px-2 py-0.5 rounded-full bg-slate-100 text-slate-600 text-xs font-medium border border-slate-200">
              {{ items.length }}
            </span>
          </div>

          <button type="button" matButton color="primary" (click)="addItem()" [disabled]="isLoading">
            <mat-icon>add</mat-icon>
            Adicionar Item
          </button>
        </div>

        <div class="flex flex-col gap-3 max-h-[380px] overflow-y-auto pr-1 pt-1">
          <app-invoice-item-row
            *ngFor="let item of itemGroups; let i = index"
            [group]="item"
            [index]="i"
            [products]="products"
            [disabled]="isLoading"
            [removable]="items.length > 1"
            (remove)="removeItem(i)">
          </app-invoice-item-row>
        </div>
      </form>
    </mat-dialog-content>

    <div class="px-6 py-4 border-t border-slate-100 flex items-center justify-end gap-2 bg-slate-50/50">
      <button matButton type="button" (click)="onCancel()" [disabled]="isLoading">
        Cancelar
      </button>

      <button matButton="filled" color="primary" (click)="onSubmit()" [disabled]="form.invalid || isLoading">
        <mat-spinner *ngIf="isLoading" diameter="18" class="mr-2 inline-block"></mat-spinner>
        <mat-icon *ngIf="!isLoading">save</mat-icon>
        <span>{{ isLoading ? 'Salvando nota fiscal...' : 'Criar Nota Fiscal' }}</span>
      </button>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      width: 100%;
      box-sizing: border-box;
      overflow-x: hidden;
    }
  `],
})
export class InvoiceForm implements OnInit {
  form!: FormGroup;
  isLoading = false;
  products: Product[] = [];

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<InvoiceForm>,
    private invoiceService: InvoiceService,
    private productService: ProductService,
    private toast: Toaster,
    @Inject(MAT_DIALOG_DATA) public data: InvoiceDialogData
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadProducts();
  }

  get items(): FormArray {
    return this.form.get('items') as FormArray;
  }

  get itemGroups(): FormGroup[] {
    return this.items.controls as FormGroup[];
  }

  private initForm(): void {
    this.form = this.fb.group({
      items: this.fb.array([], [Validators.required, Validators.minLength(1)]),
    });

    this.addItem();
  }

  private loadProducts(): void {
    this.productService.getAll().subscribe({
      next: (products) => (this.products = products),
      error: () => {},
    });
  }

  private createItemFormGroup(): FormGroup {
    return this.fb.group({
      productId: [null, [Validators.required]],
      quantity: [1, [Validators.required, Validators.min(1)]],
    });
  }

  addItem(): void {
    this.items.push(this.createItemFormGroup());
  }

  removeItem(index: number): void {
    if (this.items.length > 1) {
      this.items.removeAt(index);
    } else {
      this.toast.warning('A fatura precisa ter ao menos 1 item.');
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const payload: CreateInvoiceRequest = this.form.getRawValue();

    this.invoiceService.create(payload)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe(() => {
        this.toast.success('Fatura cadastrada com sucesso!');
        this.dialogRef.close(true);
      });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
