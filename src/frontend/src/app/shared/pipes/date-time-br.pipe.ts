import { Pipe, PipeTransform } from '@angular/core';

const formatter = new Intl.DateTimeFormat('pt-BR', {
  timeZone: 'America/Sao_Paulo',
  day: '2-digit',
  month: '2-digit',
  year: 'numeric',
  hour: '2-digit',
  minute: '2-digit',
  hour12: false,
});

/**
 * Formata datas vindas do backend (sem timezone, ex: "2026-08-19 14:30:00", que
 * representam um instante UTC) no padrão brasileiro dd/MM/yyyy HH:mm, já convertido
 * para o horário de Brasília. Usa `Intl` nativo em vez do `DatePipe` do Angular para
 * não depender de registro de locale (`registerLocaleData`).
 */
@Pipe({
  name: 'dateTimeBr',
})
export class DateTimeBrPipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    if (!value) {
      return '';
    }

    // O backend normalmente envia a data sem indicação de timezone (ex.:
    // "2026-08-18T22:57:15" ou "2026-08-18 22:57:15"), mas sempre em UTC
    // (DateTime.UtcNow), então acrescentamos o "Z" para o parse ser correto.
    // Em alguns casos (ex.: nota recém-fechada, cujo DTO é montado a partir da
    // entidade em memória em vez de recarregado do banco) o próprio backend já
    // inclui o "Z" ou um offset (+HH:mm) — nesses casos não duplicamos o sufixo,
    // senão o parse falha e a data desaparece.
    const normalized = value.replace(' ', 'T');
    const hasTimezone = /[zZ]$|[+-]\d{2}:\d{2}$/.test(normalized);
    const isoValue = hasTimezone ? normalized : `${normalized}Z`;
    const date = new Date(isoValue);

    if (isNaN(date.getTime())) {
      return '';
    }

    // O Intl separa data e hora com vírgula no formato pt-BR; removemos para
    // manter o padrão "dd/MM/yyyy HH:mm".
    return formatter.format(date).replace(',', '');
  }
}
