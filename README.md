Minimum System Requirements:
****************************

Operating System - Windows 8.1, 10 or 11
Application - Visual Studio 2022
            - WampServer64
            - SQL Server Management Studio
            - Postman (for testing the API)


Minimum Hardware requirements:
******************************

CPU/PROCESSOR --> 1.8 GHz or faster processor. Quad-core or better recommended

RAM --> 2 GB of RAM; 8 GB of RAM recommended (2.5 GB minimum if running on a virtual machine)

HARD DISK DRIVE(HDD) STORAGE --> Minimum of 800MB up to 210 GB of available hard disk space, depending on features installed; typical installations require 20-50 GB of free space of hard disk space

SOLID STATE DRIVE(SSD) STORAGE --> To improve performance, install Windows and Visual Studio on a solid state drive

RESOLUTION --> Video card that supports a minimum display resolution of 720p (1280 by 720); Visual Studio will work best at a resolution of WXGA (1366 by 768) or higher


 ***********************************************
             SECTION 01
 ***********************************************


How To Run The Application:
****************************
0. Download the application from the github repository: https://github.com/V1ckyM4k/ToDoListApi
1. (In File Explorer) Unzip the folder "ToDoListApi"
2. Open Visual Studio 2022
3. In Visual Studio, click on "Open a local folder" and find the project in the respected folder and open it
4. Configure values from the .env.example file from the repo in appsettings.json
5. Open SQL Server Management Studio.
6.  Create an empty database (e.g. TodoApi).
7.  Open TodoApiDump.sql and execute it against that database.
8.  Click on the highlighted play button with the project name at the top the application to debug and run the application
9.  Navigate to: https://localhost:7083/swagger
    - Use Swagger UI to: See all routes, View request/response schemas, and Test endpoints.

 ***********************************************
             Application Overview
 ***********************************************
The application is a user-based ToDo List Api developed using Asp.Net Core Web Api, all data is stored in a Sql Server Database, and Swagger for building and documenting the project's APIs.
It allows users to:
- Register, Login and Delete their account.
- Create, Read, Update and Delete Tasks.
- Upload csv files of Tasks at.

 ***********************************************
             API Endpoints Summaries
 ***********************************************
### Authentication: ###
- /api/Auth/register
  - POST : User enters their details to store their credentials in the db
- /api/Auth/login
  - POST : User enters their email and password and provides a jwt token which will e used to authorize them(copy the jwt into the authorize field on swagger)
- /api/Auth/delete
  - DELETE : User enters their email to delete their account
    
### ToDo: ###
- /api/Tasks/task
  - GET :  User enters task ID to retrieve a task from tasks table
  - POST :  User enters task details to store a new task into the tasks table
  - DELETE : User enters task ID to delete a task from tasks table
  
- /api/Tasks/tasks
  - GET : retrieves all the user's tasks 
  - POST : allows users to import csv file of tasks

 
