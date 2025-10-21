import { Qualification } from "./student.models";


export interface StudentLoginResponse {
  token: string;
  user: {
    userProfileID: number;
    name: string;
    surname: string;
    email: string;
    qualification: Qualification;
  };
}

export interface StudentProfileResponse {
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

export interface SubscriptionResponse {
  message: string;
  module: {
    moduleCode: string;
    moduleName: string;
  };
  subscriptionDate: Date;
}

export interface StudentSubscriptionsResponse {
  student: string;
  subscribedModules: Array<{
    moduleID: number;
    moduleCode: string;
    moduleName: string;
    programType: Qualification;
    subscribedAt: Date;
  }>;
  totalSubscriptions: number;
}

export interface AvailableModulesResponse {
  student: string;
  notSubscribedModules: Array<{
    moduleID: number;
    moduleCode: string;
    moduleName: string;
    programType: Qualification;
    description?: string;
  }>;
  totalAvailable: number;
}

export interface AssignedTutorResponse {
  message: string;
  tutor: {
    tutorID: number;
    name: string;
    surname: string;
    email: string;
    qualifiedSince: Date;
  };
}