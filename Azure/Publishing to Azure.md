## 1 Create an application gateway
> 1. Create a resource group
> 2. Create a application gateway
> 3. Provide a name for the gateway, create a new vnet
> 4. Provide a subnet for app gateway and one more for backend app. Make note of the addresses
> 5. Create a public ip for the frontend
> 6. Create a new empty backend pool
> 7. Create a routing rule, proivde name for rule and listener, select public ip which we created earlier and leave the rest of values
> 8. For backend target, select the backend pool we created and create a new http setting
> 9. Then select review and create

## 2 Publish web app to azure app service from visual studio
Right click the web project in VS Studio and select publish
Choose azure from the list of options, then Azure App Service for windows
In the  next window, select create a new app service
give a name for the appservice , select region and create a hosting plan
Once the service is created, click on the publish option in visual studio to publish the app in azure


## 3 Adding the app service to backend pool
Go to the backend pool we created earlier and select the app service in the target dropdown
