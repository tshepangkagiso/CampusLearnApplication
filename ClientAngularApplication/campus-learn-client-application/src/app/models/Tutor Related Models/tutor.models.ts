
export interface TutorProfile {
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

export interface TutorModule {
  tutorModuleID: number;
  tutorID: number;
  moduleID: number;
  qualifiedSince: Date;
  isActive: boolean;
  module?: Module;
}

export interface Module {
  moduleID: number;
  moduleCode: string;
  moduleName: string;
  programType: Qualification;
  description?: string;
}

export interface FAQ {
  faqID: number;
  frequentlyAskedQuestion: string;
  answer: string;
  tutorID: number;
  moduleCode: string;
  createdAt: Date;
  isPublished: boolean;
  viewCount: number;
  updatedAt?: Date;
}