function money(v) {
    if (v === null || v === undefined) return "—";
    const n = Number(v);
    if (!Number.isFinite(n)) return "—";
    return n.toLocaleString(undefined, { style: "currency", currency: "EUR" });
}

export function StatsCards({ dashboard }) {
    const current = dashboard?.current || null;
    const bidsCount = current?.bids?.length ?? 0;

    const lastBid = current?.bids?.[0] ?? null;

    return (
        <div className="vm-cards">
            <div className="vm-card">
                <div className="vm-card__k">Huidig product</div>
                <div className="vm-card__v">{current?.productNaam || "Geen actief product"}</div>
            </div>

            <div className="vm-card">
                <div className="vm-card__k">Huidige prijs</div>
                <div className="vm-card__v">{money(current?.huidigePrijs)}</div>
            </div>

            <div className="vm-card">
                <div className="vm-card__k">Laatste bod</div>
                <div className="vm-card__v">
                    {lastBid ? `${money(lastBid.amount)} • ${lastBid.koperNaam || "—"}` : "—"}
                </div>
            </div>

            <div className="vm-card">
                <div className="vm-card__k">Biedingen</div>
                <div className="vm-card__v">{bidsCount}</div>
            </div>

            <div className="vm-card">
                <div className="vm-card__k">Veilingstatus</div>
                <div className="vm-card__v">{String(dashboard?.status ?? "—")}</div>
            </div>
        </div>
    );
}
