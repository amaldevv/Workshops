# 1 REgister your app is Azure AD
1. Navigate to Azure AD in Azure portal
2. Go to your tenant, App Registrations, select New Registration
3. Provide a name for the app and select a account type - single tenant, redirect uri we will provide later(Provid a local one for the time being, we will correct later)
4. click Register
5. Once its created, get the Application Id(Client id) from the overview page. We will need to use it in our configuration page to connect to the azure ad app b5ef6d9c-8f34-451b-af2d-de1c5257b6f1
6. Goto authentication, select Ad Platform and chose web
7. Add REdirect uri as https://localhost:44321/ and logout url as https://localhost:44321/signout-oidc, select ID tokens
8. Add one more redirect URI for signin 

# 2 Configure your web app to connect to azure ad

1. Modify appsettings.json to configure azure ad
```json
   "UseAzureAD": true,
  "AZureAD": {
    "TenantId": "bf4f7fe7-2f17-408b-bb64-0c93d0f55ff6",
    "ClientId": "bf4f7fe7-2f17-408b-bb64-0c93d0f55ff6",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath ": "/signout-oidc"
  },

   ```       
   2. Modify program.cs to read settings from appsetting.json
   3. Add identity package in startup.cs
   ```Install-Package Microsoft.Identity.Web -Version 0.2.0-preview```
   4. Get settings from config file and configure identity
   