import { useEffect, useMemo, useState } from "react";
import { getPublicActieveVeiling } from "../../features/veiling/api/veilingPublicApi";
import "../../styles.css";

export default function MarketSnapshot() {
    const [status, setStatus] = useState("OFFLINE");
    const [lastPrice, setLastPrice] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        let alive = true;

        async function load() {
            try {
                const actief = await getPublicActieveVeiling();
                if (!alive) return;

                const hasVeiling = actief?.id && actief.id > 0;
                const hp = actief?.huidigProduct;

                setStatus(hasVeiling && hp ? "LIVE" : "OFFLINE");
                setLastPrice(hp?.huidigePrijs ?? null);
            } catch {
                if (!alive) return;
                setStatus("OFFLINE");
                setLastPrice(null);
            } finally {
                if (alive) setLoading(false);
            }
        }

        load();
        const i = setInterval(load, 3000);
        return () => {
            alive = false;
            clearInterval(i);
        };
    }, []);

    const priceText = useMemo(() => {
        if (loading) return "...";
        if (lastPrice == null) return "-";
        return `${Number(lastPrice).toFixed(2)}`;
    }, [loading, lastPrice]);

    const biddersText = "-"; // pas mogelijk met backend hubtracking

    return (
        <section className="market-snapshot py-4">
            <div className="container">
                <div className="snapshot-card">
                    <div className="row g-3 align-items-center">
                        <div className="col-md-3">
                            <div className="kpi">
                                <div className="kpi-label">Status</div>
                                <div className="kpi-value">
                                    <span className={status === "LIVE" ? "badge-live" : "badge-offline"}>
                                        {status}
                                    </span>
                                </div>
                            </div>
                        </div>

                        <div className="col-md-3">
                            <div className="kpi">
                                <div className="kpi-label">Online bieders</div>
                                <div className="kpi-value">{biddersText}</div>
                                <div className="kpi-sub text-muted">Realtime bieders.</div>
                            </div>
                        </div>

                        <div className="col-md-3">
                            <div className="kpi">
                                <div className="kpi-label">Laatste prijs</div>
                                <div className="kpi-value">{priceText}</div>
                            </div>
                        </div>

                        <div className="col-md-3">
                            <div className="kpi">
                                <div className="kpi-label">Latency SLA</div>
                                <div className="kpi-value">&lt; 200 ms</div>
                                <div className="kpi-sub">Conform NFR</div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </section>
    );
}
