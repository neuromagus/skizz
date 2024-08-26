import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { CartService } from '../services/cart.service';
import { SnackbarService } from '../services/snackbar.service';

export const emptyCartGuard: CanActivateFn = (route, state) => {
    const snack = inject(SnackbarService)
    const cartService = inject(CartService)
    const router = inject(Router)

    if (cartService.cart()) {
        return true
    } else {
        snack.error("Your cart is empty")
        router.navigateByUrl("/cart")
        return false
    }
};
