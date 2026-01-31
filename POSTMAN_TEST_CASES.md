# Postman Test Cases - FU News Management System API

## Prerequisites
1. Run the seed data endpoint first: `POST /api/system/seed`
2. Base URL: `http://localhost:5000` (your API URL)
3. Authentication: Most endpoints require JWT token in Authorization header (Bearer Token)

## Test Data from Seed
- **Admin Account**: admin@funews.edu.vn / Admin123!
- **Staff Account**: staff@funews.edu.vn / Staff123!
- **Lecturer Account**: john.lecturer@funews.edu.vn / Lecturer123!
- **Categories**: Academic (ID: 1), Student Life (ID: 2), Research (ID: 3), Sports (ID: 4), Technology (ID: 5)
- **Tags**: announcement, deadline, event, scholarship, graduation, exam, research, innovation, competition, international

---

## 1. Authentication Controller (`/api/auth`)

### 1.1 POST /api/auth/register

#### TC-AUTH-REG-001: Register - Success
- **Method**: POST
- **Endpoint**: `/api/auth/register`
- **Description**: Successfully register a new user
- **Authorization**: None (AllowAnonymous)
- **Request Body**:
```json
{
  "accountName": "Test User",
  "email": "testuser@funews.edu.vn",
  "password": "Test123!",
  "accountRole": 0
}
```
- **Expected Status**: 200 OK
- **Expected Response**:
```json
{
  "data": {
    "accountID": 6,
    "accountName": "Test User",
    "accountEmail": "testuser@funews.edu.vn",
    "accountRole": 0
  },
  "statusCode": "200",
  "message": "User registered successfully.",
  "isSuccess": true
}
```

#### TC-AUTH-REG-002: Register - Fail (Duplicate Email)
- **Method**: POST
- **Endpoint**: `/api/auth/register`
- **Description**: Fail to register with existing email
- **Authorization**: None
- **Request Body**:
```json
{
  "accountName": "Duplicate User",
  "email": "admin@funews.edu.vn",
  "password": "Test123!",
  "accountRole": 0
}
```
- **Expected Status**: 409 Conflict
- **Expected Response**: Error message about email already exists

#### TC-AUTH-REG-003: Register - Invalid (Missing Required Fields)
- **Method**: POST
- **Endpoint**: `/api/auth/register`
- **Description**: Fail to register with invalid data
- **Authorization**: None
- **Request Body**:
```json
{
  "accountName": "",
  "email": "invalid-email",
  "password": "123",
  "accountRole": 0
}
```
- **Expected Status**: 400 Bad Request
- **Expected Response**: Validation errors

---

### 1.2 POST /api/auth/login

#### TC-AUTH-LOGIN-001: Login - Success
- **Method**: POST
- **Endpoint**: `/api/auth/login`
- **Description**: Successfully login with valid credentials
- **Authorization**: None
- **Request Body**:
```json
{
  "email": "admin@funews.edu.vn",
  "password": "Admin123!"
}
```
- **Expected Status**: 200 OK
- **Expected Response**:
```json
{
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  },
  "statusCode": "200",
  "message": "Login successful.",
  "isSuccess": true
}
```
- **Note**: Save the accessToken for subsequent requests

#### TC-AUTH-LOGIN-002: Login - Fail (Invalid Credentials)
- **Method**: POST
- **Endpoint**: `/api/auth/login`
- **Description**: Fail to login with wrong password
- **Authorization**: None
- **Request Body**:
```json
{
  "email": "admin@funews.edu.vn",
  "password": "WrongPassword123!"
}
```
- **Expected Status**: 401 Unauthorized
- **Expected Response**: Error message about invalid credentials

#### TC-AUTH-LOGIN-003: Login - Invalid (Malformed Email)
- **Method**: POST
- **Endpoint**: `/api/auth/login`
- **Description**: Fail to login with invalid email format
- **Authorization**: None
- **Request Body**:
```json
{
  "email": "not-an-email",
  "password": "Admin123!"
}
```
- **Expected Status**: 400 Bad Request
- **Expected Response**: Validation error

---

## 2. Account Controller (`/api/accounts`)

### 2.1 GET /api/accounts/{id}

#### TC-ACC-GETID-001: Get Account by ID - Success
- **Method**: GET
- **Endpoint**: `/api/accounts/1`
- **Description**: Successfully retrieve account by ID
- **Authorization**: Bearer {token}
- **Request Body**: None
- **Expected Status**: 200 OK
- **Expected Response**:
```json
{
  "data": {
    "accountID": 1,
    "accountName": "System Admin",
    "accountEmail": "admin@funews.edu.vn",
    "accountRole": 1,
    "accountRoleName": "Admin"
  },
  "statusCode": "200",
  "message": "Account retrieved successfully.",
  "isSuccess": true
}
```

#### TC-ACC-GETID-002: Get Account by ID - Fail (Not Found)
- **Method**: GET
- **Endpoint**: `/api/accounts/9999`
- **Description**: Fail to retrieve non-existent account
- **Authorization**: Bearer {token}
- **Request Body**: None
- **Expected Status**: 404 Not Found
- **Expected Response**: Error message about account not found

#### TC-ACC-GETID-003: Get Account by ID - Invalid (No Authorization)
- **Method**: GET
- **Endpoint**: `/api/accounts/1`
- **Description**: Fail to retrieve account without authentication
- **Authorization**: None
- **Request Body**: None
- **Expected Status**: 401 Unauthorized
- **Expected Response**: Unauthorized error

---

### 2.2 GET /api/accounts

#### TC-ACC-LIST-001: Get Accounts - Success (All Accounts)
- **Method**: GET
- **Endpoint**: `/api/accounts`
- **Description**: Retrieve all accounts with default pagination
- **Authorization**: Bearer {token}
- **Query Parameters**: None
- **Expected Status**: 200 OK
- **Expected Response**: Paginated list of accounts

#### TC-ACC-LIST-002: Get Accounts - Success (With Pagination)
- **Method**: GET
- **Endpoint**: `/api/accounts?page=1&pageSize=2`
- **Description**: Retrieve accounts with custom page size
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - page=1
  - pageSize=2
- **Expected Status**: 200 OK
- **Expected Response**: 2 accounts per page

#### TC-ACC-LIST-003: Get Accounts - Success (Search by Name)
- **Method**: GET
- **Endpoint**: `/api/accounts?searchTerm=Admin`
- **Description**: Search accounts by name
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - searchTerm=Admin
- **Expected Status**: 200 OK
- **Expected Response**: Accounts matching "Admin" in name or email

