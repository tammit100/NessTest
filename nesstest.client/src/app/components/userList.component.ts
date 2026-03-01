import { Component, OnInit } from '@angular/core';
import { UserService } from '../services/user.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';


import { User } from '../Models/User.model';
import { Role } from '../Models/Role.model';

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
    td { vertical-align: top; }
  `]
})

export class UserListComponent implements OnInit {
  allUsers: User[] = [];   // רשימת המשתמשים המלאה מהשרת
  roles: Role[] = [];      // רשימת התפקידים המלאה מהשרת
  filteredUsers: any[] = []; // הרשימה שמוצגת בטבלה
  selectedUser = {} as User;

  

  filterName: string = '';
  filterPosition: any = '';

  // משתנים לעריכה / הוספת משתמש חדש
  editName: string = '';
  editPosition: any = '';
  editPhone: string = '';
  editEmail: string = '';
  editIsActive: boolean = true;



  isNewUser: boolean = false;
  searchId: string = '';  
  updatedData: any;

  constructor(private userService: UserService) {}

  ngOnInit() {
    this.userService.getUsers().subscribe((data: User[]) => {
      this.allUsers = data;
      console.log(this.allUsers);
      this.filteredUsers = data; // תצוגה מלאה
    });

    // טעינת תפקידים ל-Combo Box
    this.userService.getRoles().subscribe((data: Role[]) => {
      this.roles = data;
    });
  }


  onSearch() {
    this.filteredUsers = this.allUsers.filter(user => {
      // חיפוש לפי שם (טקסט חופשי)
      const nameMatch = user.username.toLowerCase().includes(this.filterName.toLowerCase());
      
      // חיפוש לפי תפקיד (קוד מספר)
      // אם לא נבחר תפקיד (searchPosition === ""), נחזיר true לכולם
      const positionMatch = this.filterPosition === "" || user.role.code == this.filterPosition;

      return nameMatch && positionMatch;
    });
  }
  getRoleName(roleCode: number): string {
    const role = this.roles.find(r => r.code === roleCode);
    return role ? role.description : 'לא הוגדר';
  }

  getRole(roleCode: number): Role | undefined {
      return this.roles.find(r => r.code === roleCode);
  }

  // עריכה
  editUser(user: any) {
    this.selectedUser = { ...user }; // עותק כדי לאפשר ביטול 
    // העתקת הנתונים למשתנה עריכה
    this.editName = user.username;
    this.editPosition = user.role.code;
    this.editPhone = user.phone;
    this.editEmail = user.email;
    this.editIsActive = user.isActive;
  }

  // שמירת השינויים
  saveChanges() {

    console.log('saveCahnges');
    console.log(this.selectedUser)
    console.log(this.selectedUser.id); 
    if (this.selectedUser) {
        console.log(this.selectedUser.id); 
        console.log(this.allUsers);
        this.updatedData = this.allUsers.find(u => u.id.toString() === this.selectedUser.id.toString());
        console.log('selectedUser');
        console.log(this.selectedUser);
        console.log('updatedData');
        console.log(this.updatedData);
        this.updatedData.id =  this.selectedUser.id;
        this.updatedData.username = this.editName;
        this.updatedData.phone = this.editPhone;
        this.updatedData.email = this.editEmail;
        this.updatedData.role = this.getRole(this.editPosition);
        this.updatedData.isActive = this.editIsActive;
        this.updatedData.createDate = this.selectedUser.createdDate; 
      };
      
      console.log(this.updatedData);
      this.userService.updateUser(this.updatedData).subscribe(() => {
        // 1. עדכון הרשימה המלאה בזיכרון
        const index = this.allUsers.findIndex(u => u.id === this.updatedData.id);
        if (index !== -1) {
          this.allUsers[index] = { ...this.updatedData };
        }

        // 2. איפוס שדות החיפוש כדי שהסינון הבא יציג את כולם
        this.filterName = '';
        this.filterPosition = '';
        this.selectedUser = {} as User;

        // 3. הרצת הסינון מחדש 
        this.onSearch();
      });
    }
    
  cancelEdit() {
    this.selectedUser = {} as User;
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
    this.selectedUser = {} as User;
    console.log('this.selectedUser: ' + this.selectedUser.id)
  }

  saveNewUser() {
    const newUser : User = {
      id: this.searchId,
      username: this.editName,
      role: this.getRole(this.editPosition)!,
      phone: this.editPhone,
      email: this.editEmail,
      isActive: true,
      organizationlevels: null as any,
      managerid: '',
      isTemporaryPassword: false,
      createdDate: '',
      lastUpdateDate: ''
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

