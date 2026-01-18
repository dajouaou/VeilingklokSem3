export default function VeilingInformatie({ gegevens }) {
    // Als gegevens ontbreekt: toon fallback tekst
    if (!gegevens) return <p className="vm-muted">Geen informatie over de veiling beschikbaar.</p>;

    // Status tekst opbouwen uit boolean flags
    const statusParts = [];
    if (gegevens.isGestart) statusParts.push("Gestart");
    else statusParts.push("Niet gestart");
    if (gegevens.isPauze) statusParts.push("Pauze");
    if (gegevens.isAfgesloten) statusParts.push("Afgesloten");

    const statusText = statusParts.join(" | ");

    return (
        <section className="vm-panel">
            <header className="vm-panel-header">
                <h3>Veilinginformatie</h3>
                <p className="vm-muted">Status en actuele gegevens.</p>
            </header>

            <div className="vm-kv">
                <div className="vm-kv-row">
                    <span className="vm-muted">Veilingnummer</span>
                    <strong>{gegevens.id}</strong>
                </div>

                <div className="vm-kv-row">
                    <span className="vm-muted">Status</span>
                    <strong>{statusText}</strong>
                </div>

                {/* Alleen tonen als er een huidig product is */}
                {gegevens.huidigProduct && (
                    <div className="vm-kv-row">
                        <span className="vm-muted">Huidig product</span>
                        <strong>
                            {gegevens.huidigProduct.soort} ({gegevens.huidigProduct.resterendeHoeveelheid} stuks)
                        </strong>
                    </div>
                )}

                <div className="vm-kv-row">
                    <span className="vm-muted">In wachtrij</span>
                    {/* Wachtrij kan null/undefined zijn dus check met Array.isArray */}
                    <strong>{Array.isArray(gegevens.wachtrij) ? gegevens.wachtrij.length : 0}</strong>
                </div>
            </div>
        </section>
    );
}
