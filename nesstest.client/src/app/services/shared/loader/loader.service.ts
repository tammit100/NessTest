import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
//import { LoaderState } from '../loader/loader';

@Injectable({
  providedIn: 'root'
})

export class LoaderService {
  private loaderSubject = new Subject<LoaderState>();
  loaderState = this.loaderSubject.asObservable();
  language :any;
   
  constructor() { }

  setLanguage(lang: any) {
    this.language = lang;
  }

  getLanguage() {
    return this.language;
  }

  show() {
    this.loaderSubject.next(<LoaderState>{ show: true });
  }

  hide() {
    this.loaderSubject.next(<LoaderState>{ show: false });
  }
}

export interface LoaderState {
  show: boolean;
}
