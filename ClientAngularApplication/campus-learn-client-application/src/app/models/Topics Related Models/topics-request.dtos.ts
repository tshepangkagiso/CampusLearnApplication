
export interface CreateQueryRequest {
  title: string;
  description: string;
  moduleCode: string;
  studentId: number;
}

export interface UpdateQueryRequest {
  title: string;
  description: string;
  isResolved: boolean;
  isUrgent: boolean;
}

export interface CreateTutorResponseRequest {
  comment: string;
  mediaContent?: File;  
  isSolution: boolean;
}
export interface CreateStudentResponseRequest {
  comment: string;
}

export interface UpdateTutorResponseRequest {
  comment: string;
  mediaContent?: File;
  isSolution: boolean;
}

