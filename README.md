# 🎓 CampusLearn™ - Complete System Overview

## 🏗️ System Architecture

### Frontend Application
- **Angular WebApp Client**  
  - Modern responsive web interface  
  - Built with Angular 17 + TypeScript  
  - Role-based dashboards (Student, Tutor, Admin)  
  - Integrated with YARP Gateway API for unified backend access  
  - Real-time UI updates via SignalR  
  - Secure JWT-based authentication flow  
  

### Microservices Architecture (7 APIs)
- **User Profile Management API** - Core authentication & user management  
- **Topics Management API** - Tutor-led learning & Q&A system  
- **Forum Management API** - Public discussion forum  
- **Private Messaging API** - Real-time 1-on-1 communication  
- **Chatbot API** - AI-powered academic assistant  
- **Notifications API** - Event-driven messaging system  
- **YARP Gateway API** - Unified API gateway & reverse proxy  

### Database Architecture (4 Databases)
- **UserProfileDb** - User accounts, profiles, module subscriptions  
- **TopicsDb** - Tutor topics, FAQs, query responses  
- **ForumDb** - Public forum topics and discussions  
- **PrivateMessagesDb** - Private chat rooms and messages  

### Infrastructure Services
- **SQL Server Azure Edge** - Primary database engine  
- **RabbitMQ** - Message broker for event-driven communication  
- **MinIO** - Object storage for files/media  
- **Seq** - Centralized logging & monitoring  
- **n8n** - Workflow automation for AI agent  

---
<img width="575" height="804" alt="Image" src="https://github.com/user-attachments/assets/c402ff34-bd18-4cc0-abf9-ee080a9e58db" />

## 🔧 Core Functional Modules

### 1. Authentication & User Management ✅
- Student/Tutor registration with campus email validation  
- JWT-based authentication with role-based access  
- Password management with secure hashing  
- Profile management with image uploads  

### 2. Peer Tutor & Topic System ✅
- Tutors create and manage learning topics  
- Students subscribe to topics/modules  
- Q&A system with multimedia responses  
- FAQ management by tutors  

### 3. Public Discussion Forum ✅
- Anonymous posting capability  
- Upvoting system for topics/responses  
- Module-based categorization  
- Admin moderation tools (pin/lock topics)  

### 4. Private Messaging ✅
- Automatic student-tutor matching  
- Real-time messaging via SignalR  
- Secure 1-on-1 academic support  
- File sharing capabilities  

### 5. AI-Powered Chatbot ✅
- 24/7 academic support via DeepSeek AI  
- n8n + Gemini AI automation agent  
- Smart escalation to human tutors  
- FAQ handling and study guidance  

### 6. Notification System ✅
- RabbitMQ event-driven notifications  
- Real-time alerts for new topics/queries  
- Forum activity notifications  
- Queue-based message processing  

---

## 🚀 Technical Implementation

### Security Features
- JWT tokens with secret key encryption  
- Campus email validation (@belgiumcampus.ac.za)  
- Password hashing with salt  
- Role-based authorization (Student/Tutor/Admin)  
- File upload validation & restrictions  

### Real-Time Capabilities
- SignalR WebSocket connections for live chat  
- RabbitMQ for async message processing  
- Live notifications for forum/topic activities  

### File Management
- MinIO object storage for profile pictures  
- Media content support (PDFs, videos, images)  
- Secure file retrieval with content typing  

### Containerization
- Docker Compose for local development  
- Health checks for database dependencies  
- Network isolation between services  
- Port mapping for external access  

---

## 📊 API Endpoints Structure

