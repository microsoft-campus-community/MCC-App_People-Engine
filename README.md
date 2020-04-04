# Commasto People Engine - API

This project is the "People Engine" for the MCC Commasto app. The app is a .Net Core 3.1 WebAPI REST backend. It uses the [Graph API](https://developer.microsoft.com/de-de/graph/) to manage users, campus, and hubs.

|            |   |   |
|------------|---|---|
| Dev        | [![Build status](https://dev.azure.com/campusCommunity/commasto/_apis/build/status/commasto-api-dev%20-%20CI)](https://dev.azure.com/campusCommunity/commasto/_build/latest?definitionId=1) | [![Deployment status](https://vsrm.dev.azure.com/campusCommunity/_apis/public/Release/badge/6cdf692a-30de-480f-9c2e-67925a7d66b3/1/1)](https://vsrm.dev.azure.com/campusCommunity/_apis/public/Release/badge/6cdf692a-30de-480f-9c2e-67925a7d66b3/1/1)  |
| Production | [![Build status](https://dev.azure.com/campusCommunity/commasto/_apis/build/status/commasto-api%20-%201%20-%20CI)](https://dev.azure.com/campusCommunity/commasto/_build/latest?definitionId=2) | [![Deployment status](https://vsrm.dev.azure.com/campusCommunity/_apis/public/Release/badge/6cdf692a-30de-480f-9c2e-67925a7d66b3/2/2)](https://vsrm.dev.azure.com/campusCommunity/_apis/public/Release/badge/6cdf692a-30de-480f-9c2e-67925a7d66b3/2/2) |


## Installation


1. First, you need to download [Visual Studio](https://visualstudio.microsoft.com/) or [VSCode](https://code.visualstudio.com/).
2. Download [.Net Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1)
3. Clone the project.
4. You need access to our development tenant to start developing. For this purpose we have created a [Microsoft Form](https://forms.office.com/Pages/ResponsePage.aspx?id=k5qmb5C-LE2k65XhzFWFOOaNAWppoEBFg7yys3HUwQJUMFFBSVJSTU5XRko0MkkyMEszSzlKWjY5QS4u). This will do two things: Add you to our [Commasto MCC App Teams](https://teams.microsoft.com/l/team/19%3a741e24bef66c4ee4ab3076a79f1c2ac4%40thread.tacv2/conversations?groupId=5a945ddc-fabf-4825-9d31-6017c8d4d179&tenantId=6fa69a93-be90-4d2c-a4eb-95e1cc558538) and invite your @campus-community user to our test AD.
5. Once you have completed the Forms you should receive two emails: One email to notify you that you've been invited to our development AD and one mail which contains the appSettings configuration so that you can start developing locally. Make sure to insert the secret from the mail into the `appSettings.Development.json` file.
6. Please **do not** develop on the dev branch. Instead, create you own feature branch. Once you are done you can test your changes by merging to the "`test`" branch. This will trigger a build to [https://commasto-api-test.azurewebsites.net](https://commasto-api-test.azurewebsites.net/swagger). Please make sure nobody else is currently testing their app since it will overwrite everything.
7. Once you are certain your code works you can create a PR. Make sure to describe your changes and reference the issue if applicable.
8. Happy coding :)

## Usage

![Swagger](readme_swagger.png)
The REST API that the application creates is exposed as a [Swagger Page](https://swagger.io/). Click on "Authorize" to access the API.

Use this table to find out the URLs for the swagger pages:

| Environment       | URL |
|-------------------|-----|
| Local             | [https://localhost:44306/swagger](https://localhost:44306/swagger/index.html)   |
| Online Dev        | [https://commasto-api-dev.azurewebsites.net/swagger](https://commasto-api-dev.azurewebsites.net/swagger/index.html)   |
| Online Production | [https://commasto-api.azurewebsites.net/swagger](https://commasto-api.azurewebsites.net/swagger/index.html)   |
| Online Testing | [https://commasto-api-test.azurewebsites.net/swagger](https://commasto-api-test.azurewebsites.net/swagger/index.html)   |

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)