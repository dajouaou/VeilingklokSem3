function formatTime(v) {
    if (!v) return "—";
    const d = new Date(v);
    if (Number.isNaN(d.getTime())) return "—";
    return d.toLocaleString();
}

export function AuditPanel({ audit }) {
    const list = Array.isArray(audit) ? audit : [];

    return (
        <div className="vm-panel">
            <div className="vm-panel__header">
                <div className="vm-panel__title">Gebeurtenissen</div>
                <div className="vm-panel__hint">{list.length} items</div>
            </div>

            {list.length === 0 ? (
                <div className="vm-empty">
                    <div className="vm-empty__title">Nog geen log</div>
                    <div className="vm-empty__text">Acties verschijnen hier automatisch.</div>
                </div>
            ) : (
                <div className="vm-list">
                    {list.map((a) => (
                        <div key={a.id} className="vm-row">
                            <div className="vm-row__main">
                                <div className="vm-row__title">{a.action}</div>
                                <div className="vm-row__meta">
                                    {a.actorNaam || a.actor || "—"} • {formatTime(a.createdAtUtc)}
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}
