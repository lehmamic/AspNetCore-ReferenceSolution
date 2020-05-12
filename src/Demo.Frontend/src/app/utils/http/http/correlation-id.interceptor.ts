import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { v4 as uuid } from 'uuid';

@Injectable()
export class AngularCorrelationIdHttpInterceptor implements HttpInterceptor {
    constructor() {
    }

    public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        req = req.clone({
            setHeaders: {
                CorrelationId: uuid(),
            },
        });

        return next.handle(req);
    }
}
