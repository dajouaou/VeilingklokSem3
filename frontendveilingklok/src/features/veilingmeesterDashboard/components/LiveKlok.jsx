export default function LiveKlok({ lot }) {
    // Als er geen actief product is: toon "geen actief product"
    if (!lot) {
        return (
            <section className="vm-panel">
                <header className="vm-panel-header">
                    <h3>Live klok</h3>
                    <p className="vm-muted">Geen actief product.</p>
                </header>
            </section>
        );
    }

    // Waarden veilig omzetten naar numbers (null/undefined = 0)
    const max = Number(lot.maximumPrijs ?? 0);
    const min = Number(lot.minimumPrijs ?? 0);
    const cur = Number(lot.huidigePrijs ?? 0);

    // Bereken progress percentage tussen min en max (voor de bar)
    // Als max < min dan kan je niet delen dus percentage = 0
    const pctRaw = max <= min ? 0 : ((cur - min) / (max - min)) * 100;

    // Clamp zodat het altijd tussen 0 en 100 blijft
    const pct = Math.max(0, Math.min(100, pctRaw));

    return (
        <section className="vm-panel vm-panel-live">
            <header className="vm-panel-header vm-panel-header-split">
                <div>
                    <h3>Live klok</h3>
                    <p className="vm-muted">
                        {lot.soort} | resterend {lot.resterendeHoeveelheid} stuks
                    </p>
                </div>
                <span className="vm-badge vm-badge-live">LIVE</span>
            </header>

            <div className="vm-live-price">
                <div className="vm-muted">Huidige prijs</div>

                <div className="vm-price">{cur.toFixed(2)} EUR</div>
                <div className="vm-muted">Update elke 5 sec</div>
            </div>

            <div className="vm-live-range">
                <div className="vm-live-range-row">
                    <span className="vm-muted">Min: {min.toFixed(2)} EUR</span>
                    <span className="vm-muted">Max: {max.toFixed(2)} EUR</span>
                </div>

                {/* Bar breedte is percentage */}
                <div className="vm-bar">
                    <div className="vm-bar-fill" style={{ width: `${pct}%` }} />
                </div>
            </div>
        </section>
    );
}
