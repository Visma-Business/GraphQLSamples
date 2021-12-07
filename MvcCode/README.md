# MvcCode

This project provides an example for authenticating with Visma Connect in order to obtain an access token for performing Visma Business Cloud GraphQL queries.

This sample is described in the [Visma Business Cloud API documentation](https://business.visma.net/apidocs/docs).

## Prerequisites

You must have an application registed in the Visma Developer Portal. This must be a web app and you must generate a client secret.

The client ID and secret must be put in the `appsettings.json` file.

```
{
   "ClientId": "...",
   "ClientSecret": "...",
   "Authority": "https://connect.visma.com",
   "SampleApi": "https://localhost:5005/"
}
```

## Credits

This sample is based on the [MvcCode sample](https://github.com/IdentityServer/IdentityServer4/tree/main/samples/Clients/src/MvcCode) from the [IdentityServer project](https://github.com/IdentityServer) on GitHub.