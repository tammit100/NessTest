import { Component, OnInit } from '@angular/core';
import { UserService } from '../services/user.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'user-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: 'userList.html',
  styles: [`
    .error-text {
        color: #d9534f;
        font-size: 11px;
        display: block;
        margin-top: 2px;
    }
    .input-error {
        border: 1px solid #d9534f !important;
    }
    /* עיצוב כדי שהתא בטבלה לא יקפוץ בגובה כשמופיעה שגיאה */
    td { vertical-align: top; }
  `]
})

export class UserListComponent implements OnInit {
  allUsers: any[] = [];   // רשימת המשתמשים המלאה מהשרת
  roles: any[] = [];      // רשימת התפקידים המלאה מהשרת
  filteredUsers: any[] = []; // הרשימה שמוצגת בטבלה
  selectedUser: any = null;
  

  filterName: string = '';
  filterPosition: any = '';

  // משתנים לעריכה / הוספת משתמש חדש
  editName: string = '';
  editPosition: any = '';
  editPhone: string = '';
  editEmail: string = '';
  editIsActive: boolean = true;

  // // שדות החיפוש
  // searchName: string = '';
  // searchPosition: string = '';
  // searchPhone: string = '';
  // searchEmail: string = ''; 
  // searchIsActive: boolean = true;

  isNewUser: boolean = false;
  searchId: string = '';  

  constructor(private userService: UserService) {}

  ngOnInit() {
    this.userService.getUsers().subscribe(data => {
      this.allUsers = data;
      this.filteredUsers = data; // תצוגה מלאה
    });

    // טעינת תפקידים ל-Combo Box
    this.userService.getRoles().subscribe(data => {
      this.roles = data;
    });
  }


  onSearch() {
    this.filteredUsers = this.allUsers.filter(user => {
      // חיפוש לפי שם (טקסט חופשי)
      const nameMatch = user.name.toLowerCase().includes(this.filterName.toLowerCase());
      
      // חיפוש לפי תפקיד (קוד מספר)
      // אם לא נבחר תפקיד (searchPosition === ""), נחזיר true לכולם
      const positionMatch = this.filterPosition === "" || user.position == this.filterPosition;

      return nameMatch && positionMatch;
    });
  }
  getRoleName(roleCode: number): string {
    const role = this.roles.find(r => r.code === roleCode);
    return role ? role.description : 'לא הוגדר';
  }

  // בלחיצה על כפתור העריכה בטבלה
  editUser(user: any) {
    this.selectedUser = { ...user }; // יוצרים עותק כדי לא לשנות את המקור לפני השמירה
    // נטען את הנתונים לשדות החיפוש כדי שיהיה אפשר לערוך אותם
    this.editName = user.name;
    this.editPosition = user.position;
    this.editPhone = user.phone;
    this.editEmail = user.email;
    this.editIsActive = user.isActive;
  }

  // שמירת השינויים
  saveChanges() {
    if (this.selectedUser) {
      const updatedData = { 
        id: this.selectedUser.id,
        name: this.editName,
        phone: this.editPhone,
        email: this.editEmail,
        position: Number(this.editPosition), 
        isActive: this.editIsActive,
        createdDate: this.selectedUser.createdDate 
      };

      this.userService.updateUser(updatedData).subscribe(() => {
        // 1. עדכון הרשימה המלאה בזיכרון
        const index = this.allUsers.findIndex(u => u.id === updatedData.id);
        if (index !== -1) {
          this.allUsers[index] = { ...updatedData };
        }

        // 2. איפוס שדות החיפוש כדי שהסינון הבא יציג את כולם
        this.filterName = '';
        this.filterPosition = '';
        this.selectedUser = null;

        // 3. הרצת הסינון מחדש 
        this.onSearch();
      });
    }
  }
    
  cancelEdit() {
    this.selectedUser = null;
    this.searchId = '';
    this.editName = '';
    this.editPosition = '';
    this.editPhone = '';
    this.editEmail = '';
    this.editIsActive = true;
  }

  prepareNewUser() {
    this.cancelEdit(); // איפוס שדות קיימים
    this.isNewUser = true;
    this.selectedUser = { id: '' }; // יצירת אובייקט ריק כדי להפעיל את ה-ngIf בטופס
    console.log('this.selectedUser: ' + this.selectedUser.id)
  }

  saveNewUser() {
    const newUser = {
      id: this.searchId,
      name: this.editName,
      position: Number(this.editPosition),
      phone: this.editPhone,
      email: this.editEmail,
      isActive: true
    };

    this.userService.addUser(newUser).subscribe(res => {
      this.allUsers.push(res);
      this.onSearch();
      this.isNewUser = false;
      this.cancelEdit();
      this.searchId = '';
    });
  }

  
  
  isFormInvalid(): boolean {
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
    const phoneRegex = /^05\d-?\d{7}$/;

    // אם אנחנו במצב עריכה, נבדוק את השדות
    const isEmailValid = emailRegex.test(this.editEmail);
    const isPhoneValid = phoneRegex.test(this.editPhone);
    const isNameValid = this.editName && this.editName.length > 1;

    return !isEmailValid || !isPhoneValid || !isNameValid;
  }
}

