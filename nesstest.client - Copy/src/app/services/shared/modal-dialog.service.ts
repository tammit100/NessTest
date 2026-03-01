import { ElementRef, Injectable } from '@angular/core';

declare var $: any;

@Injectable({
  providedIn: 'root'
})
export class ModalDialogService {
  dialogRef: ElementRef | undefined = undefined;
  dialogConfig: any = {};


  constructor() {
  }


  showYesNoMessage(title: string = '', msg: string = '', btnYesText: string = '', btnNoText: string = '') {

    setTimeout(() => {
      this.dialogConfig.title = title;
      this.dialogConfig.message = msg;
      this.dialogConfig.yesTitle = btnYesText;
      this.dialogConfig.noTitle = btnNoText;
      this.dialogConfig.modaltemplate = null;
      $(this.dialogRef?.nativeElement).modal('show');
    });

    return this.dialogConfig.modalConfirmation.asObservable();
  }

  showOkMessage(title: string = '', msg: string = '', btnYesText: string = '') {
    return this.showYesNoMessage(title, msg, btnYesText);
  }

  showCustomMessage(title: string = '', template: string = '', btnYesText: string = '') {

    setTimeout(() => {
      this.dialogConfig.title = title;
      this.dialogConfig.modaltemplate = template;
      this.dialogConfig.yesTitle = btnYesText;
      this.dialogConfig.noTitle = null
      this.dialogConfig.message = null;

      $(this.dialogRef?.nativeElement).modal('show');
    });

    return this.dialogConfig.modalConfirmation.asObservable();
  }

  showMessageAutoClose(title: string = '', msg: string = '') {

    setTimeout(() => {
      this.dialogConfig.title = title;
      this.dialogConfig.message = msg;
      this.dialogConfig.yesTitle = '';
      this.dialogConfig.noTitle = '';
      this.dialogConfig.modaltemplate = null;
      $(this.dialogRef?.nativeElement).modal('show');
    });

    let milliSeconds = 0;

    let timerId = setInterval(() => {
      milliSeconds += 1000;
      //if (!!(milliSeconds % 10000)) {
      //    this.dialogConfig.message = `${title} : ${milliSeconds / 1000}`;
      //}

      if (milliSeconds === 3000) {
        clearInterval(timerId);
        this.dialogConfig.modalConfirmation.next(false);
        $(this.dialogRef?.nativeElement).modal('hide');
      }
    }, 1000);

    return this.dialogConfig.modalConfirmation.asObservable();
  }
  closeModal() {
    $(this.dialogRef?.nativeElement).modal('hide');
  }


}
