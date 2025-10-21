import { QueryResponse } from "./topics.models";


export interface QueryTopicResponse {
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
  responses: QueryResponse[];
}

export interface QueryResponseResponse {
  queryResponseID: number;
  comment: string;
  mediaContentUrl?: string;
  responseCreationDate: Date;
  tutorID?: number;
  queryTopicID: number;
  isSolution: boolean;
}

export interface StudentQueriesResponse {
  studentId: number;
  totalQueries: number;
  queries: QueryTopicResponse[];
}

export interface TutorQueriesResponse {
  tutorId: number;
  totalQueries: number;
  queries: QueryTopicResponse[];
}

export interface QueryResponsesResponse {
  queryTopicId: number;
  totalResponses: number;
  responses: QueryResponseResponse[];
}