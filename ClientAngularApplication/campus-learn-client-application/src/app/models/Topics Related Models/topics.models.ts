
export interface QueryTopic {
  queryTopicID: number;
  queryTopicTitle: string;
  queryTopicDescription: string;
  relatedModuleCode: string;
  studentID: number;
  assignedTutorID?: number;
  topicCreationDate: Date;
  lastActivity: Date;
  isResolved: boolean;
  isUrgent: boolean;
  isAssigned: boolean;
  assignedAt?: Date;
  responses?: QueryResponse[];
  student?: Student;
  tutor?: Tutor;
}

export interface QueryResponse {
  queryResponseID: number;
  comment: string;
  mediaContentUrl?: string;
  responseCreationDate: Date;
  tutorID?: number;
  queryTopicID: number;
  isSolution: boolean;
  tutor?: Tutor;
}

export interface Student {
  studentID: number;
  userProfileID: number;
  name?: string;
  surname?: string;
  email?: string;
}

export interface Tutor {
  tutorID: number;
  userProfileID: number;
  name?: string;
  surname?: string;
  email?: string;
}