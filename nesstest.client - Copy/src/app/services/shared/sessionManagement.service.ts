import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
    providedIn: 'root'
  })
export class SessionManagementService {

    objectKeys: string[] = [];
    constructor() { }
    private emitChangeSource = new Subject<any>();

    changeEmitted$ = this.emitChangeSource.asObservable();
  
    emitChange(change: any) {
        this.emitChangeSource.next(change);
    }
  
    getObjectByKey(keyName: string) {
        if (!(this as any)[keyName]) {
            (this as any)[keyName] = sessionStorage[keyName] ? JSON.parse((sessionStorage as any).getItem(keyName)) : null;
        }
        return (this as any)[keyName];
    }

    removeObjectByKey(keyName: string) {
        sessionStorage.removeItem(keyName);
    }

    saveObject(keyName: string, obj: any) {
        (this as any)[keyName] = obj;
        sessionStorage.setItem(keyName, JSON.stringify(obj));
        if (this.objectKeys.indexOf(keyName) < 0) {
            this.objectKeys.push(keyName);
        }
    }

    clearTemporaryData() {
        this.objectKeys.forEach((key: any) => {
            if (key !== 'userDetails' && key !== 'lookups') {
                (this as any)[key] = null;
                sessionStorage.removeItem(key);
            }
        });
    }

    clearAllData() {
        this.objectKeys.forEach((key: any) => (this as any)[key] = null);
        this.objectKeys = [];
        sessionStorage.clear();
    }

    saveUserDetailsInSession(user: any) {
        this.saveObject('userDetails', user);
        this.emitChange("userDetails");
    }

    getUserDetailsFromSession() {
        return this.getObjectByKey('userDetails');

    }
}
