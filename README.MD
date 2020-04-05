# Combud Backend

### Environment variables
1. `REDIS_URL` Connection string from [StackExchange.Redis](https://stackexchange.github.io/StackExchange.Redis/Configuration) e.g `localhost:6379,password=`
2. `SQL_CONNSTR` Microsoft sql connection string, can be generated [Here](https://www.developerfusion.com/tools/sql-connection-string/)
3. `GMAP_API_KEY` A google api key that has access to the Geocoding api
4. `AWS_ID` An aws key id that has access to the ses api
5. `AWS_KEY` An aws secret key that has access to the ses api
6. `BASE_URL` The url the app is running on so the email verification can create a url
7. `SENDER_ADDRESS` An email to send the verification from

