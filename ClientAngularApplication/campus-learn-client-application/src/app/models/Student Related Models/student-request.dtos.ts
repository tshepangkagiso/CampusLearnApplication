import { Qualification } from "./student.models";


export interface StudentRegisterRequest {
  name: string;
  surname: string;
  email: string;
  password: string;
  qualification: Qualification;
  studentNumber: number;
}

export interface StudentLoginRequest {
  email: string;
  password: string;
}

export interface UpdateStudentProfileRequest {
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

export interface ChangePasswordRequest {
  userID: number;
  currentPassword: string;
  newPassword: string;
}