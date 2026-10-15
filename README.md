QuizMastery

QuizMastery is a quiz and learning web application designed to support both teachers and students. Teachers can create quizzes and assign them to students, while students can access quizzes assigned to them.

Features:
- Separate teacher and student roles
- Quiz creation with questions and answer options
- Assign quizzes to students
- View quizzes created by teachers or assigned to students
- Server-side authentication and session handling
- Layered separation of presentation, business logic, data access, and shared contracts

Technology stack:
- C# / .NET 8
- ASP.NET Core Razor Pages
- Microsoft SQL Server
- ADO.NET / Microsoft.Data.SqlClient
- Cookie authentication and session management
- xUnit or other test tooling configured in the solution (check the test project for the exact framework)

Architecture

The solution is organized into separate projects:
- PL — presentation layer built with ASP.NET Core Razor Pages
- BLL — business logic and application services
- DAL — data access and SQL Server queries
- Contracts — shared DTOs, interfaces, enums, and mapping abstractions
- BLL.Tests — tests for the business logic layer
- 
This structure separates UI concerns from business rules and database access, making the code easier to maintain and test.
Getting started

Prerequisites:
- .NET 8 SDK
- Access to a Microsoft SQL Server instance
- A database with the schema expected by the application
