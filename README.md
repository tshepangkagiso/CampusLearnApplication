# CampusLearn

# **Authentication & Authorization Summary**

## **🏗️ Architecture Overview**
**Microservices + API Gateway Pattern** with centralized JWT authentication.

## **🔐 The Flow**

### **1. User Management Service**
- **Purpose:** Identity Provider
- **Responsibilities:**
  - User registration & login
  - Password hashing (BCrypt)
  - JWT token generation with claims:
    ```csharp
    Claims: UserID, Email, Role (Student/Tutor/Admin), 
            Qualification, StudentNumber, RoleFlags
    ```

### **2. YARP API Gateway** 
- **Purpose:** Security Gatekeeper
- **Responsibilities:**
  - **Public routes:** `/users/authentication/*` (login/register)
  - **Protected routes:** All other routes require `"AuthorizationPolicy": "authenticated"`
  - **JWT Validation:** Verifies token signature, expiry, issuer
  - **Route forwarding:** Passes validated requests to downstream services

### **3. Downstream Services** (Topics, Forum, etc.)
- **Purpose:** Business Logic
- **Responsibilities:**
  - Trust YARP for authentication
  - Use `[Authorize]` attributes for authorization
  - Implement role-based policies:
    ```csharp
    [Authorize]                      // Any authenticated user
    [Authorize(Policy = "StudentOnly")]  // Students only
    [Authorize(Policy = "TutorOnly")]    // Tutors & Admins only
    ```

## **🛡️ Security Model**

### **Authentication** (Who are you?)
- **Handled by:** YARP Gateway
- **Method:** JWT token validation
- **Location:** Single point at gateway

### **Authorization** (What can you do?)
- **Handled by:** Individual services
- **Method:** ASP.NET Core Policy-based authorization
- **Location:** Controller/action level

## **📡 Request Journey**
```
Angular App 
  → YARP Gateway (JWT validation) 
  → Downstream Service (Policy check) 
  → Business Logic
```

## **🎯 Key Benefits**
1. **Single Source of Truth:** One JWT secret shared between UserManagement + YARP
2. **Centralized Auth Logic:** No duplicate validation code
3. **Fine-grained Control:** Services define their own authorization policies
4. **Scalable:** New services automatically inherit auth infrastructure
5. **Secure:** YARP blocks invalid tokens before reaching business logic

## **⚙️ Configuration Summary**
- **UserManagement:** Issues JWT tokens
- **YARP:** Validates JWT tokens + route protection
- **Services:** Policy enforcement via `[Authorize]` attributes

This creates a **clean, secure, and maintainable** authentication system that scales with your microservices architecture.

# Port Mapping Documentation

## External Access (from Host Machine)

| Service             | HTTP URL                  | HTTPS URL                 | Notes |
|---------------------|---------------------------|---------------------------|-------|
| **YARP Gateway**        | [http://localhost:7000](http://localhost:7000) | [https://localhost:7001](https://localhost:7001) |  |
| **User Profile**        | [http://localhost:7100](http://localhost:7100) | [https://localhost:7101](https://localhost:7101) |  |
| **Topics Management**   | [http://localhost:7200](http://localhost:7200) | [https://localhost:7201](https://localhost:7201) |  |
| **Forum Management**    | [http://localhost:7300](http://localhost:7300) | [https://localhost:7301](https://localhost:7301) |  |
| **Media Content**       | [http://localhost:7400](http://localhost:7400) | [https://localhost:7401](https://localhost:7401) |  |
| **Notifications**       | [http://localhost:7500](http://localhost:7500) | [https://localhost:7501](https://localhost:7501) |  |
| **Private Messaging**   | [http://localhost:7600](http://localhost:7600) | [https://localhost:7601](https://localhost:7601) |  |

### Supporting Services

| Service | URL / Connection String |
|----------|--------------------------|
| **Seq** | [http://localhost:5341](http://localhost:5341) |
| **Seq Management** | [http://localhost:8081](http://localhost:8081) |
| **SQL Server (User)** | `localhost,1433` |
| **SQL Server (Topics)** | `localhost,1434` |
| **SQL Server (Forum)** | `localhost,1435` |
| **RabbitMQ** | `amqp://localhost:5672` |
| **RabbitMQ Management** | [http://localhost:15672](http://localhost:15672) |
| **MinIO API** | [http://localhost:9000](http://localhost:9000) |
| **MinIO Console** | [http://localhost:9001](http://localhost:9001) |

---

## Internal Network Access (Between Containers)

| Service             | Internal URL |
|---------------------|---------------|
| **YARP Gateway**        | `http://campuslearn.yarpgateway.reverseproxy.api:8080` |
| **User Profile**        | `http://campuslearn.userprofilemanagement.api:8080` |
| **Topics Management**   | `http://campuslearn.topicsmanagement.api:8080` |
| **Forum Management**    | `http://campuslearn.forummanagement.api:8080` |
| **Media Content**       | `http://campuslearn.mediacontent.api:8080` |
| **Notifications**       | `http://campuslearn.notifications.api:8080` |
| **Private Messaging**   | `http://campuslearn.privatemessaging.api:8080` |

### Supporting Services

| Service | Connection / URL |
|----------|------------------|
| **Seq** | `http://seq:5341` |
| **SQL Server (User)** | `sqlserver.userprofile,1433` |
| **SQL Server (Topics)** | `sqlserver.topics,1433` |
| **SQL Server (Forum)** | `sqlserver.forum,1433` |
| **RabbitMQ** | `rabbitmq:5672` |
| **MinIO** | `http://minio:9000` |

---





