# Rebuild-Docker Script

- Stops all running containers, cleans the .NET solution, rebuilds all projects, and rebuilds Docker images.  
- Starts all services (APIs, YARP Gateway, SQL Edge, RabbitMQ, MinIO, Seq) fresh in detached mode.  
- Run the script from the solution root using PowerShell: `.\Rebuild-Docker.ps1`.


# External Host → Container
YARP Gateway       : http://localhost:7000  https://localhost:7001
UserProfile API    : http://localhost:7100  https://localhost:7101
Topics API         : http://localhost:7200  https://localhost:7201
Forum API          : http://localhost:7300  https://localhost:7301
MediaContent API   : http://localhost:7400  https://localhost:7401
Notifications API  : http://localhost:7500  https://localhost:7501
PrivateMessaging API: http://localhost:7600 https://localhost:7601
Seq UI             : http://localhost:5341
SQL Server         : localhost,1435
RabbitMQ AMQP      : amqp://localhost:5672
RabbitMQ Mgmt      : http://localhost:15672
MinIO API          : http://localhost:9000
MinIO Console      : http://localhost:9001

# Internal (container-to-container)
YARP Gateway       : http://campuslearn.yarpgateway.reverseproxy.api:8080
UserProfile API    : http://campuslearn.userprofilemanagement.api:8080
Topics API         : http://campuslearn.topicsmanagement.api:8080
Forum API          : http://campuslearn.forummanagement.api:8080
MediaContent API   : http://campuslearn.mediacontent.api:8080
Notifications API  : http://campuslearn.notifications.api:8080
PrivateMessaging API: http://campuslearn.privatemessaging.api:8080
Seq UI             : http://seq:80
SQL Server         : sqlserver:1433
RabbitMQ AMQP      : amqp://rabbitmq:5672 - amqp://guest:guest@rabbitmq:5672
MinIO API          : http://minio:9000



## APIs via Localhost

- [User Profile Management](http://localhost:6000)  
- [Topics Management](http://localhost:6100)  
- [Forum Management](http://localhost:6200)  
- [Notifications](http://localhost:6300)  
- [Media Content](http://localhost:6400)  
- [Private Messaging](http://localhost:6500)  
- [YARP Gateway](http://localhost:6600)


 # Run following docker commands on cmd:
 
    1) userprofiledb:
    docker run --restart unless-stopped -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Password123!" -p 1435:1433 --name sqlserver.userprofile -d mcr.microsoft.com/mssql/server:2025-latest

    2) topicsdb:
    docker run --restart unless-stopped -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Password123!" -p 1436:1433 --name sqlserver.topics -d mcr.microsoft.com/mssql/server:2025-latest

    3) forumdb:
    docker run --restart unless-stopped -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Password123!" -p 1438:1433 --name sqlserver.forum -d mcr.microsoft.com/mssql/server:2025-latest

    4) seq:
    docker run -d --name seq --restart unless-stopped -e ACCEPT_EULA=Y -e SEQ_PASSWORD=admin -p 5341:80 datalust/seq:latest

    5) minio:
    docker run --restart unless-stopped -d --name minio -p 9000:9000 -p 9001:9001 -e MINIO_ROOT_USER=minioadmin -e MINIO_ROOT_PASSWORD=minioadmin123 minio/minio:latest server /data --console-address ":9001"

    6) rabbitmq:
    docker run -d --name rabbitmq --restart unless-stopped -p 5672:5672 -p 15672:15672 -e RABBITMQ_DEFAULT_USER=guest -e RABBITMQ_DEFAULT_PASS=guest rabbitmq:3-management
 
    7) azure-sql-edge (single server and mutlipe dbs):
    docker run --name campuslearn-sql -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password123!" -p 1435:1433 -v campuslearn_sql_data:/var/opt/mssql --restart unless-stopped -d mcr.microsoft.com/azure-sql-edge:latest


