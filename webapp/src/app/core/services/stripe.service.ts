import { inject, Injectable } from '@angular/core';
import { loadStripe, Stripe, StripeAddressElement, StripeAddressElementOptions, StripeElements } from "@stripe/stripe-js"
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { CartService } from './cart.service';
import { Cart } from '../../shared/models/cart';
import { firstValueFrom, map } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class StripeService {
    private stripePromise: Promise<Stripe | null>
    private http = inject(HttpClient)
    baseUrl = environment.apiUrl
    private cartService = inject(CartService)
    private elements?: StripeElements
    private addressElement?: StripeAddressElement

    constructor() {
        this.stripePromise = loadStripe(environment.stripePublicKey)
    }

    getStripeInstance() {
        return this.stripePromise
    }

    async initializeElements() {
        if (!this.elements) {
            const stripe = await this.getStripeInstance()

            if (stripe) {
                const cart = await firstValueFrom(this.createOrUpdatePaymentIntent())
                this.elements = stripe.elements({ clientSecret: cart.clientSecret, appearance: { labels: "floating" } })
            } else {
                throw new Error("Stripe has not been loaded")
            }
        }

        return this.elements
    }

    async createAddressElement() {
        if (!this.addressElement) {
            const elements = await this.initializeElements()

            if (!elements) throw new Error("Elements instance has not been loaded")

            const options: StripeAddressElementOptions = {
                mode: "shipping"
            }

            this.addressElement = elements.create("address", options)
        }

        return  this.addressElement
    }

        createOrUpdatePaymentIntent() {
            const cart = this.cartService.cart()

            if (!cart) throw new Error("Problem with cart ")

            return this.http.post<Cart>(`${this.baseUrl}payments/${cart.id}`, {}).pipe(
                map(cart => {
                    this.cartService.cart.set(cart)
                    return cart
                })
            )
        }
    }
