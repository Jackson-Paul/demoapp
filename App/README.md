CRUD Demo (.NET 7 MVC + EF Core SQLite)

Run locally:

```bash
dotnet restore App/CRUDDemo.csproj
dotnet run --project App/CRUDDemo.csproj
```

The app uses a local SQLite DB file `app.db` created in the working directory. This demo uses safe model binding, anti-forgery tokens for POST actions, and validation attributes.
