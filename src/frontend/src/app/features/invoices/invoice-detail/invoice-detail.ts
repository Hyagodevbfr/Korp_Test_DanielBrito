import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { finalize } from 'rxjs';

import { Invoice, InvoiceStatus } from '../../../core/models/invoice.model';
import { InvoiceService } from '../../../core/services/invoice.service';
import { Toaster } from '../../toaster';
import { StatusChip } from '../../../shared/components/status-chip/status-chip';
import { DateTimeBrPipe } from '../../../shared/pipes/date-time-br.pipe';

@Component({
  selector: 'app-invoice-detail',
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    StatusChip,
    DateTimeBrPipe,
  ],
  template: `
    <div class="no-print px-6 pt-5 pb-3 border-b border-slate-100 flex items-center justify-between" *ngIf="invoice">
      <div class="flex items-center gap-3">
        <h2 class="text-xl font-bold text-slate-800 tracking-tight m-0">
          {{ title }}
        </h2>
        <app-status-chip [status]="invoice.status"></app-status-chip>
      </div>

      <button matIconButton (click)="redirectToInvoices()" class="text-slate-400 hover:text-slate-600">
        <mat-icon>close</mat-icon>
      </button>
    </div>

    <div *ngIf="!invoice" class="flex items-center justify-center p-10">
      <mat-spinner diameter="36"></mat-spinner>
    </div>

    <div class="px-6 py-4 overflow-x-hidden" id="printable-invoice" *ngIf="invoice">
      <div class="flex flex-col gap-5">

        <div class="print-only hidden print:block border-b border-slate-300 pb-4 mb-2">
          <h1 class="text-2xl font-bold text-slate-900 m-0">COMPROVANTE DE NOTA FISCAL</h1>
          <p class="text-sm text-slate-600 m-0">Emitido em: {{ currentDate | date:'dd/MM/yyyy HH:mm' }}</p>
        </div>

        <div class="grid grid-cols-2 sm:grid-cols-3 gap-3 p-3 bg-slate-50 rounded-xl border border-slate-200/80">
          <div>
            <span class="block text-xs font-medium text-slate-400 uppercase tracking-wider">Número</span>
            <span class="text-sm font-bold text-slate-700 font-mono">#{{ invoice.number || invoice.id }}</span>
          </div>

          <span class="text-sm font-semibold text-slate-700">
          <span class="block text-xs font-medium text-slate-400 uppercase tracking-wider">Aberta Em</span>
            {{ invoice.createdAt | dateTimeBr }}
          </span>

          <div>
            <span class="block text-xs font-medium text-slate-400 uppercase tracking-wider">Fechada Em</span>
            <span class="text-sm font-semibold text-slate-700">
              <ng-container *ngIf="invoice.closedAt; else pendingText">
                {{ invoice.closedAt | dateTimeBr }}
              </ng-container>
              <ng-template #pendingText>
                <span class="text-600 font-normal italic">Em aberto</span>
              </ng-template>
            </span>
          </div>
        </div>

        <div class="flex items-center justify-between pb-1 border-b border-slate-200/60">
          <div class="flex items-center gap-2">
            <span class="text-sm font-semibold text-slate-700">Itens da Nota Fiscal</span>
            <span class="px-2 py-0.5 rounded-full bg-slate-100 text-slate-600 text-xs font-medium border border-slate-200">
              {{ invoice.items.length }}
            </span>
          </div>
        </div>

        <div class="flex flex-col gap-2.5 max-h-[350px] overflow-y-auto pr-1">
          <div
            *ngFor="let item of invoice.items; let i = index"
            class="flex items-center justify-between p-3 bg-slate-50/70 rounded-xl border border-slate-200/80">

            <div class="flex items-center gap-3">
              <span class="text-xs font-bold text-slate-400 w-4 text-center">
                {{ i + 1 }}
              </span>

              <div class="flex flex-col">
                <span class="text-sm font-medium text-slate-800">
                  {{ item.productDescription || 'Produto #' + item.productId }}
                </span>
                <span *ngIf="item.productCode" class="text-xs font-mono text-slate-400">
                  Código: {{ item.productCode }}
                </span>
              </div>
            </div>

            <div class="flex items-center gap-1 bg-white px-3 py-1 rounded-lg border border-slate-200 text-xs font-semibold text-slate-700">
              <span class="text-slate-400 font-normal">Qtd:</span>
              <span>{{ item.quantity }}</span>
            </div>

          </div>

          <div *ngIf="invoice.items.length === 0" class="text-center py-6 text-slate-400 text-sm">
            Nenhum item encontrado nesta fatura.
          </div>
        </div>

      </div>
    </div>

    <div class="no-print px-6 py-4 border-t border-slate-100 flex items-center justify-end gap-2 bg-slate-50/50" *ngIf="invoice">

      <button matButton type="button" (click)="redirectToInvoices()" [disabled]="isLoading">
        Voltar
      </button>

      <button
        *ngIf="!isInvoiceClosed"
        matButton="filled"
        color="primary"
        (click)="printInvoice()"
        [disabled]="isLoading">
        <mat-spinner *ngIf="isLoading" diameter="18" class="mr-2 inline-block"></mat-spinner>
        <mat-icon *ngIf="!isLoading">print</mat-icon>
        <span>{{ isLoading ? 'Fechando e preparando impressão...' : 'Imprimir' }}</span>
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
export class InvoiceDetail implements OnInit {
  invoice!: Invoice;
  isLoading = false;
  currentDate = new Date();

  constructor(
    private invoiceService: InvoiceService,
    private toast: Toaster,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {}

  get title(): string {
    return this.invoice ? `Nota Fiscal #${this.invoice.number || this.invoice.id}` : 'Detalhes da Fatura';
  }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.invoiceService.getById(id).subscribe({
        next: (invoice) => (this.invoice = invoice),
        error: () => {},
      });
    }
  }

  get isInvoiceClosed(): boolean {
    return !!this.invoice && (this.invoice.status === InvoiceStatus.Closed || !!this.invoice.closedAt);
  }

  // Fecha a nota no backend (valida itens e baixa o estoque) e só então dispara a
  // impressão do navegador. Em caso de falha, a nota permanece Aberta e o erro é
  // reportado ao usuário pelo interceptor global.
  printInvoice(): void {
    if (!this.invoice?.id || this.isInvoiceClosed) {
      return;
    }

    this.isLoading = true;

    this.invoiceService.close(this.invoice.id)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (updatedInvoice) => {
          this.invoice = updatedInvoice
            ? { ...this.invoice, ...updatedInvoice }
            : { ...this.invoice, closedAt: new Date().toISOString() };

          this.toast.success('Nota fiscal fechada com sucesso!');

          // Força a atualização síncrona do DOM antes de abrir o diálogo de
          // impressão: window.print() é síncrono e, sem isso, o navegador
          // captura o layout ainda com "closedAt" antigo (nota "Em aberto"),
          // pois o Angular só re-renderizaria no próximo ciclo de change detection.
          this.cdr.detectChanges();
          window.print();
        },
      });
  }

  redirectToInvoices(): void {
    this.router.navigate(['/invoices']);
  }
}
