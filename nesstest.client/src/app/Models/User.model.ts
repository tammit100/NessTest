import { Organizationlevels } from "./Organizationlevels.model";
import { Role } from "./Role.model";

export interface User {
    id: string;
    organizationlevels: Organizationlevels; 
    username: string;
    role: Role;                           
    email: string;
    phone: string;
    managerid: string;
    password?: string;                      
    salt?: string;
    isTemporaryPassword: boolean;
    isActive: boolean;
    createdDate: Date | string;              
    lastUpdateDate: Date | string;
}