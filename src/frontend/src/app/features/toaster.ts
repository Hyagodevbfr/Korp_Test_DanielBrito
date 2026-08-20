import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root',
})
export class Toaster {
  constructor(private snackBar: MatSnackBar) {}

  success(message: string) {
    this.show(message, 'toast-success');
  }

  error(message: string) {
    this.show(message, 'toast-error');
  }

  warning(message: string) {
    this.show(message, 'toast-warning');
  }

  private show(message: string, panelClass: string) {
    this.snackBar.open(message, 'Fechar', {
      duration: 4000,
      horizontalPosition: 'end',
      verticalPosition: 'top',
      panelClass,
    });
  }
}
