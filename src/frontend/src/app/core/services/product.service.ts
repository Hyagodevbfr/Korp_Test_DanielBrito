import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { Observable } from 'rxjs';
import { Product, CreateProductRequest, UpdateProductRequest } from '../models/product.model';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private readonly baseUrl = `${environment.stockApiUrl}/products`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.baseUrl);
  }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>(this.baseUrl, request);
  }

  update(id: number, request: UpdateProductRequest): Observable<Product> {
    return this.http.put<Product>(`${this.baseUrl}/${id}`, request);
  }
}