**Gateway Routes (http://localhost:7000)**  
- `authentication/*` → Auth endpoints  
- `users/*` → User profile management  
- `topics/*` → Tutor topics & Q&A  
- `forums/*` → Public discussions  
- `messages/*` → Private messaging  
- `notifications/*` → Notification queues  
- `agent/*` → Chatbot services  

---

## 🎯 Business Goals Achieved

### For Students
✅ Flexible, accessible academic support  
✅ Peer-to-peer learning opportunities  
✅ Anonymous help-seeking in forums  
✅ 24/7 AI tutor assistance  
✅ Personalized tutor matching  

### For Tutors
✅ Platform to share expertise  
✅ Manage topics and FAQs  
✅ Respond to student queries  
✅ Upload learning materials  
✅ Build teaching portfolio  

### For Institution
✅ Enhanced student learning experience  
✅ Bridge between lectures and individual support  
✅ Scalable, maintainable microservices architecture  
✅ Modern technology stack ready for production  

---

## 🔄 Data Flow & Integration
- **User Registration** → UserProfileDb + Authentication  
- **Topic Creation** → TopicsDb + RabbitMQ notifications  
- **Forum Posts** → ForumDb + Real-time updates  
- **Private Messages** → PrivateMessagesDb + SignalR  
- **AI Queries** → Chatbot API + n8n workflows  
- **File Uploads** → MinIO + Database references  

---

## 📈 Deployment Ready
The system is fully containerized with:

- Health-checked dependencies  
- Environment configuration  
- Centralized logging  
- Scalable microservices  
- Production-ready security  


# 🧭 Port Mapping Documentation

## 🌐 External Access (from Host Machine)

### Core Application APIs

| Service | HTTP URL | Container Port | Purpose |
|----------|-----------|----------------|----------|
| **YARP Gateway** | http://localhost:7000 | 8080 | Main API Gateway |
| **User Profile API** | http://localhost:7100 | 8080 | Authentication & User Management |
| **Topics Management API** | http://localhost:7200 | 8080 | Tutor Topics & Q&A System |
| **Forum Management API** | http://localhost:7300 | 8080 | Public Discussion Forum |
| **Notifications API** | http://localhost:7400 | 8080 | Event Notifications |
| **Private Messaging API** | http://localhost:7500 | 8080 | Real-time Chat |
| **Chatbot API** | http://localhost:7600 | 8080 | AI Assistant |

### Infrastructure & Supporting Services

| Service | URL / Connection | Credentials | Purpose |
|----------|------------------|-------------|----------|
| **Seq Logging** | http://localhost:5341 | admin/admin | Centralized Logging |
| **SQL Server** | localhost,1435 | sa/Password123! | Primary Database |
| **RabbitMQ Management** | http://localhost:15672 | guest/guest | Message Queue Dashboard |
| **RabbitMQ API** | amqp://localhost:5672 | guest/guest | Message Broker |
| **MinIO Console** | http://localhost:9001 | minioadmin/minioadmin123 | File Storage UI |
| **MinIO API** | http://localhost:9000 | minioadmin/minioadmin123 | File Storage API |
| **n8n Workflows** | http://localhost:5678 | - | AI Automation |

---

## 🔗 Internal Network Access (Between Containers)

### Application Services

| Service | Internal URL | Database |
|----------|---------------|-----------|
| **YARP Gateway** | http://campuslearn.yarpgateway.reverseproxy.api:8080 | - |
| **User Profile API** | http://campuslearn.userprofilemanagement.api:8080 | UserProfileDb |
| **Topics Management API** | http://campuslearn.topicsmanagement.api:8080 | TopicsDb |
| **Forum Management API** | http://campuslearn.forummanagement.api:8080 | ForumDb |
| **Notifications API** | http://campuslearn.notifications.api:8080 | - |
| **Private Messaging API** | http://campuslearn.privatemessaging.api:8080 | PrivateMessagesDb |
| **Chatbot API** | http://campuslearn.chatbot.api:8080 | - |

### Infrastructure Services

| Service | Internal Connection | Purpose |
|----------|---------------------|----------|
| **Seq** | http://seq:80 | Logging |
| **SQL Server** | sqlserver:1433 | All Databases |
| **RabbitMQ** | amqp://rabbitmq:5672 | Message Broker |
| **MinIO** | http://minio:9000 | File Storage |
| **n8n** | http://n8n:5678 | AI Workflows |

---

## 🗄️ Database Connection Details

### Single SQL Server Instance - 4 Databases
- **Server:** sqlserver:1433 (internal) / localhost,1435 (external)  

**Databases:**
- **UserProfileDb** - User accounts & profiles  
- **TopicsDb** - Tutor topics & Q&A  
- **ForumDb** - Public discussions  
- **PrivateMessagesDb** - Chat messages  

---

## 📡 API Gateway Routing

### External Access via YARP (http://localhost:7000)

| Route | Maps To | Service |
|--------|----------|----------|
| `authentication/{**catch-all}` | User Profile API | Auth & Registration |
| `users/{**catch-all}` | User Profile API | Profile Management |
| `topics/{**catch-all}` | Topics Management API | Tutor Q&A System |
| `forums/{**catch-all}` | Forum Management API | Public Discussions |
| `messages/{**catch-all}` | Private Messaging API | Real-time Chat |
| `notifications/{**catch-all}` | Notifications API | Event Notifications |
| `agent/{**catch-all}` | Chatbot API | AI Assistant |

---

## 🔄 Key Integration Points

### SignalR Real-time Connections
- **Chat Hub:** http://localhost:7500/chatHub (via Private Messaging API)  
- **WebSocket:** Automatic connection through SignalR  

### File Upload/Download
- **Upload:** Via multipart/form-data to respective APIs  
- **Download:** http://localhost:7X00/file/{fileName} pattern  
- **Storage:** MinIO object storage (http://localhost:9001)  

### Event Flow
- **Topic Created** → Topics API → RabbitMQ → Notifications API  
- **Forum Post** → Forum API → RabbitMQ → Notifications API  
- **Message Sent** → Private Messaging API → SignalR → Real-time delivery  

---

## 🚀 Quick Start Access

### For Development:
- **Main Gateway:** http://localhost:7000  
- **Database:** SSMS to localhost,1435  
- **Logs:** http://localhost:5341  
- **Files:** http://localhost:9001  
- **Messages:** http://localhost:15672  

### For Monitoring:
- **Logs:** Seq at localhost:5341  
- **Queues:** RabbitMQ at localhost:15672  
- **Storage:** MinIO at localhost:9001  
- **Workflows:** n8n at localhost:5678  

---

All services are accessible through the **Docker network** `campuslearn-network` with **health-checked dependencies**.






