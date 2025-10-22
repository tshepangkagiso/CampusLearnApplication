
export interface ForumTopicResponse {
  responseID: number;
  responseUpVote: any;
  forumTopicID: number;
  forumTopicTitle: string;
  forumTopicDescription: string;
  relatedModuleCode: string;
  userProfileID: number;
  isAnonymous: boolean;
  anonymousName?: string;
  topicCreationDate: Date;
  lastActivity: Date;
  viewCount: number;
  topicUpVote: number;
  isPinned: boolean;
  isLocked: boolean;
  responses: ForumTopicResponse[];
}

export interface ForumTopicListResponse {
  totalTopics: number;
  topics: ForumTopicResponse[];
}

export interface ModuleForumResponse {
  moduleCode: string;
  totalTopics: number;
  topics: ForumTopicResponse[];
}

export interface UserForumResponse {
  userProfileId: number;
  totalTopics: number;
  topics: ForumTopicResponse[];
}

export interface ForumResponseResponse {
  responseID: number;
  comment: string;
  mediaContentUrl?: string;
  responseCreationDate: Date;
  userProfileID: number;
  forumTopicID: number;
  isAnonymous: boolean;
  anonymousName?: string;
  responseUpVote: number;
}

export interface ForumResponsesResponse {
  topicId: number;
  totalResponses: number;
  responses: ForumResponseResponse[];
}