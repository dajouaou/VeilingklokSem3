export default function LiveKlok({ lot }) {
    if (!lot) {
        return (
            <div className="card shadow-sm border-0">
                <div className="card-body">
                    <h5 className="mb-1">Live klok</h5>
                    <div className="text-muted">Geen actief product</div>
                </div>
            </div>
        );
    }

    const max = Number(lot.maximumPrijs ?? 0);
    const min = Number(lot.minimumPrijs ?? 0);
    const cur = Number(lot.huidigePrijs ?? 0);

    const pctRaw = max <= min ? 0 : ((cur - min) / (max - min)) * 100;
    const pct = Math.max(0, Math.min(100, pctRaw));

    return (
        <div className="card shadow-sm border-0 mb-4">
            <div className="card-body">
                <div className="d-flex justify-content-between align-items-start">
                    <div>
                        <h5 className="mb-1">Live klok</h5>
                        <div className="text-muted small">
                            {lot.soort} - resterend {lot.resterendeHoeveelheid} stuks
                        </div>
                    </div>
                    <span className="badge bg-success">LIVE</span>
                </div>

                <div className="mt-3">
                    <div className="text-muted small">Huidige prijs</div>
                    <div className="display-6 fw-bold">{cur.toFixed(2)} EUR</div>
                    <div className="text-muted small">
                        Update elke 5 sec
                    </div>
                </div>

                <div className="mt-3">
                    <div className="d-flex justify-content-between text-muted small mb-2">
                        <span>Min: {min.toFixed(2)} EUR</span>
                        <span>Max: {max.toFixed(2)} EUR</span>
                    </div>

                    <div className="progress" style={{ height: 10 }}>
                        <div className="progress-bar" style={{ width: `${pct}%` }} />
                    </div>
                </div>
            </div>
        </div>
    );
}
