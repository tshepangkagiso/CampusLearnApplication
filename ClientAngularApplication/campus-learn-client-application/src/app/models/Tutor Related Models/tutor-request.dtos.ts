import { Qualification } from "./tutor.models";

export interface UpdateTutorProfileRequest {
  userProfileID: number;
  name: string;
  surname: string;
  email: string;
  qualification: Qualification;
  studentNumber: number;
  profilePicture?: File;
}

export interface SubscribeModuleRequest {
  userId: number;
  moduleCode: string;
}

export interface CreateFAQRequest {
  question: string;
  answer: string;
  moduleCode: string;
  isPublished: boolean;
}

export interface UpdateFAQRequest {
  question: string;
  answer: string;
  moduleCode: string;
  isPublished: boolean;
}