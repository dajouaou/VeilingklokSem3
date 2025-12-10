import { useState, useEffect, useContext } from "react";
import { AuthContext } from "../auth/AuthContext";
import { fetchVeilingDagen, fetchAanmeldingenVoorDatum, planVeiling } from "../veiling/api/veilingApi";
import Sidebar from "./components/Sidebar";

import "./VeilingmeesterDashboard.css";

export default function PlanVeiling() {
    const { token, logout } = useContext(AuthContext);

    const [dagen, setDagen] = useState([]);
    const [gekozenDatum, setGekozenDatum] = useState("");
    const [startTijd, setStartTijd] = useState("09:00");

    const [available, setAvailable] = useState([]);
    const [selected, setSelected] = useState([]);

    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [sidebarOpen, setSidebarOpen] = useState(false);

    useEffect(() => {
        async function loadDays() {
            try {
                const d = await fetchVeilingDagen(token);
                setDagen(d);
            } catch {
                setError("Kon veildagen niet ophalen.");
            }
        }
        loadDays();
    }, [token]);

    useEffect(() => {
        async function loadItems() {
            if (!gekozenDatum) {
                setAvailable([]);
                setSelected([]);
                return;
            }

            try {
                const items = await fetchAanmeldingenVoorDatum(token, gekozenDatum);
                setAvailable(items);
                setSelected([]);
            } catch {
                setError("Kon producten niet ophalen.");
            }
        }
        loadItems();
    }, [gekozenDatum]);

    function addToSelected(item) {
        setAvailable(prev => prev.filter(x => x.id !== item.id));
        setSelected(prev => [...prev, item]);
    }

    function removeFromSelected(item) {
        setSelected(prev => prev.filter(x => x.id !== item.id));
        setAvailable(prev => [...prev, item]);
    }

    async function handlePlan() {
        setError("");
        setSuccess("");

        if (!gekozenDatum || !startTijd || selected.length === 0) {
            setError("Vul alle velden in.");
            return;
        }

        try {
            await planVeiling(token, {
                veildatum: gekozenDatum,
                startTijd,
                aanmeldingIds: selected.map(s => s.id)
            });

            setSuccess("Veiling succesvol gepland!");
        } catch (err) {
            setError(err.message);
        }
    }

    return (
        <div className="vm-layout">

            {/* ? Sidebar werkt nu hetzelfde als in Dashboard */}
            <Sidebar
                logout={logout}
                active="planning"
                sidebarOpen={sidebarOpen}
                setSidebarOpen={setSidebarOpen}
            />

            <div className="vm-main">

                {/* ? Hamburger knop zodat sidebar open kan op mobiel */}
                <header className="vm-topbar">
                    <button
                        className="vm-hamburger"
                        onClick={() => setSidebarOpen(true)}
                    >
                        ? Menu
                    </button>

                    <div>
                        <h1>Veiling plannen</h1>
                        <p>Kies veildag, tijd en producten.</p>
                    </div>
                </header>

                {/* Jouw originele inhoud — NIETS HIERAAN AANGEPAST */}
                <main className="vm-main-content">
                    <section className="vm-card vm-card-highlight">

                        {error && <div className="alert alert-danger">{error}</div>}
                        {success && <div className="alert alert-success">{success}</div>}

                        <div className="vm-start-grid">
                            <div className="vm-field">
                                <label>Veildatum</label>
                                <select className="form-select"
                                    value={gekozenDatum}
                                    onChange={(e) => setGekozenDatum(e.target.value)}
                                >
                                    <option value="">-- Kies dag --</option>
                                    {dagen.map(d => (
                                        <option key={d} value={d}>{d}</option>
                                    ))}
                                </select>
                            </div>

                            <div className="vm-field">
                                <label>Starttijd</label>
                                <input type="time"
                                    className="form-control"
                                    value={startTijd}
                                    onChange={(e) => setStartTijd(e.target.value)}
                                />
                            </div>
                        </div>

                        {gekozenDatum && (
                            <div className="vm-planning-container">

                                <div className="vm-list">
                                    <h5>Beschikbare producten ({available.length})</h5>

                                    {available.map(item => (
                                        <div key={item.id} className="vm-planning-item">
                                            <div>
                                                <b>{item.soort}</b> ({item.hoeveelheid} st.)
                                                <div className="small text-muted">
                                                    Min €{item.minimumPrijs.toFixed(2)}
                                                </div>
                                            </div>

                                            <button
                                                className="btn btn-outline-primary btn-sm"
                                                onClick={() => addToSelected(item)}
                                            >
                                                Toevoegen
                                            </button>
                                        </div>
                                    ))}
                                </div>

                                <div className="vm-list">
                                    <h5>Veilingvolgorde ({selected.length})</h5>

                                    {selected.map((item, i) => (
                                        <div key={item.id} className="vm-planning-item">
                                            <div>
                                                #{i + 1} – <b>{item.soort}</b>
                                            </div>

                                            <button
                                                className="btn btn-outline-secondary btn-sm"
                                                onClick={() => removeFromSelected(item)}
                                            >
                                                Verwijder
                                            </button>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}

                        <div className="text-end mt-3">
                            <button className="btn btn-primary"
                                onClick={handlePlan}
                                disabled={selected.length === 0}
                            >
                                Veiling plannen
                            </button>
                        </div>

                    </section>
                </main>
            </div>
        </div>
    );
}