# Kara-OK

ASP.NET Core MVC project (university assignment) that allows users to book karaoke rooms.

## How to run (HTTPS)

```bash
cd Kara-OK.Web
dotnet restore
dotnet ef database update
dotnet run --launch-profile "https"
```

Open in browser: https://localhost:7163

## Seeded Data

### Test Accounts
#### Owners
- email: owner.jane@test.com  
  password: Owner123!
- email: owner.mike@test.com  
  password: Owner123!

#### Customers
- email: customer.anne@test.com  
  password: Customer123!
- email: customer.jake@test.com  
  password: Customer123!

### Rooms
The database is seeded with 5 karaoke rooms:
- Pop
- Rock
- Hip-Hop
- K-Pop
- Disco