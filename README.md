
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


### ICU Problems with entity framework command
use these
DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 dotnet ef migrations add Init
DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 dotnet ef database update


### Docker postgres for local development
docker run -d	--name Domarservice-db -p 5432:5432 -e POSTGRES_PASSWORD=dev -e PGDATA=/var/lib/postgresql/data/pgdata -v /home/michael/Dockerfiles/postgres:/var/lib/postgresql/data postgres

docker run -p 8082:80 -e 'PGADMIN_DEFAULT_EMAIL=michael@osund.com' -e 'PGADMIN_DEFAULT_PASSWORD=test' -d dpage/pgadmin4

Remember that the pgadmin container should connect to your local computers ip, not localhost.