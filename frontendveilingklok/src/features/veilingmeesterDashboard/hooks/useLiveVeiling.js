import { useEffect, useRef, useState } from "react";
import { createAuctionHubConnection } from "../../../signalr/auctionHubConnection"; // maakt HubConnection met token

export default function useLiveVeiling(token, veilingId) {
    // ConnRef bewaart de actuele connection tussen renders (zonder re-render te triggeren)
    const connRef = useRef(null);

    // Live state die via SignalR events wordt bijgewerkt
    const [lot, setLot] = useState(null);
    const [wachtrij, setWachtrij] = useState([]);
    const [lastBid, setLastBid] = useState(null);
    const [audit, setAudit] = useState([]); // altijd array
    const [onlineBieders, setOnlineBieders] = useState(null);

    const [loading, setLoading] = useState(false);

    useEffect(() => {
        // Als er geen token of veilingId is: reset state zodat UI geen oude data toont
        if (!token || !veilingId) {
            setLot(null);
            setWachtrij([]);
            setLastBid(null);
            setAudit([]);
            setOnlineBieders(null);
            setLoading(false);
            return;
        }

        // alive voorkomt dat we state updaten nadat component al unmounted is
        let alive = true;
        setLoading(true);

        // Maak nieuwe SignalR connectie met JWT token
        const conn = createAuctionHubConnection(token);
        connRef.current = conn;

        // Handlers voor server events → updaten state
        const onLot = (dto) => setLot(dto);
        const onWachtrij = (items) => setWachtrij(items ?? []);
        const onBod = (bod) => setLastBid(bod);

        // Audit: prepend nieuw event en hou max 50 items
        const onAudit = (evt) => setAudit((prev) => [evt, ...prev].slice(0, 50));

        // Online bieders
        const onOnline = (aantal) => setOnlineBieders(aantal);

        // Koppel handlers aan eventnamen (moeten matchen met backend SendAsync names)
        conn.on("OntvangHuidigProduct", onLot);
        conn.on("OntvangWachtrij", onWachtrij);
        conn.on("OntvangBod", onBod);
        conn.on("OntvangAudit", onAudit);
        conn.on("OntvangOnlineBieders", onOnline);

        async function start() {
            try {
                // Start websocket/longpoll verbinding
                await conn.start();
                if (!alive) return;

                // Join group zodat je alleen events voor deze veiling ontvangt
                await conn.invoke("JoinVeilingGroep", Number(veilingId));

                setLoading(false);
            } catch (e) {
                // Belangrijk: error loggen, maar UI niet volledig slopen
                console.error("SignalR start error:", e);
                setLoading(false);
            }
        }

        start();

        // Cleanup bij unmount of token/veilingId wijziging
        return () => {
            alive = false;

            // Handlers loskoppelen om memory leaks/dubbele events te voorkomen
            try {
                conn.off("OntvangHuidigProduct", onLot);
                conn.off("OntvangWachtrij", onWachtrij);
                conn.off("OntvangBod", onBod);
                conn.off("OntvangAudit", onAudit);
                conn.off("OntvangOnlineBieders", onOnline);
            } catch (_) { }

            // Connection netjes verlaten + stoppen
            (async () => {
                try {
                    if (conn?.state === "Connected") {
                        await conn.invoke("VerlaatVeilingGroep", Number(veilingId));
                    }
                } catch (_) { }

                try {
                    await conn.stop();
                } catch (_) { }
            })();
        };
    }, [token, veilingId]);

    // Hook geeft live data terug aan componenten
    return {
        lot,
        wachtrij,
        lastBid,
        audit,
        onlineBieders,
        loading,
    };
}
