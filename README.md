# ShippingParser

ASN Message Parser

## Overview

This C# .NET project is a solution designed to parse Acknowledgement Shipping Notification (ASN) messages received from suppliers. The ASN messages are structured in a specific format, with each section representing a box and lines below describing the contents of the box. The solution extracts relevant information into database.

## Implementation

This C# .NET solution monitors the "ShippingParsing" folder on the desktop, automatically parsing any dropped files. The solution specifically parses data from files named "data.txt." 

If the "ShippingParsing" folder doesn't exist, it will be created (tested on Windows, may work on Linux).

## RAM Optimization

For optimal RAM usage, the program employs a chunked file-reading approach. This allows the solution to efficiently handle large files without exceeding available memory.

## Database Setup

The program uses SQLite as its database, and **there's no need** to manually create database migrations. The required database schema will be automatically generated when the application runs for the first time. 

## Example Data File

You can use the provided [example data.txt file](https://github.com/AlexisKv/ShippingParser/blob/main/data.txt) to test the parsing functionality. Simply download the file and place it in the "ShippingParsing" folder on your desktop. The solution is configured to parse data exclusively from files named "data.txt."


