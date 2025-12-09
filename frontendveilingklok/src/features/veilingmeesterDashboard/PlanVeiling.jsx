import { useState, useEffect, useContext } from "react";
import { AuthContext } from "../auth/AuthContext";
import { fetchVeilingDagen, startVeiling } from "../veiling/api/veilingApi";
import "./VeilingmeesterDashboard.css";

export default function PlanVeiling() {
    const { token } = useContext(AuthContext);

    const [dagen, setDagen] = useState([]);
    const [gekozenDatum, setGekozenDatum] = useState("");
    const [startTijd, setStartTijd] = useState("09:00");
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    useEffect(() => {
        async function load() {
            try {
                setLoading(true);
                setError("");
                const d = await fetchVeilingDagen(token);
                setDagen(d);
            } catch (e) {
                console.error(e);
                setError("Kon veildagen niet ophalen.");
            } finally {
                setLoading(false);
            }
        }
        load();
    }, [token]);

    async function handlePlan() {
        setError("");
        setSuccess("");

        if (!gekozenDatum) {
            setError("Kies een veildatum.");
            return;
        }

        try {
            await startVeiling(token, gekozenDatum, startTijd);
            setSuccess("Veiling succesvol gestart.");
        } catch (e) {
            console.error(e);
            setError("Veiling starten mislukt.");
        }
    }

    return (
        <div className="vm-layout vm-layout-inner">
            <div className="vm-main vm-main-full">
                <header className="vm-topbar">
                    <div>
                        <h1>Veiling plannen</h1>
                        <p>Kies een komende veildag en configureer de starttijd.</p>
                    </div>
                </header>

                <main className="vm-main-content">
                    <section className="vm-card vm-card-highlight">
                        <div className="vm-card-header">
                            <div>
                                <h2>Nieuwe veiling configureren</h2>
                                <p>Je kunt hier een veiling klaarzetten of direct starten.</p>
                            </div>
                        </div>

                        {error && <div className="alert alert-danger vm-alert">{error}</div>}
                        {success && (
                            <div className="alert alert-success vm-alert">{success}</div>
                        )}

                        {loading ? (
                            <div className="vm-loading-block">
                                <div className="spinner-border text-primary" />
                                <span>Beschikbare veildagen laden…</span>
                            </div>
                        ) : (
                            <div className="vm-start-grid">
                                <div className="vm-field">
                                    <label>Beschikbare veildagen</label>
                                    <select
                                        className="form-select"
                                        value={gekozenDatum}
                                        onChange={(e) => setGekozenDatum(e.target.value)}
                                    >
                                        <option value="">-- Selecteer een dag --</option>
                                        {dagen.map((d) => (
                                            <option key={d} value={d}>
                                                {d}
                                            </option>
                                        ))}
                                    </select>
                                </div>

                                <div className="vm-field">
                                    <label>Starttijd</label>
                                    <input
                                        type="time"
                                        className="form-control"
                                        value={startTijd}
                                        onChange={(e) => setStartTijd(e.target.value)}
                                    />
                                </div>

                                <div className="vm-field vm-field-actions">
                                    <button
                                        className="btn btn-primary vm-primary-btn"
                                        onClick={handlePlan}
                                        disabled={!gekozenDatum || loading}
                                    >
                                        Start veiling
                                    </button>
                                    <span className="vm-hint">
                                        Controleer of er nog geen andere actieve veiling is.
                                    </span>
                                </div>
                            </div>
                        )}
                    </section>
                </main>
            </div>
        </div>
    );
}
