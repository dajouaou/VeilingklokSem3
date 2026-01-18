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

                    {/* Select is "controlled": value komt uit state gekozenDatum */}
                    <select
                        id="veildatum"
                        className="vm-input"
                        value={gekozenDatum}
                        // Bij wijziging: update state in parent component
                        onChange={(e) => setGekozenDatum(e.target.value)}
                    >
                        <option value="">Kies een dag</option>

                        {/* veildagen is een lijst strings */}
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

                    {/* Controlled input: value komt uit state startTijd */}
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
                {/* Parent bepaalt wat er gebeurt bij start (API-call) */}
                <button type="button" className="vm-btn vm-btn-primary" onClick={onStart}>
                    Start veiling
                </button>
            </div>
        </section>
    );
}
