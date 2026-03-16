# Kiosk API API REST for technical knowledge test.

The API emulates the management of kiosk terminals using three entities structured as follows:
• 	Customer: Customers that can own one or more kiosk terminals. A customer may have from 0 to N associated kiosks.
• 	Kiosk: A kiosk terminal. It is associated with a single Customer and may have from 0 to M associated Devices.
• 	Device: Devices or peripherals that are part of a kiosk terminal.

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
- **SQl Server on Docker**
    - docker-compose file provided

## Requirements
- .Net 9.0
- Docker

## Installation

1. Clone repository:
   ```bash
   git clone https://github.com/nitrocked/proyecto.git
   ```
2. Start docker container:
   ```bash
   docker-compose up -d
   ```

3. Build project:
   ```bash
   dotnet restore
   dotnet run 
   ```
4. Explore API documentation:
   ```
   http://localhost:5200/swagger/index.html
   ```