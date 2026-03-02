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
    .text-danger.small {
        display: block;
        font-size: 11px;
        margin-top: 4px;
        white-space: nowrap; /* מונע מההודעה לרדת שורה בצורה מכוערת */
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
  filteredUsers: any[] = []; // הרשימה המפולטרת שמוצגת בטבלה
  selectedUser: User | null = null; // המשתמש הנבחר לעריכה


  
  // משתנים לסינון
  filterName: string = '';
  filterPosition: any = '';

  // משתנים לעריכה / הוספת משתמש חדש
  editName: string = '';
  editPosition: any = '';
  editPhone: string = '';
  editEmail: string = '';
  editIsActive: boolean = true;


  
  isNewUser: boolean = false; // האם משתמש חדש
  searchId: string = '';  // id של המשתמש הנוכחי
  updatedData: any;

  constructor(private userService: UserService) {}

  // initialization - טעינת נתונים מהשרת (משתמשים ותפקידים)
  ngOnInit() {
    // טעינת משתמשים
    this.userService.getUsers().subscribe((data: User[]) => {
      this.allUsers = data;
      console.log(this.allUsers);
      this.filteredUsers = data; // תצוגה מלאה
    });

    // טעינת תפקידים
    this.userService.getRoles().subscribe((data: Role[]) => {
      this.roles = data;
    });
  }


  onSearch() {
    this.filteredUsers = this.allUsers.filter(user => {
      // חיפוש לפי שם (טקסט חופשי)
      const nameMatch = user.username.toLowerCase().includes(this.filterName.toLowerCase());
      
      // חיפוש לפי תפקיד (קוד מספר)
      // אם לא נבחר תפקיד נחזיר את הכל
      const positionMatch = this.filterPosition === "" || user.roleId == this.filterPosition;

      return nameMatch && positionMatch;
    });
  }

  // הצגת תיאור  (description) לפי הקוד
  getRoleName(roleCode: number): string {
    const role = this.roles.find(r => r.code === roleCode);
    return role ? role.description : 'לא הוגדר';
  }

  // החזרת האובייקט Role (לפי הקוד)
  getRole(roleCode: number): Role | undefined {
      return this.roles.find(r => r.code === roleCode);
  }

  // עריכה
  editUser(user: any) {

    console.log('editUser')
    this.selectedUser = { ...user }; // עותק כדי לאפשר ביטול 
    // העתקת הנתונים למשתנה עריכה
    this.editName = user.username;
    this.editPosition = user.roleId;
    this.editPhone = user.phone;
    console.log('user.phone:' + user.phone);
    console.log('this.editPhone:' + this.editPhone);
    this.editEmail = user.email;
    this.editIsActive = user.isActive;
  }

  saveChanges() {
    console.log('saveChanges started');

    if (!this.selectedUser || !this.selectedUser.id) return;

    const userToUpdate = this.allUsers.find(u => u.id.toString() === this.selectedUser!.id.toString());
    if (!userToUpdate) return;

    // בניית אובייקט נקי 
    this.updatedData = {
        id: userToUpdate.id,
        username: this.editName,
        phone: this.editPhone,
        email: this.editEmail,
        roleId: Number(this.editPosition),
        organizationlevelsId: userToUpdate.organizationlevelsId || 2034,
        isActive: this.editIsActive,
        managerid: userToUpdate.managerid || "",
        password: userToUpdate.password || "", 
        salt: userToUpdate.salt || "",
        isTemporaryPassword: !!userToUpdate.isTemporaryPassword,
        createDate: userToUpdate.createDate ? new Date(userToUpdate.createDate).toISOString() : new Date().toISOString(),
        lastUpdateDate: new Date().toISOString()
    };

    this.userService.updateUser(this.updatedData).subscribe({
        next: (response) => {
            console.log('Update successful', response);
            const index = this.allUsers.findIndex(u => u.id === response.id);
            if (index !== -1) {
                this.allUsers[index] = response;
            }
            this.selectedUser = null; // סגירת מצב עריכה
            this.onSearch();
        },
        error: (err) => {
            console.error('Update failed', err);
            // אם עדיין יש 400, בדוק בלשונית Network -> Response מה השדה הבעייתי
        }
    });
  } 
  
  // ביטול מצב עריכה
  cancelEdit() {
    this.selectedUser = {} as User;
    this.searchId = '';
    this.editName = '';
    this.editPosition = '';
    this.editPhone = '';
    this.editEmail = '';
    this.editIsActive = true;
  }

  // הכנת משתמש חדש (איפוס שדות)
  prepareNewUser() {
    this.cancelEdit(); // איפוס שדות קיימים
    this.isNewUser = true;
    this.selectedUser = {} as User;
    console.log('this.selectedUser: ' + this.selectedUser.id)
  }

  // הוספת משתמש חדש
  saveNewUser() {
    // בניית אובייקט משתמש חדש תקין
    const newUser: User = {
        id: this.searchId, 
        organizationlevelsId: 2034, // ID ברירת מחדל קיים ב-DB
        roleId: Number(this.editPosition), // המרה למספר
        username: this.editName,
        phone: this.editPhone,
        email: this.editEmail,
        managerid: "", 
        password: "", 
        salt: "", 
        isTemporaryPassword: true,
        isActive: true,
        // שימוש בפורמט ISO
        createDate: new Date().toISOString(),
        lastUpdateDate: new Date().toISOString()
    };

    //console.log('Sending new user to server:', newUser);

    this.userService.addUser(newUser).subscribe({
        next: (res) => {
            //console.log('User added successfully:', res);
            
            // הוספה למערך המקומי ועדכון התצוגה
            this.allUsers.push(res);
            this.onSearch();
            
            // איפוס המצב
            this.isNewUser = false;
            this.cancelEdit();
            this.searchId = '';
        },
        error: (err) => {
            console.error('Add user failed. Check Network -> Response for details.', err);
        }
    });
  }

  isFormInvalid(): boolean {
    // Regex גמיש יותר שתואם ל-HTML
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    const phoneRegex = /^05\d-?\d{7}$/;

    // בדיקה שקיימים ערכים לפני הפעולה
    const isEmailValid = this.editEmail ? emailRegex.test(this.editEmail) : false;
    const isPhoneValid = this.editPhone ? phoneRegex.test(this.editPhone) : false;
    const isNameValid = this.editName && this.editName.trim().length > 1;

    // מחזיר true אם אחד מהם לא תקין -> הכפתור יהיה disabled
    return !(isEmailValid && isPhoneValid && isNameValid);
  } 

}

