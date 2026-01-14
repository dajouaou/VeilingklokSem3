import { useState, useEffect, useContext } from "react";
import { AuthContext } from "../auth/AuthContext";
import { fetchVeilingDagen, fetchAanmeldingenVoorDatum, planVeiling } from "../veiling/api/veilingApi";
import Sidebar from "./components/Sidebar";
import Topbar from "./components/Topbar";
import "./VeilingmeesterDashboard.css";

export default function PlanVeiling() {
    const { token, logout } = useContext(AuthContext);

    const [leverdagen, setLeverdagen] = useState([]);
    const [leverdatum, setLeverdatum] = useState("");
    const [veildatum, setVeildatum] = useState("");
    const [startTijd, setStartTijd] = useState("09:00");

    const [available, setAvailable] = useState([]);
    const [selected, setSelected] = useState([]);

    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [sidebarOpen, setSidebarOpen] = useState(false);

    useEffect(() => {
        fetchVeilingDagen(token)
            .then(setLeverdagen)
            .catch(() => setError("Kon leverdatums niet ophalen"));
    }, [token]);

    useEffect(() => {
        if (!leverdatum) {
            setAvailable([]);
            setSelected([]);
            return;
        }

        fetchAanmeldingenVoorDatum(token, leverdatum)
            .then((items) => {
                setAvailable(items);
                setSelected([]);
            })
            .catch(() => setError("Kon producten niet ophalen"));
    }, [leverdatum, token]);

    function addToSelected(item) {
        setAvailable((prev) => prev.filter((x) => x.id !== item.id));
        setSelected((prev) => [...prev, item]);
    }

    function removeFromSelected(item) {
        setSelected((prev) => prev.filter((x) => x.id !== item.id));
        setAvailable((prev) => [...prev, item]);
    }

    async function handlePlan() {
        setError("");
        setSuccess("");

        if (!leverdatum || !veildatum || selected.length === 0) {
            setError("Vul alle velden in.");
            return;
        }

        const payload = {
            leverdatum,
            veildatum,
            startTijd,
            aanmeldingIds: selected.map((s) => s.id),
        };

        try {
            await planVeiling(token, payload);
            setSuccess("Producten zijn toegevoegd aan de geplande veiling.");
            setSelected([]);

            const items = await fetchAanmeldingenVoorDatum(token, leverdatum);
            setAvailable(items);
        } catch (err) {
            setError(err.message);
        }
    }

    return (
        <div className="vm-shell">
            <Sidebar logout={logout} sidebarOpen={sidebarOpen} setSidebarOpen={setSidebarOpen} />

            <div className="vm-content">
                <Topbar title="Veiling plannen" veiling={null} onMenuClick={() => setSidebarOpen(true)} />

                <main className="vm-page">
                    {error && <div className="vm-alert vm-alert-danger">{error}</div>}
                    {success && <div className="vm-alert">{success}</div>}

                    <section className="vm-hero-card">
                        <h2>Planning</h2>
                        <p className="vm-muted">Kies leverdatum, veildatum en producten voor de wachtrij.</p>
                    </section>

                    <section className="vm-panel">
                        <div className="vm-form-grid">
                            <div className="vm-field">
                                <label className="vm-label" htmlFor="leverdatum">
                                    Leverdatum (producten)
                                </label>
                                <select
                                    id="leverdatum"
                                    className="vm-input"
                                    value={leverdatum}
                                    onChange={(e) => setLeverdatum(e.target.value)}
                                >
                                    <option value="">Kies leverdatum</option>
                                    {leverdagen.map((d) => (
                                        <option key={d} value={d}>
                                            {d}
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <div className="vm-field">
                                <label className="vm-label" htmlFor="veildatum">
                                    Veildatum (klok draait)
                                </label>
                                <input
                                    id="veildatum"
                                    type="date"
                                    className="vm-input"
                                    value={veildatum}
                                    min={leverdatum}
                                    onChange={(e) => setVeildatum(e.target.value)}
                                />
                            </div>

                            <div className="vm-field">
                                <label className="vm-label" htmlFor="starttijd2">
                                    Starttijd
                                </label>
                                <input
                                    id="starttijd2"
                                    type="time"
                                    className="vm-input"
                                    value={startTijd}
                                    onChange={(e) => setStartTijd(e.target.value)}
                                />
                            </div>
                        </div>

                        {leverdatum && (
                            <div style={{ marginTop: 14 }} className="vm-grid-2">
                                <section className="vm-panel">
                                    <header className="vm-panel-header">
                                        <h3>Beschikbaar ({available.length})</h3>
                                        <p className="vm-muted">Producten die nog niet ingepland zijn.</p>
                                    </header>

                                    <ul className="vm-list">
                                        {available.map((item) => (
                                            <li key={item.id} className="vm-list-item vm-list-item-split">
                                                <div>
                                                    <strong>{item.soort}</strong> ({item.hoeveelheid})
                                                    <div className="vm-muted">
                                                        Min {Number(item.minimumPrijs ?? 0).toFixed(2)} EUR
                                                    </div>
                                                </div>

                                                <button type="button" className="vm-btn vm-btn-primary" onClick={() => addToSelected(item)}>
                                                    Toevoegen
                                                </button>
                                            </li>
                                        ))}
                                    </ul>
                                </section>

                                <section className="vm-panel">
                                    <header className="vm-panel-header">
                                        <h3>Geselecteerd ({selected.length})</h3>
                                        <p className="vm-muted">Deze producten gaan de veiling in.</p>
                                    </header>

                                    <ul className="vm-list">
                                        {selected.map((item, i) => (
                                            <li key={item.id} className="vm-list-item vm-list-item-split">
                                                <div>
                                                    <strong>#{i + 1}</strong> {item.soort}
                                                </div>
                                                <button type="button" className="vm-btn" onClick={() => removeFromSelected(item)}>
                                                    Verwijder
                                                </button>
                                            </li>
                                        ))}
                                    </ul>
                                </section>
                            </div>
                        )}

                        <div style={{ marginTop: 14, display: "flex", justifyContent: "flex-end" }}>
                            <button type="button" className="vm-btn vm-btn-primary" onClick={handlePlan} disabled={selected.length === 0}>
                                Toevoegen aan veiling
                            </button>
                        </div>
                    </section>
                </main>
            </div>
        </div>
    );
}
