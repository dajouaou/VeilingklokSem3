import { useEffect, useState } from "react";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";

export default function useLiveVeiling(token, veilingId) {
    const [lot, setLot] = useState(null);
    const [queue, setQueue] = useState([]);
    const [bids, setBids] = useState([]);
    const [audit, setAudit] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (!veilingId || !token) return;

        setLoading(true);

        const connection = new HubConnectionBuilder()
            .withUrl("https://localhost:56418/hub/veiling", {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        connection.on("ReceiveCurrentLot", (data) => {
            setLot(data);
        });

        connection.on("ReceiveQueue", (items) => {
            setQueue(items || []);
        });

        connection.on("ReceiveBid", (bod) => {
            setBids((prev) => [bod, ...prev]);
        });

        connection.on("ReceiveAudit", (evt) => {
            setAudit((prev) => [evt, ...prev]);
        });

        connection
            .start()
            .then(() => {
                connection.invoke("JoinVeilingGroup", veilingId);
                setLoading(false);
            })
            .catch((err) => {
                console.error("SignalR fout:", err);
                setLoading(false);
            });

        return () => {

            connection
                .invoke("LeaveVeilingGroup", veilingId)
                .catch(() => { })
                .finally(() => connection.stop());
        };
    }, [veilingId, token]);

    return { lot, queue, bids, audit, loading };
}
