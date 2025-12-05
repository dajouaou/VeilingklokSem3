import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";

export default function useSignalR(token) {
    const [connection, setConnection] = useState(null);
    const [events, setEvents] = useState([]);

    useEffect(() => {
        if (!token) return;

        const conn = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:56418/hub/veiling", {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        conn.start().catch(err => console.error("SignalR error:", err));

        setConnection(conn);

        return () => conn.stop();
    }, [token]);

    return connection;
}
