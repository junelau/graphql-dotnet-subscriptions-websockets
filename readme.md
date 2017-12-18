# GraphQL Subscriptions on ASP.NET Core using WebSockets

### BEWARE: Use at your own risk! Not currently used for a production system...

### Let me know how you get on!

## Background

This is a chat style sample of multiple rooms using GraphQL's subscription.

A WebSocket extension receiver is added in `Startup.cs`  
https://github.com/junelau/graphql-dotnet-subscriptions-websockets/blob/e94c2a21b517e75899230954b4fba253761e4fad/Startup.cs#L58

Implemented classes, along with an example GraphQL schema are located in __GraphQL.Subscriptions.Example__.

## Credits

The fantastic [graphql-dotnet](https://github.com/graphql-dotnet/graphql-dotnet) repository by Joe McBride.  
Great tutorial on [Real-time ASP.NET Core with Websockets](https://radu-matei.com/blog/aspnet-core-websockets-middleware/) by Radu Matei.
