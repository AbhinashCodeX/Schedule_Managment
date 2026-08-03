# Schedule Management System

A role-based web application developed using **ASP.NET Core MVC** for managing coaches, users, activities, availability, locations, and activity bookings.

The system allows:

- Admins to manage users, coaches, locations, and activity types
- Coaches to add their availability
- Users to book activities based on coach, date, and available time

---

## Project Overview

The **Schedule Management System** is designed to simplify the process of scheduling and booking sports or fitness activities.

Users can register either as a:

- User
- Coach

The application contains three roles:

- Admin
- Coach
- User

Each role has different permissions and responsibilities.

---

## Technologies Used

- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server
- LINQ
- HTML
- CSS
- Bootstrap
- JavaScript
- jQuery
- AJAX
- JSON
- Model Binding
- SweetAlert
- Toast Notifications

---

## Concepts Implemented

This project demonstrates the following ASP.NET Core MVC concepts:

- MVC Architecture
- Entity Framework Core
- Database First Approach
- LINQ Queries
- Dependency Injection
- Model Binding
- ViewModels
- Server-side Validation
- Client-side Validation
- AJAX Calls
- JSON Responses
- Cascading Dropdowns
- ViewBag
- ViewData
- TempData
- Session Management
- Role-based Authorization
- Password Hashing
- Search
- Sorting
- Filtering
- Pagination
- SweetAlert Confirmations
- Toast Notifications

---

# User Roles

## 1. Admin

The Admin manages the main data of the application.

### Admin Features

- View all users
- View all coaches
- Edit users
- Edit coaches
- Delete users
- Delete coaches
- Activate or deactivate accounts
- Manage countries
- Manage states
- Manage districts
- Manage activity types
- Search records
- Filter records
- Sort records
- View paginated lists

---

## 2. Coach

A Coach can register and provide availability for different activities.

### Coach Features

- Register as a Coach
- Login to the application
- Manage profile
- Select an activity type
- Add availability for multiple dates
- Add start time
- Add end time
- View availability
- Edit availability
- Delete availability
- View bookings made by users

### Coach Availability Example

A coach can select:

- Activity Type: Football
- Date: 10 August 2026
- Start Time: 08:00 AM
- End Time: 10:00 AM

The coach can add availability for multiple dates.

---

## 3. User

A User can register and book available activities.

### User Features

- Register as a User
- Login to the application
- Manage profile
- Select an activity type
- View coaches based on the selected activity
- Select an available coach
- Select an available date
- Select an available time
- Book an activity
- View booking history
- Cancel a booking

---

# Registration Module

Both Coach and User can register through the registration page.

### Registration Fields

- Full Name
- Email
- Phone Number
- Password
- Confirm Password
- Register As
- Country
- State
- District
- Full Address

### Registration Flow

1. User selects a role.
2. User enters personal details.
3. User selects a country.
4. States are loaded using AJAX.
5. User selects a state.
6. Districts are loaded using AJAX.
7. User enters the full address.
8. Password is hashed before storing it in the database.
9. Registration data is saved.

---

# Location Management

The application uses dependent or cascading dropdowns.

```text
Country
   ↓
State
   ↓
District
   ↓
Full Address
