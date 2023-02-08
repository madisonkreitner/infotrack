This project is a .NET Web Api deployed in Azure App Service and Azure Api Management

To make changes to the API, update the api yaml definition. You can use https://swagger.io/ to view and edit the yaml.

Once the yaml is updated, run the GenerateFromSwagger.ps1 file located within CI/. This will update the models for the API and the client API in the React app.

For local development, run the project in Visual Studio and navigate to the swagger endpoint to test the API.
