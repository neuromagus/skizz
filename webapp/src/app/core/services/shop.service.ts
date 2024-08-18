import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Product } from '../../shared/models/product';
import { Pagination } from '../../shared/models/pagination';
import { ShopParams } from '../../shared/models/shopParams';

@Injectable({
    providedIn: 'root'
})
export class ShopService {
    baseUrl = "https://localhost:5051/api/"
    private http = inject(HttpClient)

    types: string[] = []
    brands: string[] = []

    getProducts(shopParams: ShopParams) {
        let params = new HttpParams()

        if (shopParams.brands.length > 0) {
            params = params.append("brands", shopParams.brands.join(","))
        }

        if (shopParams.types.length > 0) {
            params = params.append("types", shopParams.types.join(","))
        }

        if (shopParams.sort) {
            params = params.append("sort", shopParams.sort)
        }
        
        params = params.append("pageSize", 20)

        return this.http.get<Pagination<Product>>(this.baseUrl + "products", {params})
    }

    getBrands() {
        if (this.brands.length > 0) return
        return this.http.get<string[]>(this.baseUrl + "products/brands").subscribe({
            next: response => this.brands = response
        })
    }

    getTypes() {
        if (this.types.length > 0) return
        return this.http.get<string[]>(this.baseUrl + "products/types").subscribe({
            next: response => this.types = response
        })
    }
}