#### TC-ACC-LIST-004: Get Accounts - Success (Filter by Role)
- **Method**: GET
- **Endpoint**: `/api/accounts?accountRole=1`
- **Description**: Filter accounts by admin role
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - accountRole=1
- **Expected Status**: 200 OK
- **Expected Response**: Only admin accounts

#### TC-ACC-LIST-005: Get Accounts - Success (Sort Descending)
- **Method**: GET
- **Endpoint**: `/api/accounts?isDescending=true`
- **Description**: Sort accounts in descending order
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - isDescending=true
- **Expected Status**: 200 OK
- **Expected Response**: Accounts sorted Z-A

#### TC-ACC-LIST-006: Get Accounts - Success (Combined Filters)
- **Method**: GET
- **Endpoint**: `/api/accounts?searchTerm=Staff&accountRole=2&page=1&pageSize=5&isDescending=false`
- **Description**: Combine search, filter, sort, and pagination
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - searchTerm=Staff
  - accountRole=2
  - page=1
  - pageSize=5
  - isDescending=false
- **Expected Status**: 200 OK
- **Expected Response**: Staff accounts matching search criteria

#### TC-ACC-LIST-007: Get Accounts - Success (Empty Results)
- **Method**: GET
- **Endpoint**: `/api/accounts?searchTerm=NonExistentUser`
- **Description**: Search with no matching results
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - searchTerm=NonExistentUser
- **Expected Status**: 200 OK
- **Expected Response**: Empty items array with totalItems=0

#### TC-ACC-LIST-008: Get Accounts - Invalid (Page Out of Range)
- **Method**: GET
- **Endpoint**: `/api/accounts?page=-1`
- **Description**: Request with invalid page number
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - page=-1
- **Expected Status**: 400 Bad Request
- **Expected Response**: Validation error

---

### 2.3 POST /api/accounts

#### TC-ACC-CREATE-001: Create Account - Success
- **Method**: POST
- **Endpoint**: `/api/accounts`
- **Description**: Successfully create a new account
- **Authorization**: Bearer {token} (Admin)
- **Request Body**:
```json
{
  "accountName": "New Staff Member",
  "accountEmail": "newstaff@funews.edu.vn",
  "password": "NewStaff123!",
  "accountRole": 2
}
```
- **Expected Status**: 201 Created
- **Expected Response**:
```json
{
  "data": {
    "accountID": 7,
    "accountName": "New Staff Member",
    "accountEmail": "newstaff@funews.edu.vn",
    "accountRole": 2,
    "accountRoleName": "Staff"
  },
  "statusCode": "201",
  "message": "Account created successfully.",
  "isSuccess": true
}
```

#### TC-ACC-CREATE-002: Create Account - Fail (Duplicate Email)
- **Method**: POST
- **Endpoint**: `/api/accounts`
- **Description**: Fail to create account with existing email
- **Authorization**: Bearer {token} (Admin)
- **Request Body**:
```json
{
  "accountName": "Duplicate Account",
  "accountEmail": "admin@funews.edu.vn",
  "password": "Test123!",
  "accountRole": 2
}
```
- **Expected Status**: 409 Conflict
- **Expected Response**: Error about email already exists

#### TC-ACC-CREATE-003: Create Account - Invalid (Missing Required Fields)
- **Method**: POST
- **Endpoint**: `/api/accounts`
- **Description**: Fail to create account with invalid data
- **Authorization**: Bearer {token} (Admin)
- **Request Body**:
```json
{
  "accountName": "",
  "accountEmail": "invalid-email",
  "password": "weak",
  "accountRole": 99
}
```
- **Expected Status**: 400 Bad Request
- **Expected Response**: Validation errors

---

### 2.4 PUT /api/accounts/{id}

#### TC-ACC-UPDATE-001: Update Account - Success
- **Method**: PUT
- **Endpoint**: `/api/accounts/3`
- **Description**: Successfully update an existing account
- **Authorization**: Bearer {token} (Admin)
- **Request Body**:
```json
{
  "accountName": "John Lecturer Updated",
  "accountEmail": "john.lecturer.updated@funews.edu.vn",
  "accountRole": 3
}
```
- **Expected Status**: 200 OK
- **Expected Response**:
```json
{
  "data": {
    "accountID": 3,
    "accountName": "John Lecturer Updated",
    "accountEmail": "john.lecturer.updated@funews.edu.vn",
    "accountRole": 3,
    "accountRoleName": "Lecturer"
  },
  "statusCode": "200",
  "message": "Account updated successfully.",
  "isSuccess": true
}
```

#### TC-ACC-UPDATE-002: Update Account - Fail (Not Found)
- **Method**: PUT
- **Endpoint**: `/api/accounts/9999`
- **Description**: Fail to update non-existent account
- **Authorization**: Bearer {token} (Admin)
- **Request Body**:
```json
{
  "accountName": "Updated Name",
  "accountEmail": "updated@funews.edu.vn",
  "accountRole": 2
}
```
- **Expected Status**: 404 Not Found
- **Expected Response**: Error about account not found

#### TC-ACC-UPDATE-003: Update Account - Invalid (Email Already Exists)
- **Method**: PUT
- **Endpoint**: `/api/accounts/3`
- **Description**: Fail to update with email that belongs to another account
- **Authorization**: Bearer {token} (Admin)
- **Request Body**:
```json
{
  "accountName": "John Lecturer",
  "accountEmail": "admin@funews.edu.vn",
  "accountRole": 3
}
```
- **Expected Status**: 409 Conflict
- **Expected Response**: Error about email already exists

---

### 2.5 DELETE /api/accounts/{id}

#### TC-ACC-DELETE-001: Delete Account - Success
- **Method**: DELETE
- **Endpoint**: `/api/accounts/6`
- **Description**: Successfully delete an account
- **Authorization**: Bearer {token} (Admin)
- **Request Body**: None
- **Expected Status**: 200 OK
- **Expected Response**:
```json
{
  "statusCode": "200",
  "message": "Account deleted successfully.",
  "isSuccess": true
}
```

#### TC-ACC-DELETE-002: Delete Account - Fail (Not Found)
- **Method**: DELETE
- **Endpoint**: `/api/accounts/9999`
- **Description**: Fail to delete non-existent account
- **Authorization**: Bearer {token} (Admin)
- **Request Body**: None
- **Expected Status**: 404 Not Found
- **Expected Response**: Error about account not found

