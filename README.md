# ProductsMgtTask
Testing Task

StartUp Steps:

0- Open the project with Visual Studio 2019 v16.3.8 or higher 

1- Update the Default connection string to match your database server (Found in folder OA_API -> appsettings.json ) 

2- Run Data Migration
   - Delete Existing Migration in folder OA.DataAccess -> Migrations
   - Open Package Manager Console
   - Set DataAccess as default project
   - Run Commands:  Add-Migration Init
                      Update-DataBase
                      
3- Run the Project Then Open the API Description on link https://localhost:44301/index.html

4- Seed Data
  Use SeedData/SeedUser API to add user with admin role
    This will create user with credentials (Email: Admin@app.com, Password: Admin@123)
  Use SeedData/SeedProduct API to add test products and categories
  
5- Authorize 
  Use Auth/Login API with the admin credentials that mentioned above to get an access token

6- Testing 
	Use Swagger or Postman to test the products and category api’s 
	Use Visual Studio to run the API integration tests that provided in the OA.Tests Project
