import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { Router } from '@angular/router';

import { Invoice } from '../../../core/models/invoice.model';
import { InvoiceService } from '../../../core/services/invoice.service';
import { InvoiceForm } from '../invoice-form/invoice-form';
import { StatusChip } from '../../../shared/components/status-chip/status-chip';
import { EmptyState } from '../../../shared/components/empty-state/empty-state';
import { DateTimeBrPipe } from '../../../shared/pipes/date-time-br.pipe';

@Component({
  selector: 'app-invoice-list',
  imports: [
    CommonModule,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatDialogModule,
    StatusChip,
    EmptyState,
    DateTimeBrPipe,
  ],
  template: `
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
      <div>
        <h1 class="text-2xl font-bold text-slate-800 tracking-tight">Faturas</h1>
        <p class="text-sm text-slate-500">Consulte as notas fiscais emitidas e acompanhe o status de cada uma</p>
      </div>
      <button matButton="filled" color="primary" class="!py-5" (click)="openAddDialog()">
        <mat-icon>add</mat-icon> Nova Fatura
      </button>
    </div>

    <div class="bg-white rounded-xl border border-slate-200/80 shadow-sm overflow-hidden">

      <div class="p-4 border-b border-slate-100 bg-slate-50/50 flex items-center justify-between">
        <mat-form-field appearance="outline" class="w-full max-w-md density-compact !mb-0">
          <mat-label>Buscar faturas</mat-label>
          <mat-icon matPrefix class="text-slate-400 mr-2">search</mat-icon>
          <input matInput (keyup)="applyFilter($event)" placeholder="Digite número, status..." #input>
        </mat-form-field>
      </div>

      <div class="overflow-x-auto">
        <table mat-table [dataSource]="dataSource" class="w-full">

          <ng-container matColumnDef="number">
            <th mat-header-cell *matHeaderCellDef class="!text-xs !font-semibold !text-slate-500 !uppercase tracking-wider"> Número </th>
            <td mat-cell *matCellDef="let element" class="!font-medium !text-slate-700">
              <span class="inline-flex items-center px-2.5 py-1 rounded-md text-xs font-mono bg-slate-100 text-slate-700 border border-slate-200">
                #{{ element.number }}
              </span>
            </td>
          </ng-container>

          <ng-container matColumnDef="status">
            <th mat-header-cell *matHeaderCellDef class="!text-xs !font-semibold !text-slate-500 !uppercase tracking-wider"> Status </th>
            <td mat-cell *matCellDef="let element">
              <app-status-chip [status]="element.status"></app-status-chip>
            </td>
          </ng-container>

          <ng-container matColumnDef="createdAt">
            <th mat-header-cell *matHeaderCellDef class="!text-xs !font-semibold !text-slate-500 !uppercase tracking-wider"> Data de Criação </th>
            <td mat-cell *matCellDef="let element" class="!text-slate-600 !text-sm">
              {{ element.createdAt | dateTimeBr }}
            </td>
          </ng-container>

          <ng-container matColumnDef="closedAt">
            <th mat-header-cell *matHeaderCellDef class="!text-xs !font-semibold !text-slate-500 !uppercase tracking-wider"> Data de Fechamento </th>
            <td mat-cell *matCellDef="let element" class="!text-slate-600 !text-sm">
              <span *ngIf="element.closedAt; else pendingText">{{ element.closedAt | dateTimeBr }}</span>
              <ng-template #pendingText>
                <span class="text-slate-400 italic">Em aberto</span>
              </ng-template>
            </td>
          </ng-container>

          <ng-container matColumnDef="details">
            <th mat-header-cell *matHeaderCellDef class="!text-xs !font-semibold !text-slate-500 !uppercase tracking-wider text-right"> Ações </th>
            <td mat-cell *matCellDef="let element" class="text-right">
              <button matIconButton color="primary" (click)="openInvoiceDetails(element.id)">
                <mat-icon>receipt</mat-icon>
              </button>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayedColumns" class="!bg-slate-50/80"></tr>
          <tr mat-row *matRowDef="let row; columns: displayedColumns;" class="hover:bg-slate-50/60 transition-colors"></tr>

          <tr class="mat-row" *matNoDataRow>
            <td class="mat-cell p-8" [attr.colspan]="displayedColumns.length">
              <app-empty-state [message]="'Não encontramos nenhuma fatura para “' + input.value + '”.'"></app-empty-state>
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
export class InvoiceList implements OnInit {
  displayedColumns: string[] = ['number', 'status', 'createdAt', 'closedAt', 'details'];
  dataSource = new MatTableDataSource<Invoice>([]);

  constructor(
    private invoiceService: InvoiceService,
    private dialog: MatDialog,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadInvoices();
  }

  loadInvoices(): void {
    this.invoiceService.getAll().subscribe({
      next: (invoices) => (this.dataSource.data = invoices),
      error: () => (this.dataSource.data = []),
    });
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  openAddDialog(): void {
    const dialogRef = this.dialog.open(InvoiceForm, {
      width: '520px',
      data: { title: 'Cadastrar Nova Fatura' },
    });

    dialogRef.afterClosed().subscribe((result: boolean) => {
      if (result) {
        this.loadInvoices();
      }
    });
  }

  openInvoiceDetails(id: number): void {
    this.router.navigate(['/invoices', id]);
  }
}