#### TC-ACC-DELETE-003: Delete Account - Invalid (Unauthorized)
- **Method**: DELETE
- **Endpoint**: `/api/accounts/1`
- **Description**: Fail to delete account without proper authorization
- **Authorization**: None
- **Request Body**: None
- **Expected Status**: 401 Unauthorized
- **Expected Response**: Unauthorized error

---

## 3. Category Controller (`/api/categories`)

### 3.1 GET /api/categories/{id}

#### TC-CAT-GETID-001: Get Category by ID - Success
- **Method**: GET
- **Endpoint**: `/api/categories/1`
- **Description**: Successfully retrieve category by ID
- **Authorization**: Bearer {token}
- **Request Body**: None
- **Expected Status**: 200 OK
- **Expected Response**:
```json
{
  "categoryID": 1,
  "categoryName": "Academic",
  "categoryDescription": "Academic related news and announcements",
  "parentCategoryID": null,
  "parentCategoryName": null,
  "isActive": true,
  "subCategoriesCount": 2,
  "newsArticlesCount": 2
}
```

#### TC-CAT-GETID-002: Get Category by ID - Fail (Not Found)
- **Method**: GET
- **Endpoint**: `/api/categories/9999`
- **Description**: Fail to retrieve non-existent category
- **Authorization**: Bearer {token}
- **Request Body**: None
- **Expected Status**: 404 Not Found
- **Expected Response**: Error message about category not found

#### TC-CAT-GETID-003: Get Category by ID - Invalid (No Authorization)
- **Method**: GET
- **Endpoint**: `/api/categories/1`
- **Description**: Fail to retrieve category without authentication
- **Authorization**: None
- **Request Body**: None
- **Expected Status**: 401 Unauthorized
- **Expected Response**: Unauthorized error

---

### 3.2 GET /api/categories

#### TC-CAT-LIST-001: Get Categories - Success (All Categories)
- **Method**: GET
- **Endpoint**: `/api/categories`
- **Description**: Retrieve all categories with default pagination
- **Authorization**: Bearer {token}
- **Query Parameters**: None
- **Expected Status**: 200 OK
- **Expected Response**: Paginated list of all categories

#### TC-CAT-LIST-002: Get Categories - Success (With Pagination)
- **Method**: GET
- **Endpoint**: `/api/categories?page=1&pageSize=3`
- **Description**: Retrieve categories with custom page size
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - page=1
  - pageSize=3
- **Expected Status**: 200 OK
- **Expected Response**: 3 categories per page

#### TC-CAT-LIST-003: Get Categories - Success (Search by Name)
- **Method**: GET
- **Endpoint**: `/api/categories?searchTerm=Academic`
- **Description**: Search categories by name or description
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - searchTerm=Academic
- **Expected Status**: 200 OK
- **Expected Response**: Categories matching "Academic"

#### TC-CAT-LIST-004: Get Categories - Success (Filter by Active Status)
- **Method**: GET
- **Endpoint**: `/api/categories?isActive=true`
- **Description**: Filter only active categories
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - isActive=true
- **Expected Status**: 200 OK
- **Expected Response**: Only active categories

#### TC-CAT-LIST-005: Get Categories - Success (Filter by Parent Category)
- **Method**: GET
- **Endpoint**: `/api/categories?parentCategoryID=1`
- **Description**: Get subcategories of a parent category
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - parentCategoryID=1
- **Expected Status**: 200 OK
- **Expected Response**: Subcategories of Academic category

#### TC-CAT-LIST-006: Get Categories - Success (Include Subcategories)
- **Method**: GET
- **Endpoint**: `/api/categories?includeSubCategories=true`
- **Description**: Include subcategories in results
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - includeSubCategories=true
- **Expected Status**: 200 OK
- **Expected Response**: All categories including subcategories

#### TC-CAT-LIST-007: Get Categories - Success (Sort by Name Descending)
- **Method**: GET
- **Endpoint**: `/api/categories?sortBy=name&isDescending=true`
- **Description**: Sort categories by name in descending order
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - sortBy=name
  - isDescending=true
- **Expected Status**: 200 OK
- **Expected Response**: Categories sorted Z-A

#### TC-CAT-LIST-008: Get Categories - Success (Combined Filters)
- **Method**: GET
- **Endpoint**: `/api/categories?searchTerm=Student&isActive=true&page=1&pageSize=5`
- **Description**: Combine search, filter, and pagination
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - searchTerm=Student
  - isActive=true
  - page=1
  - pageSize=5
- **Expected Status**: 200 OK
- **Expected Response**: Active categories matching "Student"

#### TC-CAT-LIST-009: Get Categories - Success (Only Parent Categories)
- **Method**: GET
- **Endpoint**: `/api/categories?parentCategoryID=null`
- **Description**: Get only top-level categories
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - parentCategoryID=null (or omit parent filter)
- **Expected Status**: 200 OK
- **Expected Response**: Only parent categories

#### TC-CAT-LIST-010: Get Categories - Success (Empty Results)
- **Method**: GET
- **Endpoint**: `/api/categories?searchTerm=NonExistentCategory`
- **Description**: Search with no matching results
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - searchTerm=NonExistentCategory
- **Expected Status**: 200 OK
- **Expected Response**: Empty categories array

---

### 3.3 GET /api/categories/{parentId}/subcategories

#### TC-CAT-SUB-001: Get Subcategories - Success
- **Method**: GET
- **Endpoint**: `/api/categories/1/subcategories`
- **Description**: Successfully retrieve subcategories
- **Authorization**: Bearer {token}
- **Request Body**: None
- **Expected Status**: 200 OK
- **Expected Response**: List of subcategories for Academic category

#### TC-CAT-SUB-002: Get Subcategories - Fail (Parent Not Found)
- **Method**: GET
- **Endpoint**: `/api/categories/9999/subcategories`
- **Description**: Fail to retrieve subcategories for non-existent parent
- **Authorization**: Bearer {token}
- **Request Body**: None
- **Expected Status**: 404 Not Found
- **Expected Response**: Error about parent category not found

#### TC-CAT-SUB-003: Get Subcategories - Success (Empty Results)
- **Method**: GET
- **Endpoint**: `/api/categories/3/subcategories`
- **Description**: Retrieve subcategories for category with no children
- **Authorization**: Bearer {token}
- **Request Body**: None
- **Expected Status**: 200 OK
- **Expected Response**: Empty array

---

### 3.4 POST /api/categories

