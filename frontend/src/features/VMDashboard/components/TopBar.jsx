function formatDateTime(v) {
    if (!v) return "—";
    const d = new Date(v);
    if (Number.isNaN(d.getTime())) return "—";
    return d.toLocaleString();
}

function statusLabel(s) {
    if (!s) return "Onbekend";
    return String(s);
}

export function TopBar({ veilingId, status, startUtc, endUtc, hasActive }) {
    return (
        <div className="vm-topbar">
            <div className="vm-topbar__left">
                <div className="vm-title">VM Dashboard</div>
                <div className="vm-subtitle">
                    Veiling <span className="vm-mono">#{veilingId ?? "—"}</span>
                    <span className={`vm-pill ${hasActive ? "is-on" : "is-off"}`}>
            {hasActive ? "Actief" : "Geen actieve veiling"}
          </span>
                </div>
            </div>

            <div className="vm-topbar__right">
                <div className="vm-kv">
                    <div className="vm-kv__k">Status</div>
                    <div className="vm-kv__v">{statusLabel(status)}</div>
                </div>
                <div className="vm-kv">
                    <div className="vm-kv__k">Start</div>
                    <div className="vm-kv__v">{formatDateTime(startUtc)}</div>
                </div>
                <div className="vm-kv">
                    <div className="vm-kv__k">Eind</div>
                    <div className="vm-kv__v">{formatDateTime(endUtc)}</div>
                </div>
            </div>
        </div>
    );
}
