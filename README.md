### API Documentation

#### Introduction

Welcome to the documentation for the Entity API. This API provides functionalities to manage entities with addresses, dates, and names.

#### Authentication

This API does not require authentication for accessing its endpoints.

#### Base URL

The base URL for all API endpoints is:

https://api.example.com/api/entities

vbnet
Copy code

#### Endpoints

1. **Get Entities**

   - **Description**: Retrieves a list of entities based on various filtering criteria.
   - **Endpoint**: `GET /api/entities`
   - **Query Parameters**:
     - `search` (optional): Search string to filter entities based on addresses or names.
     - `gender` (optional): Filter entities by gender.
     - `startDate` (optional): Filter entities with birth dates after or on this date.
     - `endDate` (optional): Filter entities with birth dates before or on this date.
     - `countries` (optional): Filter entities by countries in addresses.
     - `sortBy` (optional): Sort entities based on a specific property.
     - `sortOrder` (optional): Sort order (asc for ascending, desc for descending).
     - `page` (optional): Page number for pagination.
     - `pageSize` (optional): Number of entities per page.
   - **Request Example**:
     ```http
     GET /api/entities?search=John&gender=Male&sortBy=FirstName&sortOrder=asc&page=1&pageSize=10
     ```
   - **Response**:
     ```json
     {
       "TotalCount": 1,
       "TotalPages": 1,
       "PageSize": 10,
       "CurrentPage": 1,
       "Entities": [
         {
           "Id": "1",
           "Addresses": [
             {
               "AddressLine": "123 Main St",
               "City": "City1",
               "Country": "Country1"
             }
           ],
           "Dates": [
             {
               "DateType": "Birth",
               "DateValue": "1990-01-01T00:00:00"
             }
           ],
           "Deceased": false,
           "Gender": "Male",
           "Names": [
             {
               "FirstName": "John",
               "LastName": "Doe"
             }
           ]
         }
       ]
     }
     ```
   - **Error Responses**:
     - Status: 400 Bad Request
       ```json
       {
         "error": "Invalid parameters"
       }
       ```

2. **Get Entity by ID**

   - **Description**: Retrieves a specific entity by its ID.
   - **Endpoint**: `GET /api/entities/{id}`
   - **Path Parameters**:
     - `id` (required): ID of the entity to retrieve.
   - **Response**:
     - Status: 200 OK
       ```json
       {
         "Id": "1",
         "Addresses": [
           {
             "AddressLine": "123 Main St",
             "City": "City1",
             "Country": "Country1"
           }
         ],
         "Dates": [
           {
             "DateType": "Birth",
             "DateValue": "1990-01-01T00:00:00"
           }
         ],
         "Deceased": false,
         "Gender": "Male",
         "Names": [
           {
             "FirstName": "John",
             "LastName": "Doe"
           }
         ]
       }
       ```
   - **Error Responses**:
     - Status: 404 Not Found
       ```json
       {
         "error": "Entity not found"
       }
       ```

3. **Update Entity**

   - **Description**: Updates an existing entity.
   - **Endpoint**: `PUT /api/entities/{id}`
   - **Path Parameters**:
     - `id` (required): ID of the entity to update.
   - **Request Body**: JSON representation of the entity with updated data.
   - **Response**:
     - Status: 204 No Content
   - **Error Responses**:
     - Status: 500 Internal Server Error
       ```json
       {
         "error": "Internal server error"
       }
       ```

4. **Delete Entity**

   - **Description**: Deletes an existing entity.
   - **Endpoint**: `DELETE /api/entities/{id}`
   - **Path Parameters**:
     - `id` (required): ID of the entity to delete.
   - **Response**:
     - Status: 204 No Content
   - **Error Responses**:
     - Status: 404 Not Found
       ```json
       {
         "error": "Entity not found"
       }
       ```

#### Rate Limits

This API does not impose any rate limits.

#### Examples

**Example 1**: Fetch Data

```bash
curl -X GET "https://api.example.com/api/entities?search=John&gender=Male&sortBy=FirstName&sor
