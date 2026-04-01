# AP-clinical-System

## setup info:

i selected `ASP.NET Core Web App (Model-View-Controller)` **Same as lab 2.4** when creating the starter files

## installed packages:

i tried installing -- following the instructions in lab 2.4 :

- Microsoft.EntityFrameworkCore.Tools
- Microsoft.EntityFrameworkCore.SqlServer

but idk why it wasn't successful T-T

successful imports:

- Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation (for hot reload)

## using hot reload in vs code:

1. install the following extensions in **vs code**
   - C# Dev Kit
   - i believe i also installed (c#) but idk if its needed
2. in vs code open a new terminal and write the following command:

```shell
dotnet watch --project ap-clinical-system run
```
