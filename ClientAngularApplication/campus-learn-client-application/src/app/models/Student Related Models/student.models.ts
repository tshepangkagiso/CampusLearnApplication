export interface StudentProfile {
  userProfileID: number;
  name: string;
  surname: string;
  email: string;
  qualification: Qualification;
  studentNumber: number;
  profilePictureUrl?: string;
  isActive: boolean;
  createdAt: Date;
  lastLogin?: Date;
}

export enum Qualification {
  DIP = 0,
  BIT = 1,
  BCOM = 2
}

export interface StudentModule {
  studentModuleID: number;
  studentID: number;
  moduleID: number;
  subscribedAt: Date;
  module?: Module;
}

export interface Module {
  moduleID: number;
  moduleCode: string;
  moduleName: string;
  programType: Qualification;
  description?: string;
}

export interface Tutor {
  tutorID: number;
  userProfileID: number;
  name: string;
  surname: string;
  email: string;
  qualification: Qualification;
}

export enum UserRole {
  Student = 0,
  Tutor = 1,
  Admin = 2
}