import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';

@Injectable()
export class UtilitiesService {

    constructor(private http: HttpClient) {
    }

    isValidForm(form: FormGroup) {

        // over all controls in form and check validations of them
        for (var c in form.controls) {
            if (!this.isFormControlValid(form, c)) {
                return false;
            }
        }
        return true;
    }

    isFormControlValid(form: FormGroup, name: string) {

        //for specific control over his errors and return validation status
        for (var en in form.controls[name].errors) {
            if (form.controls[name].hasError(en) == true) {
                return false;
            }
        }
        return true;
    }

    getMultiSelectTexts() {
        return {
            defaultTitle: 'בחר',
            checkAll: 'בחר הכל',
            uncheckAll: 'הסר הכל',
            allSelected: 'נבחר הכל',
            searchEmptyResult: 'אין תוצאות',
            checked: 'נבחר',
            checkedPlural: 'נבחרו',
            searchPlaceholder: 'חפש'
        };
    }

    getMultiSelectSettings() {
        var settings = {            
            showCheckAll: true,
            showUncheckAll: true,
            displayAllSelectedText: false,
            dynamicTitleMaxItems: 2
        };
        return settings;
    }

    getMultiSelectWithSearchSettings() {
        var settings = {
            enableSearch: true,
            showCheckAll: true,
            showUncheckAll: true,
            displayAllSelectedText: false,
            dynamicTitleMaxItems: 2
        };
        return settings;
    }
    
}
