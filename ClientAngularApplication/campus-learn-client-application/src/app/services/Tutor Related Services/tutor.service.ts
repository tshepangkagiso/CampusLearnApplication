import { Injectable } from '@angular/core';
import { TutorProfileService } from './tutor-profile.service';
import { TutorQualificationService } from './tutor-qualification.service';
import { TutorFAQService } from './tutor-faq.service';
import { TutorTopicsService } from '../Topics Related Services/tutor-topics.service';
import { ForumService } from '../Forum Related Services/forum.service';
import { ChatService } from '../Private Messages Related Services/chat.service';
import { PrivateMessagesService } from '../Private Messages Related Services/private-messages.service';

@Injectable({
  providedIn: 'root'
})
export class TutorService {
  constructor(
    private profileService: TutorProfileService,
    private qualificationService: TutorQualificationService,
    private faqService: TutorFAQService,
    private topicsService: TutorTopicsService,
    private forumService: ForumService,
    private privateMessagesService: PrivateMessagesService,
    private chatService: ChatService

  ) {}

  // Profile methods
  getTutorProfile(tutorId: number) {
    return this.profileService.getTutorProfile(tutorId);
  }

  updateTutorProfile(request: any) {
    return this.profileService.updateTutorProfile(request);
  }

  getProfilePictureUrl(tutorId: number) {
    return this.profileService.getProfilePictureUrl(tutorId);
  }




  // Qualification methods
  qualifyForModule(request: any) {
    return this.qualificationService.qualifyForModule(request);
  }

  removeQualification(request: any) {
    return this.qualificationService.removeQualification(request);
  }

  getQualifiedModules(tutorId: number) {
    return this.qualificationService.getQualifiedModules(tutorId);
  }

  getTutorsForModule(moduleCode: string) {
    return this.qualificationService.getTutorsForModule(moduleCode);
  }





  // FAQ methods
  createFAQ(tutorId: number, request: any) {
    return this.faqService.createFAQ(tutorId, request);
  }

  getTutorFAQs(tutorId: number) {
    return this.faqService.getTutorFAQs(tutorId);
  }

  getAllTutorFAQs(tutorId: number) {
    return this.faqService.getAllTutorFAQs(tutorId);
  }

  updateFAQ(faqId: number, tutorId: number, request: any) {
    return this.faqService.updateFAQ(faqId, tutorId, request);
  }

  deleteFAQ(faqId: number, tutorId: number) {
    return this.faqService.deleteFAQ(faqId, tutorId);
  }

  getModuleFAQs(moduleCode: string) {
    return this.faqService.getModuleFAQs(moduleCode);
  }





  // Topics methods
  getTutorQueries(tutorId: number) {
    return this.topicsService.getTutorQueries(tutorId);
  }

  getAllQueries() {
    return this.topicsService.getAllQueries();
  }

  createTutorResponse(queryTopicId: number, tutorId: number, request: any) {
    return this.topicsService.createTutorResponse(queryTopicId, tutorId, request);
  }

  updateTutorResponse(responseId: number, tutorId: number, request: any) {
    return this.topicsService.updateTutorResponse(responseId, tutorId, request);
  }

  getQueryResponses(queryTopicId: number) {
    return this.topicsService.getQueryResponses(queryTopicId);
  }

  getResponseById(responseId: number) {
    return this.topicsService.getResponseById(responseId);
  }

  deleteResponse(responseId: number) {
    return this.topicsService.deleteResponse(responseId);
  }

  getResponseFile(fileName: string) {
    return this.topicsService.getResponseFile(fileName);
  }




   // Forum methods (Tutors can participate in forums too)
  createForumTopic(request: any) {
    return this.forumService.createTopic(request);
  }

  getAllForumTopics() {
    return this.forumService.getAllTopics();
  }

  getForumTopicById(topicId: number) {
    return this.forumService.getTopicById(topicId);
  }

  getForumTopicsByModule(moduleCode: string) {
    return this.forumService.getTopicsByModule(moduleCode);
  }

  getForumTopicsByUser(userProfileId: number) {
    return this.forumService.getTopicsByUser(userProfileId);
  }

  updateForumTopic(topicId: number, userProfileId: number, request: any) {
    return this.forumService.updateTopic(topicId, userProfileId, request);
  }

  deleteForumTopic(topicId: number, userProfileId: number) {
    return this.forumService.deleteTopic(topicId, userProfileId);
  }

  upvoteForumTopic(topicId: number) {
    return this.forumService.upvoteTopic(topicId);
  }




  
  // Admin forum methods
  pinForumTopic(topicId: number, isPinned: boolean) {
    return this.forumService.pinTopic(topicId, isPinned);
  }

  lockForumTopic(topicId: number, isLocked: boolean) {
    return this.forumService.lockTopic(topicId, isLocked);
  }

  createForumResponse(topicId: number, request: any) {
    return this.forumService.createResponse(topicId, request);
  }

  getForumTopicResponses(topicId: number) {
    return this.forumService.getTopicResponses(topicId);
  }

  getForumResponseById(responseId: number) {
    return this.forumService.getResponseById(responseId);
  }

  updateForumResponse(responseId: number, userProfileId: number, request: any) {
    return this.forumService.updateResponse(responseId, userProfileId, request);
  }

  deleteForumResponse(responseId: number, userProfileId: number) {
    return this.forumService.deleteResponse(responseId, userProfileId);
  }

  upvoteForumResponse(responseId: number) {
    return this.forumService.upvoteResponse(responseId);
  }

  getForumFile(fileName: string) {
    return this.forumService.getForumFile(fileName);
  }



  


 // Private Messages methods
  getAllChatRooms() {
    return this.privateMessagesService.getAllChatRooms();
  }

  getChatRoomById(roomId: number) {
    return this.privateMessagesService.getChatRoomById(roomId);
  }

  getTutorChatRooms(tutorId: number) {
    return this.privateMessagesService.getTutorChatRooms(tutorId);
  }

  getChatRoomByStudentTutorQuery(studentId: number, tutorId: number, queryId: number) {
    return this.privateMessagesService.getChatRoomByStudentTutorQuery(studentId, tutorId, queryId);
  }

  getActiveChatRooms() {
    return this.privateMessagesService.getActiveChatRooms();
  }

  getChatRoomStats() {
    return this.privateMessagesService.getChatRoomStats();
  }

  getChatRoomMessages(roomId: number) {
    return this.privateMessagesService.getChatRoomMessages(roomId);
  }





  // Real-time Chat methods
  startChatConnection() {
    return this.chatService.startConnection();
  }

  joinChatRoom(roomId: number) {
    return this.chatService.joinRoom(roomId);
  }

  sendChatMessage(roomId: number, senderId: number, content: string) {
    return this.chatService.sendMessage(roomId, senderId, content);
  }

  leaveChatRoom(roomId: number) {
    return this.chatService.leaveRoom(roomId);
  }

  onChatMessageReceived() {
    return this.chatService.messageReceived$;
  }

}