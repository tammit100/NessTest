import { Injectable, Inject, PLATFORM_ID } from '@angular/core';

import {
  HttpInterceptor,
  HttpRequest,
  HttpResponse,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse
} from '@angular/common/http';

import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { LoaderService } from './loader.service';
import { isPlatformBrowser } from '@angular/common';
import { ModalDialogService } from '../modal-dialog.service';
import { MessagesEN, MessagesHE } from '../../../Enum/messages.enum';

@Injectable()
export class HttpConfigInterceptor implements HttpInterceptor {
  requests: any = [];
  res: any = null;
  error: any = null;
  status: number = 0;
  isBrowser: boolean;
  messagesEnum: any = {};

  constructor(private router: Router, private modalSrv: ModalDialogService, private loaderService: LoaderService, @Inject(PLATFORM_ID) platformId: Object) {

    this.isBrowser = isPlatformBrowser(platformId);
    this.messagesEnum = this.loaderService.getLanguage() == "HE" ? MessagesHE : MessagesEN;
  }
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.showLoader();
    request = request.clone({ withCredentials: true });

    if (this.isBrowser) {
      const token: string | null = localStorage.getItem('token');

      if (token) {
        request = request.clone({ headers: request.headers.set('Authorization', 'basic ' + token) });
      }
    }
    if (!this.isBrowser || !(request.body instanceof FormData)) {
      if (!request.headers.has('Content-Type')) {
        request = request.clone({ headers: request.headers.set('Content-Type', 'application/json') });
      }

      request = request.clone({ headers: request.headers.set('Accept', 'application/json') });
      this.requests.push(request);
    }
    var that = this;
    return next.handle(request).pipe(
      map((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse) {
          var token = event.headers.get('SessionID');
          var errorMessage;
          if (token && this.isBrowser) {
            localStorage.setItem('token', token);
          }
          if (event.body && event.body.Messages && event.body.Messages.length > 0) {
            errorMessage = event.body.Messages.find((m: any) => m.LogLevel < 2);
          } else if (event.body && event.body.messages && event.body.messages.length > 0) {
            errorMessage = event.body.messages.find((m: any) => m.LogLevel < 2);
          }
          if (errorMessage) {
            this.showErrors([errorMessage]);
            throwError(errorMessage);
          }
          that.requests.pop();
          if (that.requests && that.requests.length == 0)
            this.hideLoader();
        }
        return event;
      }),
      catchError((error: HttpErrorResponse) => {
        //let data = {};
        //data = {
        //  reason: error && error.error.reason ? error.error.reason : '',
        //  status: error.status
        //};


        that.status = error.status;
        that.error = error;
        that.requests.pop();


        if (this.status === 500) {
          let msg = [{ 'Text': '' }];

          msg[0].Text = this.messagesEnum['GeneralError'];
          this.showErrors(msg);


        }
        else if (this.status === 403 || this.status == 401) {
          let msg = [{ 'Text': '' }];

          msg[0].Text = this.messagesEnum['AuthorizeAction'];
          this.showErrors(msg);
        }
        else if (this.status === 410) {
          let msg = [{ 'Text': '' }];

          msg[0].Text = this.messagesEnum['Idle'];
          this.showErrors(msg);
          this.router.navigateByUrl('/');
        }
        else if (this.status === 400) {
          let msg = [{ 'Text': '' }];
          msg.pop();

          let errorObj = this.error.error;
          for (let p in errorObj) {
            msg.push({ 'Text': errorObj[p][0] });

          }
          this.showErrors(msg);

        }
        if (that.requests && that.requests.length == 0) {
          this.hideLoader();
        }
        return throwError(error);
      }));
  }

  showErrors(messages: Array<any>) {
    let msg: string = '';
    for (var i = 0; i < messages.length; i++) {
      msg += ' ' + messages[i].Text + ' ';
    }
    this.modalSrv.showOkMessage(this.messagesEnum['title'], msg, this.messagesEnum['OKBtn']);
  }

  showLoader() {
    this.loaderService.show();
  }
  hideLoader() {
    this.loaderService.hide();
  }
}
