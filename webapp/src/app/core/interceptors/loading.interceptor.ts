import { HttpInterceptorFn } from '@angular/common/http';
import { delay } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
    return next(req).pipe(
        // maybe need more, than 0.5 sec for shaping speed
        delay(500)
    )
};
