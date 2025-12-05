# micro-fintech-dotNet — WIP (cooking in the code kitchen)
A small distributed system built with **.NET 8**, demonstrating microservices, messaging patterns, and multi-database integration.  
This project serves as a focused backend engineering showcase.

---

## Overview

The solution contains two independent backend services that coordinate work asynchronously and maintain their own data stores.  
The design emphasizes clean separation, autonomy, and simple event-driven interactions.

---

## Tech Stack

- **.NET 8**
- **Entity Framework Core**
- **RabbitMQ**
- **PostgreSQL**
- **SQL Server**

---

## Running the Project

Infrastructure services run via Docker Compose:

```bash
docker compose up -d
```

Run each service separately:

```bash
dotnet run
```

---

## Purpose

This project highlights backend fundamentals:

- Service-oriented design  
- Asynchronous integration  
- Clean data boundaries  
- Practical financial-style workflows  

The implementation is intentionally minimal and demonstration-focused.

---
