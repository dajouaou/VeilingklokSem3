export default function VeilingStartPanel({
    veildagen,
    gekozenDatum,
    startTijd,
    setGekozenDatum,
    setStartTijd,
    onStart,
}) {
    return (
        <section className="vm-panel">
            <header className="vm-panel-header">
                <h3>Nieuwe veiling starten</h3>
                <p className="vm-muted">Selecteer veildatum en starttijd.</p>
            </header>

            <div className="vm-form-grid">
                <div className="vm-field">
                    <label className="vm-label" htmlFor="veildatum">
                        Veildatum
                    </label>
                    <select
                        id="veildatum"
                        className="vm-input"
                        value={gekozenDatum}
                        onChange={(e) => setGekozenDatum(e.target.value)}
                    >
                        <option value="">Kies een dag</option>
                        {veildagen.map((d) => (
                            <option key={d} value={d}>
                                {d}
                            </option>
                        ))}
                    </select>
                </div>

                <div className="vm-field">
                    <label className="vm-label" htmlFor="starttijd">
                        Starttijd
                    </label>
                    <input
                        id="starttijd"
                        type="time"
                        className="vm-input"
                        value={startTijd}
                        onChange={(e) => setStartTijd(e.target.value)}
                    />
                </div>
            </div>

            <div className="vm-form-actions">
                <button type="button" className="vm-btn vm-btn-primary" onClick={onStart}>
                    Start veiling
                </button>
            </div>
        </section>
    );
}
