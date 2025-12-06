// /src/features/veilingmeesterDashboard/hooks/useLiveVeiling.js
import { useEffect, useState } from "react";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";

export default function useLiveVeiling(token, veilingId) {
    const [lot, setLot] = useState(null);
    const [queue, setQueue] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (!veilingId) return;

        setLoading(true);

        const connection = new HubConnectionBuilder()
            .withUrl("https://localhost:5001/hubs/auction", {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        connection.on("ReceiveCurrentLot", (data) => {
            setLot(data);
        });

        connection.on("ReceiveQueue", (items) => {
            setQueue(items);
        });

        connection.start()
            .then(() => {
                connection.invoke("SubscribeVeiling", veilingId);
                setLoading(false);
            })
            .catch(err => console.error("SignalR error:", err));

        return () => {
            connection.stop();
        };
    }, [veilingId, token]);

    return { lot, queue, loading };
}