#### TC-CAT-CREATE-001: Create Category - Success (Parent Category)
- **Method**: POST
- **Endpoint**: `/api/categories`
- **Description**: Successfully create a new parent category
- **Authorization**: Bearer {token} (Admin)
- **Request Body**:
```json
{
  "categoryName": "New Category",
  "categoryDescription": "Description for new category",
  "parentCategoryID": null,
  "isActive": true
}
```
- **Expected Status**: 201 Created
- **Expected Response**:
```json
{
  "categoryID": 10,
  "categoryName": "New Category",
  "categoryDescription": "Description for new category",
  "parentCategoryID": null,
  "parentCategoryName": null,
  "isActive": true,
  "subCategoriesCount": 0,
  "newsArticlesCount": 0
}
```

#### TC-CAT-CREATE-002: Create Category - Fail (Duplicate Name)
- **Method**: POST
- **Endpoint**: `/api/categories`
- **Description**: Fail to create category with existing name
- **Authorization**: Bearer {token} (Admin)
- **Request Body**:
```json
{
  "categoryName": "Academic",
  "categoryDescription": "Duplicate category",
  "parentCategoryID": null,
  "isActive": true
}
```
- **Expected Status**: 409 Conflict
- **Expected Response**: Error about category name already exists

#### TC-CAT-CREATE-003: Create Category - Invalid (Missing Required Fields)
- **Method**: POST
- **Endpoint**: `/api/categories`
- **Description**: Fail to create category with invalid data
- **Authorization**: Bearer {token} (Admin)
- **Request Body**:
```json
{
  "categoryName": "",
  "categoryDescription": "",
  "parentCategoryID": null,
  "isActive": true
}
```
- **Expected Status**: 400 Bad Request
- **Expected Response**: Validation errors

---

### 3.5 PUT /api/categories/{id}

#### TC-CAT-UPDATE-001: Update Category - Success
- **Method**: PUT
- **Endpoint**: `/api/categories/1`
- **Description**: Successfully update an existing category
- **Authorization**: Bearer {token} (Staff/Admin)
- **Request Body**:
```json
{
  "categoryName": "Academic Updated",
  "categoryDescription": "Updated description for academic category",
  "parentCategoryID": null,
  "isActive": true
}
```
- **Expected Status**: 200 OK
- **Expected Response**:
```json
{
  "categoryID": 1,
  "categoryName": "Academic Updated",
  "categoryDescription": "Updated description for academic category",
  "parentCategoryID": null,
  "parentCategoryName": null,
  "isActive": true,
  "subCategoriesCount": 2,
  "newsArticlesCount": 2
}
```

#### TC-CAT-UPDATE-002: Update Category - Fail (Not Found)
- **Method**: PUT
- **Endpoint**: `/api/categories/9999`
- **Description**: Fail to update non-existent category
- **Authorization**: Bearer {token} (Staff/Admin)
- **Request Body**:
```json
{
  "categoryName": "Updated Category",
  "categoryDescription": "Updated description",
  "parentCategoryID": null,
  "isActive": true
}
```
- **Expected Status**: 404 Not Found
- **Expected Response**: Error about category not found

#### TC-CAT-UPDATE-003: Update Category - Invalid (Duplicate Name)
- **Method**: PUT
- **Endpoint**: `/api/categories/2`
- **Description**: Fail to update with name that already exists
- **Authorization**: Bearer {token} (Staff/Admin)
- **Request Body**:
```json
{
  "categoryName": "Academic",
  "categoryDescription": "Description",
  "parentCategoryID": null,
  "isActive": true
}
```
- **Expected Status**: 409 Conflict
- **Expected Response**: Error about category name already exists

---

### 3.6 DELETE /api/categories/{id}

#### TC-CAT-DELETE-001: Delete Category - Success
- **Method**: DELETE
- **Endpoint**: `/api/categories/10`
- **Description**: Successfully delete a category with no subcategories or news
- **Authorization**: Bearer {token} (Admin)
- **Request Body**: None
- **Expected Status**: 200 OK
- **Expected Response**:
```json
{
  "message": "Category deleted successfully"
}
```

#### TC-CAT-DELETE-002: Delete Category - Fail (Has Subcategories)
- **Method**: DELETE
- **Endpoint**: `/api/categories/1`
- **Description**: Fail to delete category with subcategories
- **Authorization**: Bearer {token} (Admin)
- **Request Body**: None
- **Expected Status**: 400 Bad Request
- **Expected Response**: Error about cannot delete category with subcategories

#### TC-CAT-DELETE-003: Delete Category - Invalid (Not Found)
- **Method**: DELETE
- **Endpoint**: `/api/categories/9999`
- **Description**: Fail to delete non-existent category
- **Authorization**: Bearer {token} (Admin)
- **Request Body**: None
- **Expected Status**: 404 Not Found
- **Expected Response**: Error about category not found

---

## 4. Tag Controller (`/api/tags`)

### 4.1 GET /api/tags/{id}

#### TC-TAG-GETID-001: Get Tag by ID - Success
- **Method**: GET
- **Endpoint**: `/api/tags/1`
- **Description**: Successfully retrieve tag by ID
- **Authorization**: Bearer {token}
- **Request Body**: None
- **Expected Status**: 200 OK
- **Expected Response**:
```json
{
  "tagID": 1,
  "tagName": "announcement",
  "note": "General announcements",
  "newsArticlesCount": 3
}
```

#### TC-TAG-GETID-002: Get Tag by ID - Fail (Not Found)
- **Method**: GET
- **Endpoint**: `/api/tags/9999`
- **Description**: Fail to retrieve non-existent tag
- **Authorization**: Bearer {token}
- **Request Body**: None
- **Expected Status**: 404 Not Found
- **Expected Response**: Error message about tag not found

#### TC-TAG-GETID-003: Get Tag by ID - Invalid (No Authorization)
- **Method**: GET
- **Endpoint**: `/api/tags/1`
- **Description**: Fail to retrieve tag without authentication
- **Authorization**: None
- **Request Body**: None
- **Expected Status**: 401 Unauthorized
- **Expected Response**: Unauthorized error

---

### 4.2 GET /api/tags

#### TC-TAG-LIST-001: Get Tags - Success (All Tags)
- **Method**: GET
- **Endpoint**: `/api/tags`
- **Description**: Retrieve all tags with default pagination
- **Authorization**: Bearer {token}
- **Query Parameters**: None
- **Expected Status**: 200 OK
- **Expected Response**: Paginated list of all tags

#### TC-TAG-LIST-002: Get Tags - Success (With Pagination)
- **Method**: GET
- **Endpoint**: `/api/tags?page=1&pageSize=5`
- **Description**: Retrieve tags with custom page size
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - page=1
  - pageSize=5
