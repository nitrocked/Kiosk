# Kiosk API API REST for technical knowledge test.

The API emulates the management of kiosk terminals using three entities structured as follows:
  - Customer: Customers that can own one or more kiosk terminals. A customer may have from 0 to N associated kiosks.
  - Kiosk: A kiosk terminal. It is associated with a single Customer and may have from 0 to M associated Devices.
  - Device: Devices or peripherals that are part of a kiosk terminal.

**Considerations:**
• 	When the API project starts, migrations and seeding are applied idempotently. I am aware this is not the optimal approach in a real-world environment; I choosed it for this technical test for the sake of simplicity.
• 	I am aware that the Get All action in the Customer controller retrieves all nested child elements, which would negatively impact performance in a real-world scenario with larger datasets.

## Features

- **.Net WebAPI 9.0**
    - Controller based API
    - Dependency injection
    - Autommapper for DTO/Entity mapping
    - EF as ORM
    - CLEAN
    - SwaggerUI
    - Auth
- **SQl Server on Docker**
    - docker-compose file provided

## Requirements
- .Net 9.0
- Docker

## Installation & Running

1. Clone repository:
   ```bash
   git clone https://github.com/nitrocked/Kiosk.git
   ```

2. Start docker container once inside the directory:
   ```bash
   cd Kiosk
   docker-compose up -d
   ```

3. Build and run project:
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project Kiosk.Api
   ```
   Or in the Kiosk.Api directory:
   ```bash
   cd Kiosk.Api
   dotnet restore
   dotnet run
   ```

4. **Authentication (JWT)**
   
   The API is secured with JWT authentication. Use the following default credentials to obtain a token:
   - **Username:** admin
   - **Password:** password
   
   To authenticate:
   - Call `POST /api/auth/login` with the credentials to get a JWT token
   - In SwaggerUI:
      - Click the **"Authorize"** button (top right), then paste `Bearer <your_token>` in the value field
      - All subsequent requests will include the authentication header automatically
   - In your preferred API client tool (Postman, curl, etc):
      - Add the header `Authorization: Bearer <your_token>` to all requests

5. Test API by integrated SwaggerUI or your preferred client tool:
   ```
   http://localhost:5200/swagger/index.html
   ```