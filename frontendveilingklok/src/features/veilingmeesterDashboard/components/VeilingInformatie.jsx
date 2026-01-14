export default function VeilingInformatie({ gegevens }) {
    if (!gegevens) return <p className="vm-muted">Geen informatie over de veiling beschikbaar.</p>;

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
                    <strong>{Array.isArray(gegevens.wachtrij) ? gegevens.wachtrij.length : 0}</strong>
                </div>
            </div>
        </section>
    );
}
