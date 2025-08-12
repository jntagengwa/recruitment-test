# Metricell recruitment test

List the contents of the Employees table in the SQLite database included in the base project.

Add functionality so that you can add, remove and modify the items of that table.

Create a button in the interface that triggers the following SQL queries. Ensure that the front end is updated dynamically every time the data in the database is modified:

- Increment the field `Value` by 1 where the field `Name` starts with ‘E’, by 10 where `Name` starts with ‘G’ and all others by 100.
- List the sum of all Values for all Names that begin with A, B or C but only present the data where the summed values are greater than or equal to 11171

## Required environment

The base project is using the most recent version of visual studio community with the following technologies:

- SQLite - database
- ASP.NET - backend
- React – frontend

# Recruitment Test – Employee Management App

This project is a recruitment test that requires you to build a full-stack application for managing employees, with a focus on backend, frontend, and integration functionality. The application should allow users to perform CRUD operations on employee records, execute special SQL queries, and see live updates on the frontend.

## Task Overview

Required features:

- Display the list of employees from an SQLite database.
- Implement full CRUD (Create, Read, Update, Delete) operations for employees.
- Add a special button to trigger specific SQL operations (detailed below).

## Functional Requirements

- **List Employees:** Show all entries from the Employees table.
- **Add Employee:** Provide a way to add a new employee record.
- **Edit Employee:** Allow modification of existing employee details.
- **Delete Employee:** Enable removal of employees from the database.
- **Special SQL Operation:** A button that, when clicked, executes the following:
  - Increment the `Value` field by 1 where `Name` starts with ‘E’.
  - Increment the `Value` field by 10 where `Name` starts with ‘G’.
  - Increment the `Value` field by 100 for all other names.
  - After the update, list the sum of all `Value` fields for names beginning with A, B, or C, but only display results where the summed value is greater than or equal to 11,171.

## Bonus Features

- **Live Frontend Updates:** The frontend reflects changes immediately after any modification or special operation.
- **API Documentation:** Backend endpoints are documented and accessible via Swagger UI.

## Technology Stack

- **Backend:** ASP.NET (latest version)
- **Database:** SQLite
- **Frontend:** React
- **API Documentation:** Swagger/OpenAPI

## Setup Instructions

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd recruitment-test
   ```
2. **Frontend Setup:**
   - Navigate to the frontend directory.
   - Install dependencies:
     ```bash
     npm install
     ```
3. **Backend Setup:**
   - Ensure you have the latest version of Visual Studio Community Edition installed.
   - Open the solution in Visual Studio.
   - Restore NuGet packages.
   - Run database migrations if required (the base project includes an SQLite database).
   - Start the ASP.NET backend server.
   - Access Swagger UI at `/swagger` to view API documentation.
   - The frontend will automatically be built and displayed when you run the solution.
   - The React app should connect to the ASP.NET backend and display/manage employee data.
4. **Usage:**
   - Use the frontend to list, add, edit, and delete employees.
   - Use the special SQL operation button to perform the batch update and view the filtered aggregation.
   - All changes should be reflected in real-time on the frontend.
