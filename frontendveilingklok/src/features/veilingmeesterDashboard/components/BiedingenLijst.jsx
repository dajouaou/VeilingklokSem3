export default function BiedingenLijst({ biedingen }) {
    // Als er nog geen biedingen zijn: toon een boodschap
    if (!biedingen || biedingen.length === 0) {
        return <p className="vm-muted">Er zijn nog geen biedingen geplaatst.</p>;
    }

    return (
        <section className="vm-panel">
            <header className="vm-panel-header">
                <h3>Biedingen</h3>
                <p className="vm-muted">Overzicht van recente biedingen.</p>
            </header>

            <ul className="vm-list">
                {biedingen.map((b, i) => (
                    <li key={b.id || i} className="vm-list-item vm-list-item-split">
                        <div className="vm-text">
                            <strong>{Number(b.prijs ?? 0).toFixed(2)} EUR</strong>
                            {b.koperNaam ? (
                                <span className="vm-muted"> ({b.koperNaam})</span>
                            ) : null}
                        </div>

                        <div className="vm-time">
                            {b.tijdstip ? new Date(b.tijdstip).toLocaleTimeString() : ""}
                        </div>
                    </li>
                ))}
            </ul>
        </section>
    );
}
