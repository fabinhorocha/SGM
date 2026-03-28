# MMS - Maintenance Management System 🛠️

**MMS (SGM)** is a robust Full Stack solution developed to optimize industrial asset management. The system centralizes the control of predictive and preventive maintenance, offering a modern interface for KPI visualization and direct integration with ERP processes.

---

## 🚀 Project Highlights
* **SAP Integration:** Architecture designed for seamless communication with SAP ERP, enabling equipment data retrieval and the creation of maintenance notifications.
* **High Performance:** Powered by **Dapper ORM** on the backend to ensure fast queries and low latency, even when handling large data volumes.
* **Hierarchical Visualization:** Implementation of a **Tree View** structure for detailed navigation between machinery, sub-assemblies, and spare parts.
* **Dynamic Dashboards:** Interactive panels featuring performance metrics and real-time maintenance status per equipment.

---

## 🛠️ Tech Stack

### **Backend**
* **Framework:** .NET Framework
* **Language:** C#
* **Data Access:** Dapper (Performance-focused Micro-ORM)
* **Database:** SQL Server (Support for PostgreSQL & MySQL)
* **Architecture:** Clean Architecture / Repository Pattern

### **Frontend**
* **Framework:** AngularJS
* **Styling:** SCSS / Bootstrap / Customized Admin Template
* **Charts:** ngx-charts / Chart.js
* **State Management:** RxJS

---

## 📋 Core Features

1. **Maintenance Dashboard:** Visualization of availability KPIs and failure rates per equipment.
2. **Asset Management:** Complete equipment tree with detailed component breakdowns.
3. **Notification Interface:** Creation and tracking of service orders (SAP Integration).
4. **Maintenance Planning:** Management of predictive, preventive, and corrective maintenance routines.

---

## 📂 Repository Structure

```text
SGM/
├── backend/    # .NET Core API (Controllers, Services, Repositories)
├── frontend/   # Angular Application (Components, Services, Models)