import { useEffect, useState } from "react";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";

const HUB_URL = "https://localhost:56418/hub/veiling";

export default function useLiveVeiling(token, veilingId) {
    const [lot, setLot] = useState(null);
    const [queue, setQueue] = useState([]);
    const [bids, setBids] = useState([]);
    const [audit, setAudit] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (!token || !veilingId) return;

        setLoading(true);

        const connection = new HubConnectionBuilder()
            .withUrl(HUB_URL, {
                accessTokenFactory: () => token,
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        connection.on("OntvangHuidigProduct", setLot);
        connection.on("OntvangWachtrij", (wachtrij) =>
            setQueue(wachtrij ?? [])
        );
        connection.on("OntvangBod", (bod) =>
            setBids(prev => [bod, ...prev])
        );
        connection.on("OntvangAudit", (evt) =>
            setAudit(prev => [evt, ...prev])
        );

        async function start() {
            try {
                await connection.start();
                await connection.invoke("JoinVeilingGroep", veilingId);
                setLoading(false);
            } catch (err) {
                if (err?.name !== "AbortError") {
                    console.error("SignalR verbindingsfout:", err);
                }
                setLoading(false);
            }
        }

        start();

        return () => {
            (async () => {
                try {
                    await connection.invoke("VerlaatVeilingGroep", veilingId);
                } catch (_) { }
                await connection.stop();
            })();
        };
    }, [token, veilingId]);

    return {
        lot,
        queue,
        bids,
        audit,
        loading,
    };
}
