// src/signalr/auctionHub.js
import * as signalR from "@microsoft/signalr";

export function createAuctionHub(veilingId, handlers) {
    const HUB_URL = import.meta.env.VITE_SIGNALR_HUB || "/hubs/auction";

    const conn = new signalR.HubConnectionBuilder()
        .withUrl(import.meta.env.VITE_API_BASE + HUB_URL, {
            withCredentials: true,
        })
        .withAutomaticReconnect()
        .build();

    // handlers: {onStatus, onLot, onQueue, onBid}
    conn.on("AuctionStatusChanged", handlers.onStatus);
    conn.on("CurrentLotChanged", handlers.onLot);
    conn.on("QueueUpdated", handlers.onQueue);
    conn.on("BidPlaced", handlers.onBid);

    async function start() {
        await conn.start();
        await conn.invoke("JoinAuctionGroup", veilingId);
    }

    start();

    return conn;
}