- **Expected Status**: 200 OK
- **Expected Response**: 5 tags per page

#### TC-TAG-LIST-003: Get Tags - Success (Search by Name)
- **Method**: GET
- **Endpoint**: `/api/tags?searchTerm=exam`
- **Description**: Search tags by name or note
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - searchTerm=exam
- **Expected Status**: 200 OK
- **Expected Response**: Tags matching "exam"

#### TC-TAG-LIST-004: Get Tags - Success (Sort by Name)
- **Method**: GET
- **Endpoint**: `/api/tags?sortBy=name&isDescending=false`
- **Description**: Sort tags by name ascending
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - sortBy=name
  - isDescending=false
- **Expected Status**: 200 OK
- **Expected Response**: Tags sorted A-Z

#### TC-TAG-LIST-005: Get Tags - Success (Sort Descending)
- **Method**: GET
- **Endpoint**: `/api/tags?sortBy=name&isDescending=true`
- **Description**: Sort tags by name descending
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - sortBy=name
  - isDescending=true
- **Expected Status**: 200 OK
- **Expected Response**: Tags sorted Z-A

#### TC-TAG-LIST-006: Get Tags - Success (Combined Search and Pagination)
- **Method**: GET
- **Endpoint**: `/api/tags?searchTerm=tion&page=1&pageSize=3`
- **Description**: Combine search and pagination
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - searchTerm=tion
  - page=1
  - pageSize=3
- **Expected Status**: 200 OK
- **Expected Response**: Tags matching "tion" with pagination

#### TC-TAG-LIST-007: Get Tags - Success (Page 2)
- **Method**: GET
- **Endpoint**: `/api/tags?page=2&pageSize=5`
- **Description**: Retrieve second page of tags
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - page=2
  - pageSize=5
- **Expected Status**: 200 OK
- **Expected Response**: Second page of tags (tags 6-10)

#### TC-TAG-LIST-008: Get Tags - Success (Empty Search Results)
- **Method**: GET
- **Endpoint**: `/api/tags?searchTerm=NonExistentTag`
- **Description**: Search with no matching results
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - searchTerm=NonExistentTag
- **Expected Status**: 200 OK
- **Expected Response**: Empty tags array

---

### 4.3 POST /api/tags

#### TC-TAG-CREATE-001: Create Tag - Success
- **Method**: POST
- **Endpoint**: `/api/tags`
- **Description**: Successfully create a new tag
- **Authorization**: Bearer {token} (Admin)
- **Request Body**:
```json
{
  "tagName": "workshop",
  "note": "Workshop and training events"
}
```
- **Expected Status**: 201 Created
- **Expected Response**:
```json
{
  "tagID": 11,
  "tagName": "workshop",
  "note": "Workshop and training events",
  "newsArticlesCount": 0
}
```

#### TC-TAG-CREATE-002: Create Tag - Fail (Duplicate Name)
- **Method**: POST
- **Endpoint**: `/api/tags`
- **Description**: Fail to create tag with existing name
- **Authorization**: Bearer {token} (Admin)
- **Request Body**:
```json
{
  "tagName": "announcement",
  "note": "Duplicate tag"
}
```
- **Expected Status**: 409 Conflict
- **Expected Response**: Error about tag name already exists

#### TC-TAG-CREATE-003: Create Tag - Invalid (Missing Required Fields)
- **Method**: POST
- **Endpoint**: `/api/tags`
- **Description**: Fail to create tag with invalid data
- **Authorization**: Bearer {token} (Admin)
- **Request Body**:
```json
{
  "tagName": "",
  "note": ""
}
```
- **Expected Status**: 400 Bad Request
- **Expected Response**: Validation errors

---

### 4.4 PUT /api/tags/{id}

#### TC-TAG-UPDATE-001: Update Tag - Success
- **Method**: PUT
- **Endpoint**: `/api/tags/1`
- **Description**: Successfully update an existing tag
- **Authorization**: Bearer {token} (Admin)
- **Request Body**:
```json
{
  "tagName": "announcement-updated",
  "note": "Updated general announcements"
}
```
- **Expected Status**: 200 OK
- **Expected Response**:
```json
{
  "tagID": 1,
  "tagName": "announcement-updated",
  "note": "Updated general announcements",
  "newsArticlesCount": 3
}
```

#### TC-TAG-UPDATE-002: Update Tag - Fail (Not Found)
- **Method**: PUT
- **Endpoint**: `/api/tags/9999`
- **Description**: Fail to update non-existent tag
- **Authorization**: Bearer {token} (Admin)
- **Request Body**:
```json
{
  "tagName": "updated-tag",
  "note": "Updated note"
}
```
- **Expected Status**: 404 Not Found
- **Expected Response**: Error about tag not found

#### TC-TAG-UPDATE-003: Update Tag - Invalid (Duplicate Name)
- **Method**: PUT
- **Endpoint**: `/api/tags/2`
- **Description**: Fail to update with name that already exists
- **Authorization**: Bearer {token} (Admin)
- **Request Body**:
```json
{
  "tagName": "announcement",
  "note": "Trying to use existing name"
}
```
- **Expected Status**: 409 Conflict
- **Expected Response**: Error about tag name already exists

---

### 4.5 DELETE /api/tags/{id}

#### TC-TAG-DELETE-001: Delete Tag - Success
- **Method**: DELETE
- **Endpoint**: `/api/tags/11`
- **Description**: Successfully delete a tag with no associated news articles
- **Authorization**: Bearer {token} (Admin)
- **Request Body**: None
- **Expected Status**: 200 OK
- **Expected Response**:
```json
{
  "message": "Tag deleted successfully"
}
```

#### TC-TAG-DELETE-002: Delete Tag - Fail (Has Associated News)
- **Method**: DELETE
- **Endpoint**: `/api/tags/1`
- **Description**: Fail to delete tag with associated news articles
- **Authorization**: Bearer {token} (Admin)
- **Request Body**: None
- **Expected Status**: 400 Bad Request
- **Expected Response**: Error about cannot delete tag with associated news

#### TC-TAG-DELETE-003: Delete Tag - Invalid (Not Found)
- **Method**: DELETE
- **Endpoint**: `/api/tags/9999`
- **Description**: Fail to delete non-existent tag
- **Authorization**: Bearer {token} (Admin)
- **Request Body**: None
- **Expected Status**: 404 Not Found
- **Expected Response**: Error about tag not found

---

## 5. News Article Controller (`/api/news-articles`)

