import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
    const router = inject(Router)


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
                    alert(err.error.title || err.error)
                    break
            }

            return throwError(() => err)
        })
    )
};
