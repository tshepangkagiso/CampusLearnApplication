
export interface CreateForumTopicRequest {
  title: string;
  description: string;
  moduleCode: string;
  userProfileID: number;
  isAnonymous: boolean;
  anonymousName?: string;
}

export interface UpdateForumTopicRequest {
  title: string;
  description: string;
  moduleCode: string;
}

export interface CreateForumResponseRequest {
  comment: string;
  mediaContent?: File;
  userProfileID: number;
  isAnonymous: boolean;
  anonymousName?: string;
}

export interface UpdateForumResponseRequest {
  comment: string;
  mediaContent?: File;
}