### 5.1 GET /api/news-articles/{id}

#### TC-NEWS-GETID-001: Get News Article by ID - Success
- **Method**: GET
- **Endpoint**: `/api/news-articles/1`
- **Description**: Successfully retrieve news article by ID
- **Authorization**: Bearer {token}
- **Request Body**: None
- **Expected Status**: 200 OK
- **Expected Response**:
```json
{
  "newsArticleID": 1,
  "newsTitle": "New Academic Year Registration Opens",
  "headline": "Students can now register for the upcoming academic year",
  "createdDate": "2024-01-15T10:00:00Z",
  "newsContent": "The registration portal...",
  "newsSource": "Academic Office",
  "categoryID": 1,
  "categoryName": "Academic",
  "newsStatus": true,
  "createdByID": 1,
  "createdByName": "System Admin",
  "updatedByID": null,
  "updatedByName": null,
  "modifiedDate": null,
  "tagsCount": 2
}
```

#### TC-NEWS-GETID-002: Get News Article by ID - Fail (Not Found)
- **Method**: GET
- **Endpoint**: `/api/news-articles/9999`
- **Description**: Fail to retrieve non-existent news article
- **Authorization**: Bearer {token}
- **Request Body**: None
- **Expected Status**: 404 Not Found
- **Expected Response**: Error message about news article not found

#### TC-NEWS-GETID-003: Get News Article by ID - Invalid (No Authorization)
- **Method**: GET
- **Endpoint**: `/api/news-articles/1`
- **Description**: Fail to retrieve news article without authentication
- **Authorization**: None
- **Request Body**: None
- **Expected Status**: 401 Unauthorized
- **Expected Response**: Unauthorized error

---

### 5.2 GET /api/news-articles

#### TC-NEWS-LIST-001: Get News Articles - Success (All Articles)
- **Method**: GET
- **Endpoint**: `/api/news-articles`
- **Description**: Retrieve all news articles with default pagination
- **Authorization**: Bearer {token}
- **Query Parameters**: None
- **Expected Status**: 200 OK
- **Expected Response**: Paginated list of all news articles

#### TC-NEWS-LIST-002: Get News Articles - Success (With Pagination)
- **Method**: GET
- **Endpoint**: `/api/news-articles?page=1&pageSize=3`
- **Description**: Retrieve news articles with custom page size
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - page=1
  - pageSize=3
- **Expected Status**: 200 OK
- **Expected Response**: 3 news articles per page

#### TC-NEWS-LIST-003: Get News Articles - Success (Search by Title)
- **Method**: GET
- **Endpoint**: `/api/news-articles?searchTerm=Academic`
- **Description**: Search news articles by title, headline, or content
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - searchTerm=Academic
- **Expected Status**: 200 OK
- **Expected Response**: News articles matching "Academic"

#### TC-NEWS-LIST-004: Get News Articles - Success (Filter by Status)
- **Method**: GET
- **Endpoint**: `/api/news-articles?newsStatus=true`
- **Description**: Filter published news articles
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - newsStatus=true
- **Expected Status**: 200 OK
- **Expected Response**: Only published news articles

#### TC-NEWS-LIST-005: Get News Articles - Success (Filter by Category)
- **Method**: GET
- **Endpoint**: `/api/news-articles?categoryID=1`
- **Description**: Filter news articles by category
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - categoryID=1
- **Expected Status**: 200 OK
- **Expected Response**: News articles in Academic category

#### TC-NEWS-LIST-006: Get News Articles - Success (Filter by Author)
- **Method**: GET
- **Endpoint**: `/api/news-articles?createdByID=2`
- **Description**: Filter news articles by author
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - createdByID=2
- **Expected Status**: 200 OK
- **Expected Response**: News articles created by staff user

#### TC-NEWS-LIST-007: Get News Articles - Success (Filter by Date Range)
- **Method**: GET
- **Endpoint**: `/api/news-articles?createdDateFrom=2024-01-01&createdDateTo=2024-12-31`
- **Description**: Filter news articles by creation date range
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - createdDateFrom=2024-01-01
  - createdDateTo=2024-12-31
- **Expected Status**: 200 OK
- **Expected Response**: News articles created in 2024

#### TC-NEWS-LIST-008: Get News Articles - Success (Sort by Date Descending)
- **Method**: GET
- **Endpoint**: `/api/news-articles?sortBy=date&isDescending=true`
- **Description**: Sort news articles by date, newest first
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - sortBy=date
  - isDescending=true
- **Expected Status**: 200 OK
- **Expected Response**: News articles sorted by date (newest first)

#### TC-NEWS-LIST-009: Get News Articles - Success (Combined Filters)
- **Method**: GET
- **Endpoint**: `/api/news-articles?searchTerm=Student&newsStatus=true&categoryID=2&page=1&pageSize=5`
- **Description**: Combine search, multiple filters, and pagination
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - searchTerm=Student
  - newsStatus=true
  - categoryID=2
  - page=1
  - pageSize=5
- **Expected Status**: 200 OK
- **Expected Response**: Published news articles in Student Life category matching "Student"

#### TC-NEWS-LIST-010: Get News Articles - Success (Empty Results)
- **Method**: GET
- **Endpoint**: `/api/news-articles?searchTerm=NonExistentArticle`
- **Description**: Search with no matching results
- **Authorization**: Bearer {token}
- **Query Parameters**: 
  - searchTerm=NonExistentArticle
- **Expected Status**: 200 OK
- **Expected Response**: Empty newsArticles array

---

### 5.3 GET /api/news-articles/category/{categoryId}

#### TC-NEWS-CAT-001: Get News by Category - Success
- **Method**: GET
- **Endpoint**: `/api/news-articles/category/1`
- **Description**: Successfully retrieve news articles by category
- **Authorization**: Bearer {token}
- **Request Body**: None
- **Expected Status**: 200 OK
- **Expected Response**: List of news articles in Academic category

#### TC-NEWS-CAT-002: Get News by Category - Fail (Category Not Found)
- **Method**: GET
- **Endpoint**: `/api/news-articles/category/9999`
- **Description**: Fail to retrieve news for non-existent category
- **Authorization**: Bearer {token}
- **Request Body**: None
- **Expected Status**: 404 Not Found
- **Expected Response**: Error about category not found

#### TC-NEWS-CAT-003: Get News by Category - Success (Empty Results)
- **Method**: GET
- **Endpoint**: `/api/news-articles/category/10`
- **Description**: Retrieve news for category with no articles
- **Authorization**: Bearer {token}
- **Request Body**: None
- **Expected Status**: 200 OK
- **Expected Response**: Empty array

