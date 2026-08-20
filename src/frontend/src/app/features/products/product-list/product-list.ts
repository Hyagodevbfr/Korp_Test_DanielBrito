import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';

import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../core/models/product.model';
import { ProductForm } from '../product-form/product-form';
import { EmptyState } from '../../../shared/components/empty-state/empty-state';

@Component({
  selector: 'app-product-list',
  imports: [
    CommonModule,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    EmptyState,
  ],
  template: `
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
      <div>
        <h1 class="text-2xl font-bold text-slate-800 tracking-tight">Produtos</h1>
        <p class="text-sm text-slate-500">Cadastre produtos e acompanhe o saldo em estoque</p>
      </div>
      <button matButton="filled" color="primary" class="!py-5" (click)="openAddDialog()">
        <mat-icon>add</mat-icon> Novo Produto
      </button>
    </div>

    <div class="bg-white rounded-xl border border-slate-200/80 shadow-sm overflow-hidden">

      <div class="p-4 border-b border-slate-100 bg-slate-50/50 flex items-center justify-between">
        <mat-form-field appearance="outline" class="w-full max-w-md density-compact !mb-0">
          <mat-label>Buscar produtos</mat-label>
          <mat-icon matPrefix class="text-slate-400 mr-2">search</mat-icon>
          <input matInput (keyup)="applyFilter($event)" placeholder="Digite código, descrição..." #input>
        </mat-form-field>
      </div>

      <div class="overflow-x-auto">
        <table mat-table [dataSource]="dataSource" class="w-full">

          <ng-container matColumnDef="code">
            <th mat-header-cell *matHeaderCellDef class="!text-xs !font-semibold !text-slate-500 !uppercase tracking-wider"> Código </th>
            <td mat-cell *matCellDef="let element" class="!font-medium !text-slate-700">
              <span class="inline-flex items-center px-2.5 py-1 rounded-md text-xs font-mono bg-slate-100 text-slate-700 border border-slate-200">
                {{ element.code }}
              </span>
            </td>
          </ng-container>

          <ng-container matColumnDef="description">
            <th mat-header-cell *matHeaderCellDef class="!text-xs !font-semibold !text-slate-500 !uppercase tracking-wider"> Descrição </th>
            <td mat-cell *matCellDef="let element" class="!text-slate-800 !font-medium"> {{ element.description }} </td>
          </ng-container>

          <ng-container matColumnDef="balance">
            <th mat-header-cell *matHeaderCellDef class="!text-xs !font-semibold !text-slate-500 !uppercase tracking-wider"> Estoque </th>
            <td mat-cell *matCellDef="let element">
              <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-semibold">
                {{ element.balance }} un
              </span>
            </td>
          </ng-container>

          <ng-container matColumnDef="edit">
            <th mat-header-cell *matHeaderCellDef class="!text-xs !font-semibold !text-slate-500 !uppercase tracking-wider text-right"> Editar </th>
            <td mat-cell *matCellDef="let element" class="text-right">
              <button matIconButton color="primary" (click)="openEditDialog(element)">
                <mat-icon>edit</mat-icon>
              </button>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayedColumns" class="!bg-slate-50/80"></tr>
          <tr mat-row *matRowDef="let row; columns: displayedColumns;" class="hover:bg-slate-50/60 transition-colors"></tr>

          <tr class="mat-row" *matNoDataRow>
            <td class="mat-cell p-8" colspan="4">
              <app-empty-state [message]="'Não encontramos nenhum produto para “' + input.value + '”.'"></app-empty-state>
            </td>
          </tr>
        </table>
      </div>
    </div>
  `,
  styles: `
    table {
      width: 100%;
    }
  `,
})
export class ProductList implements OnInit {
  displayedColumns: string[] = ['code', 'description', 'balance', 'edit'];
  dataSource = new MatTableDataSource<Product>([]);

  constructor(
    private productService: ProductService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.productService.getAll().subscribe({
      next: (products) => (this.dataSource.data = products),
      error: () => (this.dataSource.data = []),
    });
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  openAddDialog(): void {
    const dialogRef = this.dialog.open(ProductForm, {
      width: '450px',
      data: { title: 'Cadastrar Novo Produto' },
    });

    dialogRef.afterClosed().subscribe((result: boolean) => {
      if (result) {
        this.loadProducts();
      }
    });
  }

  openEditDialog(product: Product): void {
    const dialogRef = this.dialog.open(ProductForm, {
      width: '450px',
      data: {
        title: 'Editar Produto',
        product: { ...product },
      },
    });

    dialogRef.afterClosed().subscribe((result: boolean) => {
      if (result) {
        this.loadProducts();
      }
    });
  }
}
