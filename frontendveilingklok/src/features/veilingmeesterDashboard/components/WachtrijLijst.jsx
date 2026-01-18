export default function WachtrijLijst({ wachtrij }) {
    // Wachtrij leeg of ontbreekt dan fallback tekst
    if (!wachtrij || wachtrij.length === 0) {
        return <p className="vm-muted">Geen producten in de wachtrij.</p>;
    }

    return (
        <section className="vm-panel">
            <header className="vm-panel-header">
                <h3>Wachtrij</h3>
                <p className="vm-muted">Producten die nog moeten draaien.</p>
            </header>

            <ul className="vm-list">
                {wachtrij.map((item) => (
                    // Key: veilingProductId (beste) of item.id
                    <li key={item.veilingProductId || item.id} className="vm-list-item vm-list-item-split">
                        <div className="vm-text">
                            <strong>{item.soort}</strong> ({item.resterendeHoeveelheid} stuks)
                        </div>

                        {/* volgorde bepaalt de volgorde van draaien in de veiling */}
                        <div className="vm-muted">Volgorde {item.volgorde}</div>
                    </li>
                ))}
            </ul>
        </section>
    );
}
