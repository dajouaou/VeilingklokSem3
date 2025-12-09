export default function VeilingStartPanel({
    veildagen,
    gekozenDatum,
    startTijd,
    setGekozenDatum,
    setStartTijd,
    onStart
}) {
    return (
        <div className="card shadow p-4 mb-4">
            <h4 className="mb-3">Nieuwe veiling starten</h4>

            <div className="row g-3">
                <div className="col-md-6">
                    <label className="form-label">Veildatum</label>
                    <select
                        className="form-select"
                        value={gekozenDatum}
                        onChange={(e) => setGekozenDatum(e.target.value)}
                    >
                        <option value="">-- Kies een dag --</option>
                        {veildagen.map((d) => (
                            <option key={d} value={d}>
                                {d}
                            </option>
                        ))}
                    </select>
                </div>

                <div className="col-md-6">
                    <label className="form-label">Starttijd</label>
                    <input
                        type="time"
                        className="form-control"
                        value={startTijd}
                        onChange={(e) => setStartTijd(e.target.value)}
                    />
                </div>
            </div>

            <div className="mt-3 text-end">
                <button className="btn btn-success" onClick={onStart}>
                    Start veiling
                </button>
            </div>
        </div>
    );
}
