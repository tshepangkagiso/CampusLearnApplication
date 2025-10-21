import { Qualification } from "./tutor.models";

export interface TutorProfileResponse {
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

export interface TutorQualificationResponse {
  message: string;
  module: {
    moduleCode: string;
    moduleName: string;
  };
  qualifiedSince: Date;
}

export interface TutorQualificationsResponse {
  tutor: string;
  qualifiedModules: Array<{
    moduleID: number;
    moduleCode: string;
    moduleName: string;
    programType: Qualification;
    qualifiedSince: Date;
    isActive: boolean;
  }>;
  totalQualifications: number;
}

export interface AvailableTutorModulesResponse {
  tutor: string;
  notQualifiedModules: Array<{
    moduleID: number;
    moduleCode: string;
    moduleName: string;
    programType: Qualification;
    description?: string;
  }>;
  totalAvailable: number;
}

export interface ModuleTutorsResponse {
  module: string;
  moduleCode: string;
  qualifiedTutors: Array<{
    tutorID: number;
    userProfileID: number;
    name: string;
    surname: string;
    email: string;
    studentNumber: number;
    qualification: Qualification;
    isAdmin: boolean;
    qualifiedSince: Date;
  }>;
  totalTutors: number;
}

export interface FAQResponse {
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