export enum InvoiceStatus {
    Open = 0,
    Closed = 1
}

export interface InvoiceItem{
    id: number;
    productId: number;
    productCode: string;
    productDescription: string;
    quantity: number;
}

export interface Invoice{
    id: number;
    number: number;
    status: InvoiceStatus,
    items: InvoiceItem[];
    createdAt: string;
    closedAt: string | null;
}

export interface CreateInvoiceItemRequest{
    productId: number;
    quantity: number;
}

export interface CreateInvoiceRequest{
    items: CreateInvoiceItemRequest[];
}