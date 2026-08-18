import { Routes } from '@angular/router';

export const routes: Routes = [
    {path: '', redirectTo: 'invoices', pathMatch: 'full'},
    {
        path: 'products',
        loadComponent: () => import('./features/products/product-list/product-list').then(m => m.ProductList)
    },
    {
        path: 'invoices',
        loadComponent: () => import('./features/invoices/invoice-list/invoice-list').then(m => m.InvoiceList)
    },
    {
        path: 'invoices/:id',
        loadComponent: () => import('./features/invoices/invoice-detail/invoice-detail').then(m => m.InvoiceDetail)
    }
];
