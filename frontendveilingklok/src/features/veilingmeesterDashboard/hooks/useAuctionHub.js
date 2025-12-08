import { useState, useEffect } from "react";
import useSignalR from "../hooks/useSignalR";
import { getVeilingDetails } from "../../veiling/api/veilingApi";

export function useVeilingmeesterDashboard(veilingId) {
    const [details, setDetails] = useState(null);
    const [bids, setBids] = useState([]);
    const [audit, setAudit] = useState([]);

    const hub = useSignalR();

    useEffect(() => {
        async function load() {
            const d = await getVeilingDetails(veilingId);
            setDetails(d);
        }
        load();
    }, [veilingId]);

    useEffect(() => {
        if (!hub) return;

        hub.invoke("JoinVeilingGroup", veilingId);

        hub.on("ReceiveBid", bid => setBids(prev => [bid, ...prev]));
        hub.on("ReceiveAudit", evt => setAudit(prev => [evt, ...prev]));

        return () => hub.invoke("LeaveVeilingGroup", veilingId);
    }, [hub, veilingId]);

    return {
        details,
        currentLot: details?.huidigProduct,
        queue: details?.wachtrij ?? [],
        bids,
        audit
    };
}
