function money(v) {
    if (v === null || v === undefined) return "—";
    const n = Number(v);
    if (!Number.isFinite(n)) return "—";
    return n.toLocaleString(undefined, { style: "currency", currency: "EUR" });
}

function formatTime(v) {
    if (!v) return "—";
    const d = new Date(v);
    if (Number.isNaN(d.getTime())) return "—";
    return d.toLocaleString();
}

export function CurrentProductPanel({ current }) {
    if (!current) {
        return (
            <div className="vm-panel">
                <div className="vm-panel__header">
                    <div className="vm-panel__title">Huidig veilingproduct</div>
                    <div className="vm-panel__hint">Geen actief product</div>
                </div>
                <div className="vm-empty">
                    <div className="vm-empty__title">Geen actief product</div>
                    <div className="vm-empty__text">Start de veiling of ga naar het volgende product.</div>
                </div>
            </div>
        );
    }

    return (
        <div className="vm-panel">
            <div className="vm-panel__header">
                <div className="vm-panel__title">Huidig veilingproduct</div>
                <div className="vm-panel__hint">Status: {String(current.status)}</div>
            </div>

            <div className="vm-product">
                <div className="vm-product__media">
                    {current.fotoUrl ? (
                        <img className="vm-product__img" src={current.fotoUrl} alt={current.productNaam} />
                    ) : (
                        <div className="vm-product__img vm-img--placeholder">Geen foto</div>
                    )}
                </div>

                <div className="vm-product__info">
                    <div className="vm-product__name">{current.productNaam}</div>

                    <div className="vm-grid2">
                        <div className="vm-kv">
                            <div className="vm-kv__k">Hoeveelheid</div>
                            <div className="vm-kv__v">{current.hoeveelheid}</div>
                        </div>
                        <div className="vm-kv">
                            <div className="vm-kv__k">Aanvoerder</div>
                            <div className="vm-kv__v">{current.aanvoerderNaam || "—"}</div>
                        </div>
                        <div className="vm-kv">
                            <div className="vm-kv__k">Startprijs</div>
                            <div className="vm-kv__v">{money(current.startPrijs)}</div>
                        </div>
                        <div className="vm-kv">
                            <div className="vm-kv__k">Huidige prijs</div>
                            <div className="vm-kv__v">{money(current.huidigePrijs)}</div>
                        </div>
                        <div className="vm-kv">
                            <div className="vm-kv__k">Geactiveerd</div>
                            <div className="vm-kv__v">{formatTime(current.activatedAtUtc)}</div>
                        </div>
                        <div className="vm-kv">
                            <div className="vm-kv__k">Gesloten</div>
                            <div className="vm-kv__v">{formatTime(current.closedAtUtc)}</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
