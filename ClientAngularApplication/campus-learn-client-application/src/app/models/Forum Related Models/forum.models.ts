
export interface ForumTopic {
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
  responses?: ForumTopicResponse[];
  user?: UserProfile;
}

export interface ForumTopicResponse {
  responseID: number;
  comment: string;
  mediaContentUrl?: string;
  responseCreationDate: Date;
  userProfileID: number;
  forumTopicID: number;
  isAnonymous: boolean;
  anonymousName?: string;
  responseUpVote: number;
  user?: UserProfile;
}

export interface UserProfile {
  userProfileID: number;
  name: string;
  surname: string;
  email: string;
}