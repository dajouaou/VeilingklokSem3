import { useEffect, useRef, useState } from "react";
import { createAuctionHubConnection } from "../../../signalr/auctionHubConnection"; // ✅ pas pad aan!

export default function useLiveVeiling(token, veilingId) {
    const connRef = useRef(null);

    const [lot, setLot] = useState(null);
    const [wachtrij, setWachtrij] = useState([]);
    const [lastBid, setLastBid] = useState(null);
    const [audit, setAudit] = useState([]); // ✅ array, nooit null
    const [onlineBieders, setOnlineBieders] = useState(null);

    const [loading, setLoading] = useState(false);

    useEffect(() => {
        // als geen token/veiling: state resetten zodat UI niet crasht / oude data toont
        if (!token || !veilingId) {
            setLot(null);
            setWachtrij([]);
            setLastBid(null);
            setAudit([]);
            setOnlineBieders(null);
            setLoading(false);
            return;
        }

        let alive = true;
        setLoading(true);

        const conn = createAuctionHubConnection(token);
        connRef.current = conn;

      
        const onLot = (dto) => setLot(dto);
        const onWachtrij = (items) => setWachtrij(items ?? []);
        const onBod = (bod) => setLastBid(bod);
        const onAudit = (evt) => setAudit((prev) => [evt, ...prev].slice(0, 50)); // laatste 50
        const onOnline = (aantal) => setOnlineBieders(aantal);

        conn.on("OntvangHuidigProduct", onLot);
        conn.on("OntvangWachtrij", onWachtrij);
        conn.on("OntvangBod", onBod);
        conn.on("OntvangAudit", onAudit);

      
        conn.on("OntvangOnlineBieders", onOnline);

        async function start() {
            try {
                await conn.start();
                if (!alive) return;

                
                await conn.invoke("JoinVeilingGroep", Number(veilingId));

                setLoading(false);
            } catch (e) {
                console.error("SignalR start error:", e);
                setLoading(false);
            }
        }

        start();

        return () => {
            alive = false;

           
            try {
                conn.off("OntvangHuidigProduct", onLot);
                conn.off("OntvangWachtrij", onWachtrij);
                conn.off("OntvangBod", onBod);
                conn.off("OntvangAudit", onAudit);
                conn.off("OntvangOnlineBieders", onOnline);
            } catch (_) { }

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

    return {
        lot,
        wachtrij,
        lastBid,
        audit,
        onlineBieders,
        loading,
    };
}
