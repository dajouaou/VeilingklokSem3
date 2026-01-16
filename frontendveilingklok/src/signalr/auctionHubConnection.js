import * as signalR from "@microsoft/signalr";
import { API_BASE_URL } from "../config/apiBaseUrl.js";

export function createAuctionHubConnection(token) {
    return new signalR.HubConnectionBuilder()
        .withUrl(`${API_BASE_URL}/hub/veiling`, {
            accessTokenFactory: () => token,
        })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();
}
