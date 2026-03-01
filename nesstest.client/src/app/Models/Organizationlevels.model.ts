export interface Organizationlevels {
    id: number;
    name: string;
    parentId?: number; // ה-? מציין שזה יכול להיות null/undefined (כמו int?)
    isRowDeleted: boolean;
}