---

### 5.4 GET /api/news-articles/author/{authorId}

#### TC-NEWS-AUTH-001: Get News by Author - Success
- **Method**: GET
- **Endpoint**: `/api/news-articles/author/1`
- **Description**: Successfully retrieve news articles by author
- **Authorization**: Bearer {token}
- **Request Body**: None
- **Expected Status**: 200 OK
- **Expected Response**: List of news articles created by admin

#### TC-NEWS-AUTH-002: Get News by Author - Fail (Author Not Found)
- **Method**: GET
- **Endpoint**: `/api/news-articles/author/9999`
- **Description**: Fail to retrieve news for non-existent author
- **Authorization**: Bearer {token}
- **Request Body**: None
- **Expected Status**: 404 Not Found
- **Expected Response**: Error about author not found

#### TC-NEWS-AUTH-003: Get News by Author - Success (Empty Results)
- **Method**: GET
- **Endpoint**: `/api/news-articles/author/6`
- **Description**: Retrieve news for author with no articles
- **Authorization**: Bearer {token}
- **Request Body**: None
- **Expected Status**: 200 OK
- **Expected Response**: Empty array

---

### 5.5 POST /api/news-articles

#### TC-NEWS-CREATE-001: Create News Article - Success
- **Method**: POST
- **Endpoint**: `/api/news-articles`
- **Description**: Successfully create a new news article
- **Authorization**: Bearer {token} (Staff/Admin)
- **Request Body**:
```json
{
  "newsTitle": "New Campus Facility Opening",
  "headline": "State-of-the-art learning center to open next month",
  "newsContent": "The university is proud to announce the opening of a new state-of-the-art learning center that will provide students with modern facilities for collaborative learning and research.",
  "newsSource": "Campus Development Office",
  "categoryID": 2,
  "newsStatus": true
}
```
- **Expected Status**: 201 Created
- **Expected Response**:
```json
{
  "newsArticleID": 6,
  "newsTitle": "New Campus Facility Opening",
  "headline": "State-of-the-art learning center to open next month",
  "createdDate": "2024-02-15T10:00:00Z",
  "newsContent": "The university is proud to announce...",
  "newsSource": "Campus Development Office",
  "categoryID": 2,
  "categoryName": "Student Life",
  "newsStatus": true,
  "createdByID": 2,
  "createdByName": "News Staff",
  "updatedByID": null,
  "updatedByName": null,
  "modifiedDate": null,
  "tagsCount": 0
}
```

#### TC-NEWS-CREATE-002: Create News Article - Fail (Invalid Category)
- **Method**: POST
- **Endpoint**: `/api/news-articles`
- **Description**: Fail to create news article with non-existent category
- **Authorization**: Bearer {token} (Staff/Admin)
- **Request Body**:
```json
{
  "newsTitle": "Test Article",
  "headline": "Test headline",
  "newsContent": "Test content",
  "newsSource": "Test Source",
  "categoryID": 9999,
  "newsStatus": true
}
```
- **Expected Status**: 404 Not Found
- **Expected Response**: Error about category not found

#### TC-NEWS-CREATE-003: Create News Article - Invalid (Missing Required Fields)
- **Method**: POST
- **Endpoint**: `/api/news-articles`
- **Description**: Fail to create news article with invalid data
- **Authorization**: Bearer {token} (Staff/Admin)
- **Request Body**:
```json
{
  "newsTitle": "",
  "headline": "",
  "newsContent": "",
  "newsSource": "",
  "categoryID": 0,
  "newsStatus": true
}
```
- **Expected Status**: 400 Bad Request
- **Expected Response**: Validation errors

---

### 5.6 PUT /api/news-articles/{id}

#### TC-NEWS-UPDATE-001: Update News Article - Success
- **Method**: PUT
- **Endpoint**: `/api/news-articles/1`
- **Description**: Successfully update an existing news article
- **Authorization**: Bearer {token} (Staff/Admin)
- **Request Body**:
```json
{
  "newsTitle": "Updated Academic Year Registration",
  "headline": "Updated: Students can now register for the upcoming academic year",
  "newsContent": "Updated content about registration portal...",
  "newsSource": "Academic Office",
  "categoryID": 1,
  "newsStatus": true
}
```
- **Expected Status**: 200 OK
- **Expected Response**:
```json
{
  "newsArticleID": 1,
  "newsTitle": "Updated Academic Year Registration",
  "headline": "Updated: Students can now register for the upcoming academic year",
  "createdDate": "2024-01-15T10:00:00Z",
  "newsContent": "Updated content about registration portal...",
  "newsSource": "Academic Office",
  "categoryID": 1,
  "categoryName": "Academic",
  "newsStatus": true,
  "createdByID": 1,
  "createdByName": "System Admin",
  "updatedByID": 2,
  "updatedByName": "News Staff",
  "modifiedDate": "2024-02-15T14:30:00Z",
  "tagsCount": 2
}
```

#### TC-NEWS-UPDATE-002: Update News Article - Fail (Not Found)
- **Method**: PUT
- **Endpoint**: `/api/news-articles/9999`
- **Description**: Fail to update non-existent news article
- **Authorization**: Bearer {token} (Staff/Admin)
- **Request Body**:
```json
{
  "newsTitle": "Updated Article",
  "headline": "Updated headline",
  "newsContent": "Updated content",
  "newsSource": "Source",
  "categoryID": 1,
  "newsStatus": true
}
```
- **Expected Status**: 404 Not Found
- **Expected Response**: Error about news article not found

#### TC-NEWS-UPDATE-003: Update News Article - Invalid (Invalid Category)
- **Method**: PUT
- **Endpoint**: `/api/news-articles/1`
- **Description**: Fail to update with non-existent category
- **Authorization**: Bearer {token} (Staff/Admin)
- **Request Body**:
```json
{
  "newsTitle": "Updated Article",
  "headline": "Updated headline",
  "newsContent": "Updated content",
  "newsSource": "Source",
  "categoryID": 9999,
  "newsStatus": true
}
```
- **Expected Status**: 404 Not Found
- **Expected Response**: Error about category not found

---

### 5.7 DELETE /api/news-articles/{id}

#### TC-NEWS-DELETE-001: Delete News Article - Success
- **Method**: DELETE
- **Endpoint**: `/api/news-articles/6`
- **Description**: Successfully delete a news article
- **Authorization**: Bearer {token} (Admin)
- **Request Body**: None
- **Expected Status**: 200 OK
- **Expected Response**:
```json
{
  "message": "News article deleted successfully"
}
```

