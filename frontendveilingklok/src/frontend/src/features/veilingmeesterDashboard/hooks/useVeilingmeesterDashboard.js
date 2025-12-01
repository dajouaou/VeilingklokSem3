// src/hooks/useVeilingmeesterDashboard.js
import { useEffect, useState } from "react";
import { VeilingApi } from "../api/veilingmeester";
import { createAuctionHub } from "../../../signalr/auctionHub";

export function useVeilingmeesterDashboard(veilingId) {
    const [details, setDetails] = useState(null);
    const [currentLot, setCurrentLot] = useState(null);
    const [queue, setQueue] = useState([]);
    const [bids, setBids] = useState([]);
    const [audit, setAudit] = useState([]);

    // --- fetchers
    async function fetchAll() {
        const d = await VeilingApi.details(veilingId);
        setDetails(d);

        setCurrentLot(await VeilingApi.currentLot(veilingId));
        setQueue(await VeilingApi.queue(veilingId));
        setBids(await VeilingApi.bids(veilingId));
        setAudit(await VeilingApi.audit(veilingId));
    }

    useEffect(() => {
        fetchAll();

        const hub = createAuctionHub(veilingId, {
            onStatus: async (_, status) => {
                setDetails((old) => ({ ...old, status }));
            },
            onLot: async (_, lot) => {
                setCurrentLot(lot);
                setBids(await VeilingApi.bids(veilingId));
            },
            onQueue: async () => {
                setQueue(await VeilingApi.queue(veilingId));
            },
            onBid: (bid) => {
                setBids((old) => [bid, ...old]);
                setCurrentLot((old) => ({ ...old, huidigePrijs: bid.amount }));
            },
        });

        return () => hub.stop();
    }, [veilingId]);

    return {
        details,
        currentLot,
        queue,
        bids,
        audit,

        refresh: fetchAll,

        start: (body) => VeilingApi.start(body),
        pause: () => VeilingApi.pause(veilingId),
        resume: () => VeilingApi.resume(veilingId),
        stop: () => VeilingApi.stop(veilingId),

        addQueueItem: (body) => VeilingApi.addQueueItem(veilingId, body),
        reorder: (items) => VeilingApi.reorderQueue(veilingId, { items }),

        placeBid: (body) => VeilingApi.placeBid(veilingId, body),
    };
}
