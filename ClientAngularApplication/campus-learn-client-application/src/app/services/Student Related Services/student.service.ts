import { Injectable } from '@angular/core';
import { StudentProfileService } from './student-profile.service';
import { StudentSubscriptionService } from './student-subscription.service';
import { StudentTopicsService } from '../Topics Related Services/student-topics.service';
import { ForumService } from '../Forum Related Services/forum.service';
import { ChatService } from '../Private Messages Related Services/chat.service';
import { PrivateMessagesService } from '../Private Messages Related Services/private-messages.service';
import { SubscribeModuleRequest } from '../../models/Student Related Models/student-request.dtos';
import { TutorProfileService } from '../Tutor Related Services/tutor-profile.service';

@Injectable({
  providedIn: 'root'
})
export class StudentService {

  constructor(
    private profileService: StudentProfileService,
    private subscriptionService: StudentSubscriptionService,
    private topicsService: StudentTopicsService,
    private forumService: ForumService,
    private privateMessagesService: PrivateMessagesService,
    private chatService: ChatService,
    private profileTutorService: TutorProfileService
  ) {}


  // Profile methods
  getStudentProfile(studentId: number) {
    return this.profileService.getStudentProfile(studentId);
  }

  updateStudentProfile(request: any) {
    return this.profileService.updateStudentProfile(request);
  }

  getProfilePictureUrl(studentId: number) {
    return this.profileService.getProfilePictureUrl(studentId);
  }

  getProfilePicture(fileName: string)
  {
    return this.profileService.getProfilePicture(fileName);
  }

  getRandomTutorForModule(moduleCode: string) {
    return this.profileTutorService.getRandomTutorForModule(moduleCode);
  }

  getStudentIdByUserId(userId:number){
    return this.profileService.getStudentIdByUserId(userId);
  }


  // Subscription methods
  subscribeToModule(request: SubscribeModuleRequest) {
    return this.subscriptionService.subscribeToModule(request);
  }

  unsubscribeFromModule(request: SubscribeModuleRequest) {
    return this.subscriptionService.unsubscribeFromModule(request);
  }

  getSubscribedModules(studentId: number) {
    return this.subscriptionService.getSubscribedModules(studentId);
  }

  getAvailableModules(studentId: number) {
    return this.subscriptionService.getAvailableModules(studentId);
  }

  getAssignedTutor(moduleCode: string) {
    return this.subscriptionService.getAssignedTutor(moduleCode);
  }




  // Topics methods
  createQuery(request: any, tutorId: number) {
    return this.topicsService.createQuery(request, tutorId);
  }

  getStudentQueries(studentId: number) {
    return this.topicsService.getStudentQueries(studentId);
  }

  getQueryById(queryId: number) {
    return this.topicsService.getQueryById(queryId);
  }

  updateQuery(queryId: number, request: any) {
    return this.topicsService.updateQuery(queryId, request);
  }

  deleteQuery(queryId: number) {
    return this.topicsService.deleteQuery(queryId);
  }

  createStudentResponse(queryTopicId: number, studentId: number, request: any) {
    return this.topicsService.createStudentResponse(queryTopicId, studentId, request);
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



   // Forum methods
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

  getStudentChatRooms(studentId: number) {
    return this.privateMessagesService.getStudentChatRooms(studentId);
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