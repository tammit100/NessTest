import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { Header } from './components/shared/header.component';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { LoaderComponent } from './components/loader/loader.component';
import { UserListComponent } from './components/userList.component';
import { UserUpdateComponent } from './components/userUpdate.component';
import { HttpConfigInterceptor } from './services/shared/loader/httpconfig.interceptor';
import { LoaderService } from './services/shared/loader/loader.service';
import { ModalDialogService } from './services/shared/modal-dialog.service';
import { UtilitiesService } from './services/shared/utilities.service';

const routes: Routes = [
  { path: "UserList", component: UserListComponent },
  { path: "UserUpdate", component: UserUpdateComponent },
  {
    path: '',
    redirectTo: '/UserList',
    pathMatch: 'full'
  }
]

@NgModule({
  declarations: [
    AppComponent,
    Header,
    LoaderComponent,
    UserUpdateComponent
  ],
  imports: [
    CommonModule, BrowserModule, RouterModule.forRoot(routes, { useHash: true }), ReactiveFormsModule, FormsModule,
    HttpClientModule
  ],
  providers: [{ provide: HTTP_INTERCEPTORS, useClass: HttpConfigInterceptor, multi: true }, ModalDialogService, UtilitiesService, LoaderService],
  bootstrap: [AppComponent]
})
export class AppModule { }
