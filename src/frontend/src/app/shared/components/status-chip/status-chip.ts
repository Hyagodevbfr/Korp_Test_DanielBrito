import { Component, Input } from '@angular/core';
import { MatChipsModule } from '@angular/material/chips';
import { InvoiceStatus } from '../../../core/models/invoice.model';

@Component({
  selector: 'app-status-chip',
  imports: [MatChipsModule],
  template: `
    <mat-chip-set>
      <mat-chip
        [class.chip-open]="status === InvoiceStatus.Open"
        [class.chip-closed]="status === InvoiceStatus.Closed"
        disabled>
        {{ label }}
      </mat-chip>
    </mat-chip-set>
  `,
  styles: [`
    /* O chip é sempre "disabled" (é só um indicador, não uma ação). Só usamos tokens de
       tema do Material (variáveis CSS) em vez de mirar classes internas do MDC, porque
       essas variáveis herdam normalmente através do DOM interno do <mat-chip>. */
    .chip-open {
      --mat-chip-elevated-disabled-container-color: #fef3c7;
      --mat-chip-disabled-label-text-color: #92400e;
      --mat-chip-disabled-outline-color: transparent;
    }

    .chip-closed {
      --mat-chip-elevated-disabled-container-color: #dcfce7;
      --mat-chip-disabled-label-text-color: #166534;
      --mat-chip-disabled-outline-color: transparent;
    }
  `],
})
export class StatusChip {
  readonly InvoiceStatus = InvoiceStatus;

  @Input({ required: true }) status!: InvoiceStatus;

  get label(): string {
    return this.status === InvoiceStatus.Closed ? 'Fechada' : 'Aberta';
  }
}
