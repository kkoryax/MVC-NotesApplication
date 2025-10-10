# MVC-NotesApplication# MVC-NotesApplication

## Overview

MVC-NotesApplication is a web-based notes management system built with ASP.NET Core (.NET 8) using Razor Pages and MVC patterns. The application allows users to create, view, edit, and delete notes, with support for file attachments (images, videos, PDFs). It features user authentication and role-based authorization (Admin/User), and provides an admin interface for user management.

## Features

- **User Authentication & Authorization**: Secure login/logout, role-based access (Admin/User).
- **Notes Management**:
  - Create, edit, delete, and view notes.
  - Attach files (images, videos, PDFs) to notes.
  - Pin important notes.
  - Soft delete for notes.
  - Advanced filtering and pagination for notes list.
- **User Management (Admin)**:
  - Register new users.
  - View and manage users.
  - Soft delete for users.
- **Responsive UI**:
  - Bootstrap-based layout with sidebar navigation.
  - Modal dialogs for note creation and editing.
  - File upload via Dropzone.js.
  - Image and PDF preview with Fancybox.
- **Database**:
  - Entity Framework Core with SQL Server.
  - Models for notes, users, and file attachments.
  - Relationships configured for notes and files.

## Technologies

- ASP.NET Core (.NET 8)
- Razor Pages & MVC
- Entity Framework Core
- Bootstrap, jQuery, Dropzone.js, Fancybox
- SQL Server

## Structure

- **Controllers**: Handle note and user actions, return views and JSON for AJAX.
- **Repositories**: Data access logic for notes and users.
- **Models**: Strongly-typed classes for notes, users, files, and pagination.
- **Views**: Razor Pages for UI, including modals and partials.
- **wwwroot**: Static files (JS, CSS).
- **Middleware**: JWT authentication support.

## Getting Started

1. Configure your SQL Server connection string in `appsettings.json`.
2. Run database migrations if needed.
3. Build and run the application using Visual Studio 2022 or `dotnet run`.
4. Access the app in your browser at `http://localhost:5000` (or configured port).

## Notes
- File uploads are stored in the `/Upload` directory and served as static files.
