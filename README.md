# BUSCO API

## Description

This project is an API developed using the .NET Framework with C#. The API facilitates communication and data transfer between the client and the server. It is designed to be consumed by a client developed in Android.

## Scope

The activities planned within the system are:
- User registration
- Worker registration in the first or second instance
- Job proposal registration
- Worker search
- Job search
- Rating of worker, job, and user
- Notification of applications
- Messaging

## Technologies

- **Framework**: .NET with C#
- **Database**: MySQL
- **ORM**: Entity Framework (EF)

## Contributions

Contributions are welcome. Please open an issue or send a pull request to discuss any changes you wish to make.

## EER Diagram
![dbEER](https://github.com/user-attachments/assets/9b12d63a-e359-46cc-962e-e23613accec8)


## Endpoints

## **1. User management**

### **POST** `/users/create`

- **Description**: Register as a user
- **Parameters**:
  - `email` (string, required): User's email.
  - `username` (string, required): User's username.
  - `password` (string, required): User's password.
- **Response**:
  ```json
  {
   "token": "string",
   "expiration": "string"
  }
  ```

### **POST** `/users/login`

- **Description**: Login
- **Parameters**:
  - `email` (string, required): User's email.
  - `password` (string, required): User's password.
- **Response**:
  ```json
  {
   "token": "string",
   "expiration": "string"
  }
  ```

### **PATCH** `/users/confirm-register-code`

- **Description**: When the user registers, a confirmation email is sent to him/her. Once he/she has obtained this code, he/she must confirm/verify it.
- **Auth**: `Bearer` required
- **Parameters**:
  - `code` (int, required): Verification code.
- **Response**:
  ```json
  200 OK
  ```

### **PATCH** `/users/resend-code`

- **Description**: In case it is necessary to send the verification code again.
- **Auth**: `Bearer` required
- **Response**:
  ```json
  200 OK
  ```

### **PATCH** `/users/confirm-password-code`

- **Description**: If you want to change the password (the most useful case is to recover the password) you must confirm using a code that will be sent to the email.
- **Content-Type**: `application/x-www-form-urlencoded` or `multipart/form-data`
- **Auth**: `Bearer` required
- **Parameters**:
  - `email` (string, required): User's email.
  - `code` (int, required): Verification code.
- **Response**:
  ```json
  {
   "token": "string",
   "expiration": "number"
  }
  ```

### **PATCH** `/users/change-password`

- **Description**: To change the password.
- **Content-Type**: `application/x-www-form-urlencoded` or `multipart/form-data`
- **Auth**: `Bearer` required
- **Parameters**:
  - `password` (string, required): New password.
- **Response**:
  ```json
  200 OK
  ```

### **GET** `/users/me`

- **Description**: Gets the user who uses the authentication token.

- **Response**:
  ```json
  {
   "id": "int",
   "name": "string",
   "lastname": "string",
   "username": "string",
   "email": "string",
   "birthdate": "string",
   "country": "string",
   "province": "string",
   "department": "string",
   "city": "string",
   "image": "string",
   "verificationCode": "int or null",
   "confirmed": "boolean",
   "googleId": "int or null",
   "worker": {
    "userId": "int",
    "title": "string",
    "yearsExperience": "int",
    "webPage": "string",
    "description": "string"
   }
  ```

### **GET** `/users/{id}`

- **Description**: Get a user.

- **Response**:
  ```json
  {
   "id": "int",
   "name": "string",
   "lastname": "string",
   "username": "string",
   "email": "string",
   "birthdate": "string",
   "country": "string",
   "province": "string",
   "department": "string",
   "city": "string",
   "image": "string",
   "verificationCode": "int or null",
   "confirmed": "boolean",
   "googleId": "int or null",
   "worker": {
    "userId": "int",
    "title": "string",
    "yearsExperience": "int",
    "webPage": "string",
    "description": "string",
    "workersProfessions": "List of Professions"
   }
  ```

### **PUT** `/users`

- **Description**: Update user, no user ID is required as it is taken by the authentication token.
- **Auth**: `Bearer` required
- **Parameters**:
  - `Name` (string, required)
  - `Lastname` (string, required)
  - `Birthdate` (string, required)
  - `Country` (string, required)
  - `Province` (string, required)
  - `Department` (string, required)
  - `Telephone` (string, required)
- **Response**:
  ```json
    204 No Content
  ```

### **PATCH** `/users/me/image`

- **Description**: If you want to change the password (the most useful case is to recover the password) you must confirm using a code that will be sent to the email.
- **Content-Type**: `multipart/form-data`
- **Auth**: `Bearer` required
- **Parameters**:
  - `image` (file, required): Image file.
- **Response**:
  ```json
  {
   "image": "string"
  }
  ```

## **2. Professions and categories**

### **GET** `/professions/categories`

- **Description**: Get all categories.

- **Response**:
  ```json
   [
    {
  	"id": "int",
  	"name": "string"
    },
    ...
   ]
  ```

### **GET** `/professions/categories/{1}`

- **Description**: Get professions by category.

- **Response**:

  ```json
    {
  	"id": "int",
  	"name": "string",
        "professions":[
        {
            "id": "int",
            "name": "string",
            "description": "string"
            },
            ...
        ]
  }
  ```

### **GET** `/professions/{1}`

- **Description**: Get A profession.

- **Response**:
  ```json
  {
   "id": "int",
   "name": "string",
   "description": "string",
   "categoryId": "int",
   "category": {
    "id": "int",
    "name": "string"
   }
  }
  ```

### **GET** `/professions/search?query={value}`

- **Description**: Filter professions by query.

- **Response**:
  ```json
  [
   {
    "id": "int",
    "name": "string",
    "description": "string",
    "categoryId": "int"
   },
   ...
  ]
  ```

## **3. Workers**

### **POST** `/workers`

- **Description**: Registers the user as an worker.
- **Auth**: `Bearer` required
- **Parameters**:
  - `Title` (string, required)
  - `YearsExperience` (int, required)
  - `ProfessionsId` (list (int), required)
  - `WebPage` (string, required)
  - `Description` (string, required)
- **Response**:
  ```json
  200 OK
  ```

### **PUT** `/workers`

- **Description**: Update worker data.
- **Auth**: `Bearer` required
- **Parameters**:
  - `Title` (string, required)
  - `YearsExperience` (int, required)
  - `ProfessionsId` (list (int), required)
  - `WebPage` (string, required)
  - `Description` (string, required)
- **Response**:
  ```json
  204 No Content
  ```

### **GET** `/workers/recommendations?page={number}&NumberRecordsPerPage={number}`

- **Description**: Brings personalized worker recommendations..
- **Auth**: `Bearer` required
- **Response**:
  ```json
  [
   {
    "id": "int",
    "name": "string",
    "lastname": "string",
    "username": "string",
    "email": "string",
    "birthdate": "string",
    "country": "string",
    "province": "string",
    "department": "string",
    "city": "string",
    "image": "string",
    "verificationCode": "int",
    "confirmed": "boolean",
    "googleId": "int",
    "worker": {
     "userId": "int",
     "title": "string",
     "yearsExperience": "int",
     "webPage": "string",
     "description": "string",
     "workersProfessions": [
      {
       "workerId": "int",
       "professionId": "int",
       "profession": {
        "id": "int",
        "name": "string",
        "description": "string",
        "categoryId": "int"
       }
      }
     ],
     "user": {
      "id": "int",
      "name": "string",
      "lastname": "string",
      "image": "string"
     },
     "averageQualification": "double"
    }
   }
  ]
  ```

## **4. Proposals**

### **POST** `/proposals`

- **Description**: Create job proposal.
- **Content-Type**: `multipart/form-data`
- **Auth**: `Bearer` required
- **Parameters**:
  - `Title` (string, required)
  - `Description` (int, required)
  - `Requirements` (list (int), required)
  - `MinBudget` (int, required)
  - `MaxBudget` (int, required)
  - `Image` (File, required): Image file
  - `ProfessionId` (int, required)
- **Response**:
  ```json
  {
   "id": "int"
  }
  ```

### **PUT** `/proposals/{id}`

- **Description**: Update job proposal.
- **Content-Type**: `multipart/form-data`
- **Auth**: `Bearer` required
- **Parameters**:
  - `Title` (string, required)
  - `Description` (int, required)
  - `Requirements` (list (int), required)
  - `MinBudget` (int, required): Minimum price that the user is willing to pay.
  - `MaxBudget` (int, required): Maximum price that the user is willing to pay.
  - `Image` (File, required): Image file
  - `ProfessionId` (int, required)
- **Response**:
  ```json
  204 No Content
  ```

### **DELETE** `/proposals/{id}`

- **Description**: Delete job proposal.
- **Auth**: `Bearer` required
- **Response**:
  ```json
  204 No Content
  ```

### **PATCH** `/proposals/{id}/finalize`

- **Description**: Delete job proposal.
- **Auth**: `Bearer` required
- **Response**:
  ```json
  204 No Content
  ```

### **GET** `/proposals/{id}`

- **Description**: Get job proposal.
- **Response**:
  ```json
  {
   "id": "int",
   "title": "string",
   "description": "string",
   "requirements": "string",
   "date": "string",
   "minBudget": "int",
   "maxBudget": "int",
   "image": "string",
   "status": "boolean or null",
   "userId": 60,
   "professionId": 3,
   "limitDate": "string",
   "finishDate": "string"
  }
  ```

### **GET** `/proposals/recommendations?page={number}&NumberRecordsPerPage={number}`

- **Description**: Get proposal recommendations, personalized algorithm based on user preferences and data.
- **Auth**: `Bearer` required
- **Response**:
  ```json
  [
   {
    "id": "int",
    "title": "string",
    "description": "string",
    "requirements": "string",
    "date": "string",
    "minBudget": "int",
    "maxBudget": "int",
    "image": "string",
    "status": "boolean",
    "user": {
     "id": "int",
     "name": "string",
     "lastname": "string",
     "username": "string",
     "email": "string",
     "country": "string",
     "province": "string",
     "department": "string",
     "city": "string",
     "image": "string"
    }
   }
  ]
  ```

### **GET** `/proposals/all/{user-id}?NumberRecordsPerPage={number}&Page={number}&status={boolean}`

- **Clarification**: Status is optional.
- **Description**: Get proposals from a user.
- **Response**:
  ```json
  [
   {
    "id": "int",
    "title": "string",
    "description": "string",
    "requirements": "string",
    "date": "string",
    "minBudget": "int",
    "maxBudget": "int",
    "image": "string",
    "status": "boolean"
   },
   ...
  ]
  ```

## **5. Geographical data of Argentina**

- **Clarification**: URL without "api/"
- **Clarification**: I use API of the Geographic Data Standardization Service of Argentina. It would be the same if you use it from its domain, I included it in the code of this api as a matter of personal taste.

- **Link**: https://datosgobar.github.io/georef-ar-api/

### **GET** `/Geo/provincias`

- **Description**: Get the provinces of Argentina.
- **Response**:
  ```json
  [
   {
  	"id": "int",
  	"nombre": "string"
  },,
   ...
  ]
  ```

### **GET** `/Geo/departamentos/{province}`

- **Description**: Obtain the departments of a given province.
- **Response**:
  ```json
  [
   {
  	"id": "int",
  	"nombre": "string"
  },,
   ...
  ]
  ```

### **GET** `/Geo/ciudades/{province}/{department}`

- **Description**: Obtain the cities of a given department.
- **Response**:
  ```json
  [
   {
  	"id": "int",
  	"nombre": "string"
  },,
   ...
  ]
  ```

## **4. Applications**

### **POST** `/applications/{proposalId}`

- **Description**: Apply for job proposal.
- **Auth**: `Bearer` required
- **Response**:
  ```json
  200 OK
  ```

### **DELETE** `/applications/{proposalId}`

- **Description**: Suppress the request for proposal for employment.
- **Auth**: `Bearer` required
- **Response**:
  ```json
  204 No Content
  ```

### **PATCH** `/applications/{proposalId}/{applicationId}?status={true/false}`

- **Description**: Accept or reject the application.
- **Auth**: `Bearer` required
- **Response**:
  ```json
  204 No Content
  ```

### **GET** `/applications/{proposalId}`

- **Description**: Obtain the applications of a proposal.
- **Auth**: `Bearer` required
- **Response**:
  ```json
  [
   {
    "id": "int",
    "workerUserId": "int",
    "date": "string",
    "status": "boolean",
    "worker": {
     "userId": "int",
     "title": "string",
     "user": {
      "id": "int",
      "name": "string",
      "lastname": "string",
      "image": "string"
     }
    }
   },
   ...
  ]
  ```

### **GET** `/applications/{proposalId}/accepted`

- **Description**: Bring the accepted application for the employment proposal, if it exists.
- **Auth**: `Bearer` required
- **Response**:
  ```json
  {
   "id": "int",
   "workerUserId": "int",
   "date": "string",
   "status": "boolean",
   "worker": {
    "userId": "int",
    "title": "string",
    "user": {
     "id": "int",
     "name": "string",
     "lastname": "string",
     "image": "string"
    }
   }
  }
  ```

### **GET** `/applications/me`

- **Description**: Get applications to user's job proposals by their token.
- **Auth**: `Bearer` required
- **Response**:
  ```json
  [
   {
    "id": "int",
    "workerUserId": "int",
    "proposalId": "int",
    "date": "string",
    "status": "boolean",
    "proposal": {
     "id": "int",
     "title": "string",
     "description": "string",
     "requirements": "string",
     "date": "string",
     "minBudget": "int",
     "maxBudget": "int",
     "image": "string",
     "status": "boolean"
    }
   }
  ]
  ```

## **5. Jobs**

- **Clarification**: The jobs are the applications that have been accepted.

### **GET** `/jobs?finished={true/false}`

- **Description**: Get the user's jobs, you can filter by completed or not.
- **Auth**: `Bearer` required
- **Response**:

  ```json
  [
   {
    "id": "int",
    "title": "string",
    "description": "string",
    "requirements": "string",
    "date": "string",
    "minBudget": "int",
    "maxBudget": "int",
    "image": "string",
    "status": "boolean"
   },
   ...
  ]
  ```

### **GET** `/jobs/{userId}/completed`

- **Description**: Get the jobs that a user has completed.
- **Response**:
  ```json
  [
   {
    "id": "int",
    "title": "string",
    "description": "string",
    "requirements": "string",
    "date": "string",
    "minBudget": "int",
    "maxBudget": "int",
    "image": "string",
    "status": "boolean"
   },
   ...
  ]
  ```

## **6. Qualifications**

### **POST** `/qualifications`

- **Description**: Create a qualification for a user.
- **Auth**: `Bearer` required
- **Parameters**:
  - `Score` (int, required): From 1 to 5.
  - `Commentary` (string, required)
  - `WorkerUserId` (int, required):Id of the User to be qualified.
- **Response**:
  ```json
  204 No Content
  ```

### **GET** `/qualifications/{workerId}?stars={number}&page={number}&NumberRecordsPerPage={number}`

- **Description**: Get a user's ratings by user id, ratingFrequencies appears if you have at least one qualification.
- **Response**:
  ```json
  {
   "quantity": "int",
   "average": "double",
   "ratingFrequencies": {
    "1": "int",
    "2": "int",
    "3": "int",
    "4": "int",
    "5": "int"
   },
   "qualifications": [
    {
     "id": "int",
     "Score": "string",
     "Commentary": "string",
     "Date": "string",
     "UserId": "int",
     "WorkerUserId": "int",
     "User": {
      "id": "int",
      "name": "string",
      "lastname": "string",
      "username": "string",
      "email": "string",
      "birthdate": "string",
      "country": "string",
      "province": "string",
      "department": "string",
      "city": "string",
      "image": "string"
     }
    },
    ...
   ]
  }
  ```

## **7. Search**

### **GET** `/proposals/search?query={value}`

- **Description**: Search proposals.
- **Auth**: `Bearer` required
- **Queries**:
  - `query` (string, required)
  - `province` (string, optional)
  - `city` (string, optional)
  - `page` (string, optional)
  - `NumberRecordsPerPage` (string, optional)
  - `filterCategoryId` (int, optional)
- **Response**:
  ```json
  [
   {
    "id": "int",
    "title": "string",
    "description": "string",
    "requirements": "string",
    "date": "string",
    "minBudget": "int",
    "maxBudget": "int",
    "image": "string",
    "status": "boolean",
    "userId": "int",
    "professionId": "int",
    "profession": {
     "id": "int",
     "name": "string",
     "description": "string",
     "categoryId": "int"
    }
   },
   ...
  ]
  ```

### **GET** `/workers/search?query={value}`

- **Description**: Search proposals.
- **Auth**: `Bearer` required
- **Queries**:
  - `query` (string, required)
  - `province` (string, optional)
  - `city` (string, optional)
  - `page` (string, optional)
  - `NumberRecordsPerPage` (string, optional)
  - `filterCategoryId` (int, optional)
  - `filterQualification` (int, optional)
- **Response**:
  ```json
  [
   {
    "userId": "int",
    "title": "string",
    "yearsExperience": "int",
    "webPage": "string",
    "description": "string",
    "averageQualification": "float",
    "user": {
     "id": "int",
     "name": "string",
     "lastname": "string",
     "image": "string"
    }
   },
   ...
  ]
  ```

## **8. Notifications**

### **GET** `/notifications`

- **Description**: Get notifications.
- **Auth**: `Bearer` required
- **Response**:
  ```json
  [
   {
    "id": "int",
    "userReceiveId": "int",
    "userSenderId": "int",
    "dateAndTime": "string",
    "text": "string",
    "userSender": {
     "id": "int",
     "name": "string",
     "lastname": "string",
     "image": "string"
    },
    "proposalId": "int",
    "proposal": {
     "id": "int",
     "title": "string",
     "description": "string",
     "requirements": "string",
     "date": "string",
     "minBudget": "int",
     "maxBudget": "int",
     "image": "string",
     "status": "boolean or null",
     "userId": "int",
     "professionId": "int",
     "limitDate": "string",
     "finishDate": "string"
    }
   },
   ...
  ]
  ```

  ## Author

- [@danielbarros01](https://www.github.com/danielbarros01)