#### TC-NEWS-DELETE-002: Delete News Article - Fail (Not Found)
- **Method**: DELETE
- **Endpoint**: `/api/news-articles/9999`
- **Description**: Fail to delete non-existent news article
- **Authorization**: Bearer {token} (Admin)
- **Request Body**: None
- **Expected Status**: 404 Not Found
- **Expected Response**: Error about news article not found

#### TC-NEWS-DELETE-003: Delete News Article - Invalid (Insufficient Permissions)
- **Method**: DELETE
- **Endpoint**: `/api/news-articles/1`
- **Description**: Fail to delete news article without admin role
- **Authorization**: Bearer {token} (Staff - not Admin)
- **Request Body**: None
- **Expected Status**: 403 Forbidden
- **Expected Response**: Error about insufficient permissions

---

## 6. System Controller (`/api/system`)

### 6.1 DELETE /api/system/clear-data

#### TC-SYS-CLEAR-001: Clear Data - Success
- **Method**: DELETE
- **Endpoint**: `/api/system/clear-data`
- **Description**: Successfully clear all data from the database
- **Authorization**: None (AllowAnonymous)
- **Request Body**: None
- **Expected Status**: 200 OK
- **Expected Response**:
```json
{
  "message": "Database cleared successfully",
  "deletedData": {
    "newsTags": 10,
    "newsArticles": 5,
    "tags": 10,
    "categories": 9,
    "systemAccounts": 5
  }
}
```

#### TC-SYS-CLEAR-002: Clear Data - Success (Empty Database)
- **Method**: DELETE
- **Endpoint**: `/api/system/clear-data`
- **Description**: Clear data when database is already empty
- **Authorization**: None
- **Request Body**: None
- **Expected Status**: 200 OK
- **Expected Response**:
```json
{
  "message": "Database cleared successfully",
  "deletedData": {
    "newsTags": 0,
    "newsArticles": 0,
    "tags": 0,
    "categories": 0,
    "systemAccounts": 0
  }
}
```

#### TC-SYS-CLEAR-003: Clear Data - Success (Before Re-seeding)
- **Method**: DELETE
- **Endpoint**: `/api/system/clear-data`
- **Description**: Clear data to prepare for re-seeding
- **Authorization**: None
- **Request Body**: None
- **Expected Status**: 200 OK
- **Expected Response**: Same as TC-SYS-CLEAR-001
- **Note**: Follow this with TC-SYS-SEED-001 to re-seed the database

---

### 6.2 POST /api/system/seed

#### TC-SYS-SEED-001: Seed Data - Success (First Time)
- **Method**: POST
- **Endpoint**: `/api/system/seed`
- **Description**: Successfully seed the database for the first time
- **Authorization**: None (AllowAnonymous)
- **Request Body**: None
- **Expected Status**: 200 OK
- **Expected Response**:
```json
{
  "message": "Database seeded successfully",
  "seededData": {
    "systemAccounts": 5,
    "categories": 9,
    "tags": 10,
    "newsArticles": 5
  }
}
```

#### TC-SYS-SEED-002: Seed Data - Fail (Data Already Exists)
- **Method**: POST
- **Endpoint**: `/api/system/seed`
- **Description**: Fail to seed database when data already exists
- **Authorization**: None
- **Request Body**: None
- **Expected Status**: 409 Conflict
- **Expected Response**:
```json
{
  "message": "Database already contains data. Seeding skipped."
}
```

#### TC-SYS-SEED-003: Seed Data - Success (After Database Clear)
- **Method**: POST
- **Endpoint**: `/api/system/seed`
- **Description**: Successfully seed after clearing database with clear-data endpoint
- **Authorization**: None
- **Request Body**: None
- **Prerequisites**: 
  1. Run `DELETE /api/system/clear-data` first
  2. Then run seed endpoint
- **Expected Status**: 200 OK
- **Expected Response**: Same as TC-SYS-SEED-001

---

## Test Execution Order

### Recommended Workflow for Testing

1. **Initial Setup**: Run `TC-SYS-SEED-001` to seed initial data
2. **Get Authentication**: Run `TC-AUTH-LOGIN-001` with admin credentials to get JWT token
3. **Set Token**: Set the Bearer token in Postman environment/collection for subsequent requests
4. **Test Endpoints**: Execute remaining test cases in any order

### Reset and Re-test Workflow

1. **Clear Database**: Run `TC-SYS-CLEAR-001` to clear all data
2. **Re-seed Database**: Run `TC-SYS-SEED-001` to re-seed fresh data
3. **Re-authenticate**: Run `TC-AUTH-LOGIN-001` again to get a new JWT token
4. **Continue Testing**: Execute test cases as needed

---

## Notes for Testing

### Authentication Setup
1. Create a Collection in Postman
2. Set Authorization at Collection level to "Bearer Token"
3. Use a Collection Variable `{{accessToken}}` for the token
4. After successful login, manually set the token or use a test script:
```javascript
pm.test("Save access token", function() {
    var jsonData = pm.response.json();
    pm.collectionVariables.set("accessToken", jsonData.data.accessToken);
});
```

### Authorization Policies
- **AllowAnonymous**: No token required (Register, Login, Seed)
- **[Authorize]**: Requires valid JWT token (most endpoints)
- **AdminPolicy**: Requires Admin role (Account CRUD, Category/Tag/News DELETE)
- **StaffPolicy**: Requires Staff or Admin role (News Article CREATE/UPDATE)

### Expected Behavior
- All successful operations return appropriate status codes (200, 201)
- All failures return appropriate error status codes (400, 401, 403, 404, 409)
- Validation errors return detailed error messages
- All responses follow the `ApiResult<T>` structure

### Common Status Codes
- **200 OK**: Successful GET, PUT, DELETE
- **201 Created**: Successful POST
- **400 Bad Request**: Validation errors, invalid data
- **401 Unauthorized**: Missing or invalid token
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource not found
- **409 Conflict**: Duplicate data (email, name, etc.)
- **500 Internal Server Error**: Server-side errors

---

## Test Coverage Summary

| Controller | Endpoints | Test Cases |
|------------|-----------|------------|
| Auth | 2 | 6 |
| Account | 5 | 30 |
| Category | 6 | 36 |
| Tag | 5 | 26 |
| News Article | 7 | 41 |
| System | 2 | 6 |
| **Total** | **27** | **145** |

---

**End of Test Cases Document**
