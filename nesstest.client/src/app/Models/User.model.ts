export interface User {
    id: string;
    organizationlevelsId: number; // שינוי מ-Number ל-number
    roleId: number;              // שינוי מ-Number ל-number
    username: string; 
    email: string;
    phone: string;
    managerid: string;           // מומלץ m קטנה אם ה-Serializer מוגדר ל-CamelCase
    password: string;            // מומלץ p קטנה
    salt: string;                // מומלץ s קטנה
    isTemporaryPassword: boolean;
    isActive: boolean;
    createDate: string | Date;
    lastUpdateDate: string | Date;
}
