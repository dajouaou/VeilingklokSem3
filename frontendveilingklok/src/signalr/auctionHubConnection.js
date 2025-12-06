import * as signalR from "@microsoft/signalr";

const API_BASE = "https://localhost:56418";

export function createAuctionHubConnection(token) {
    return new signalR.HubConnectionBuilder()
        .withUrl(`${API_BASE}/hubs/auction`, {
            accessTokenFactory: () => token, // stuurt bearer token mee
        })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();
}
