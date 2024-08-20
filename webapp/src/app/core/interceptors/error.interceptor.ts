import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { SnackbarService } from '../services/snackbar.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
    const router = inject(Router)
    const snackBar = inject(SnackbarService)


    return next(req).pipe(
        catchError((err: HttpErrorResponse) => {
            switch (err.status) {
                case 404:
                    router.navigateByUrl("/not-found")
                    break
                case 500:
                    router.navigateByUrl("/server-error")
                    break
                default:
                    snackBar.error(err.error.title || err.error)
                    break
            }

            return throwError(() => err)
        })
    )
};
