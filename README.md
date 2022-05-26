
# Domarservice-Api

### Create a new migration

In root folder
```dotnet ef migrations add Init```

```dotnet ef database update```

<!-- ```dotnet ef migrations add Init --project Domarservice.DAL/Domarservice.DAL.csproj -s Domarservice.API```

```dotnet ef database update --project Domarservice.DAL/Domarservice.DAL.csproj -s Domarservice.API``` -->



### Important notes on users claims, and roles.
The identity users (in aspNetUsers table) has a RefereeId and a CompanyId column, only one of these should be set to the RefereeId or the CompanyId, depending on what type of user it is.
The role CompanyUser, RefereeUser and Admins also are Claims that are checked at controller level by Authorization.

So first we check if the user has the correct role for the route.
Then we check if the user has the correct Referee or Company Id to modify or read the current record.