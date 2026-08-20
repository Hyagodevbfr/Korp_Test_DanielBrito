import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { finalize } from 'rxjs';

import { Product, CreateProductRequest, UpdateProductRequest } from '../../../core/models/product.model';
import { Toaster } from '../../toaster';
import { ProductService } from '../../../core/services/product.service';

export interface ProductDialogData {
  title: string;
  product?: Product;
}

@Component({
  selector: 'app-product-form',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  template: `
    <div class="p-6 max-w-md w-full">
      <div class="flex items-center justify-between mb-6">
        <h2 class="text-xl font-bold text-slate-800 tracking-tight">{{ data.title }}</h2>
        <button matIconButton (click)="onCancel()" class="text-slate-400 hover:text-slate-600">
          <mat-icon>close</mat-icon>
        </button>
      </div>

      <form [formGroup]="form" (ngSubmit)="onSubmit()" class="flex flex-col gap-4">

        <mat-form-field appearance="outline" class="w-full">
          <mat-label>Código</mat-label>
          <input matInput formControlName="code" placeholder="Ex: SKU-001">
          <mat-error *ngIf="form.get('code')?.hasError('required')">O código é obrigatório</mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline" class="w-full">
          <mat-label>Descrição</mat-label>
          <input matInput formControlName="description" placeholder="Ex: Caderno Grande">
          <mat-error *ngIf="form.get('description')?.hasError('required')">A descrição é obrigatória</mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline" class="w-full" [class.opacity-60]="isEditMode">
          <mat-label>Saldo Inicial</mat-label>
          <input matInput type="number" formControlName="balance" placeholder="0">
          <mat-hint *ngIf="isEditMode">O saldo não pode ser alterado por aqui.</mat-hint>
          <mat-error *ngIf="form.get('balance')?.hasError('required')">O saldo é obrigatório</mat-error>
        </mat-form-field>

        <div class="flex items-center justify-end gap-3 mt-4">
          <button type="button" matButton (click)="onCancel()" [disabled]="isLoading">Cancelar</button>
          <button type="submit" matButton="filled" color="primary" [disabled]="form.invalid || isLoading">
            <mat-spinner *ngIf="isLoading" diameter="18" class="mr-2 inline-block"></mat-spinner>
            <span>{{ isLoading ? 'Salvando...' : 'Salvar' }}</span>
          </button>
        </div>

      </form>
    </div>
  `,
  styles: ``,
})
export class ProductForm implements OnInit {
  form!: FormGroup;
  isEditMode = false;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<ProductForm>,
    private productService: ProductService,
    private toast: Toaster,
    @Inject(MAT_DIALOG_DATA) public data: ProductDialogData
  ) {}

  ngOnInit(): void {
    this.isEditMode = !!this.data.product;

    this.form = this.fb.group({
      code: [this.data.product?.code || '', [Validators.required]],
      description: [this.data.product?.description || '', [Validators.required]],
      balance: [
        { value: this.data.product?.balance ?? 0, disabled: this.isEditMode },
        [Validators.required, Validators.min(0)],
      ],
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;

    const request$ = this.isEditMode
      ? this.productService.update(this.data.product!.id, this.buildUpdateRequest())
      : this.productService.create(this.form.getRawValue() as CreateProductRequest);

    request$
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe(() => {
        const message = this.isEditMode
          ? 'Produto atualizado com sucesso!'
          : 'Produto cadastrado com sucesso!';
        this.toast.success(message);
        this.dialogRef.close(true);
      });
  }

  private buildUpdateRequest(): UpdateProductRequest {
    return {
      code: this.form.value.code,
      description: this.form.value.description,
    };
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
