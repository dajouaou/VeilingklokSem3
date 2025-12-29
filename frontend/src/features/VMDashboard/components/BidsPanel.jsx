function money(v) {
    if (v === null || v === undefined) return "—";
    const n = Number(v);
    if (!Number.isFinite(n)) return "—";
    return n.toLocaleString(undefined, { style: "currency", currency: "EUR" });
}

function timeOnly(v) {
    if (!v) return "—";
    const d = new Date(v);
    if (Number.isNaN(d.getTime())) return "—";
    return d.toLocaleTimeString();
}

export function BidsPanel({ bids }) {
    const list = Array.isArray(bids) ? bids : [];

    return (
        <div className="vm-panel">
            <div className="vm-panel__header">
                <div className="vm-panel__title">Biedingen</div>
                <div className="vm-panel__hint">{list.length} recent</div>
            </div>

            {list.length === 0 ? (
                <div className="vm-empty">
                    <div className="vm-empty__title">Nog geen biedingen</div>
                    <div className="vm-empty__text">Zodra kopers bieden, zie je het hier.</div>
                </div>
            ) : (
                <div className="vm-list">
                    {list.map((b) => (
                        <div key={b.id} className="vm-row">
                            <div className="vm-row__main">
                                <div className="vm-row__title">{b.koperNaam || "Koper"}</div>
                                <div className="vm-row__meta">{timeOnly(b.placedAtUtc)}</div>
                            </div>
                            <div className="vm-row__right vm-mono">{money(b.amount)}</div>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}
