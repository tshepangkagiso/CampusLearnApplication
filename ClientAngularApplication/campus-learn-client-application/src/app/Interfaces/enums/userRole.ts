export enum UserRoleString {
  Student = 'Student',
  Tutor = 'Tutor',
  Admin = 'Admin'
}

export enum UserRoleNumber {
  Student = 0,
  Tutor = 1,
  Admin = 2
}

export const UserRoleMap = [UserRoleString.Student, UserRoleString.Tutor, UserRoleString.Admin];