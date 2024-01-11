# ShippingParser

ASN Message Parser

## Overview

This C# .NET project is a solution designed to parse Acknowledgement Shipping Notification (ASN) messages received from suppliers. The ASN messages are structured in a specific format, with each section representing a box and lines below describing the contents of the box. The solution extracts relevant information into database.

## Implementation

This C# .NET solution monitors the "ShippingParsing" folder on the desktop, automatically parsing any dropped files. The solution specifically parses data from files named "data.txt." 

If the "ShippingParsing" folder doesn't exist, it will be created (tested on Windows, may work on Linux).
## Database Migration and Update

This project utilizes SQLite as its database. To migrate and update the database, follow these steps:

1. Open a terminal/command prompt and navigate to the project directory.

2. Ensure that you have Entity Framework Tools installed. If not, install it using the following command:

   ```bash
   dotnet tool install --global dotnet-ef
   ```
 3. Run the following commands to create and apply migrations:
     ```bash
    dotnet ef migrations add InitialMigration
    dotnet ef database update
     ```   
  4. Launch application


