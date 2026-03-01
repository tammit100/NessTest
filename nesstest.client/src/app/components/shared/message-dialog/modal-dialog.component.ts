import { Component, ViewChild, ElementRef, OnInit, AfterViewInit } from '@angular/core';
import { Subject } from 'rxjs';
import { ModalDialogService } from '../../../services/shared/modal-dialog.service';


@Component({
  selector: 'app-modal-dialog',
  standalone: false,
    templateUrl: './modal-dialog.component.html',
    styleUrls: ['./modal-dialog.component.css']
})
export class ModalDialogComponent implements OnInit, AfterViewInit {
    @ViewChild('modalDialog', { static: true }) dialogRef: ElementRef | undefined;
    dialogConfig = {
        modalConfirmation: new Subject<boolean>(),
        title: 'Title',
        message: '...',
        yesTitle: 'כן',
        noTitle: 'לא',
      modaltemplate: '',
    };

    constructor(private modalSrv: ModalDialogService) {
    }

    ngOnInit() {
    }

    yesButton() {
      this.dialogConfig.modalConfirmation.next(true);
      this.dialogConfig.modalConfirmation.observers.splice(0, this.dialogConfig.modalConfirmation.observers.length);
    }

    noButton() {
      this.dialogConfig.modalConfirmation.next(false);
      this.dialogConfig.modalConfirmation.observers.splice(0, this.dialogConfig.modalConfirmation.observers.length);
    }


    ngAfterViewInit() {
        this.modalSrv.dialogRef = this.dialogRef;
        this.modalSrv.dialogConfig = this.dialogConfig;
    }

}

