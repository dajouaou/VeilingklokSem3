import { useState, useEffect, useContext } from "react";
import { AuthContext } from "../auth/AuthContext";
import {
    fetchVeilingDagen,
    fetchAanmeldingenVoorDatum,
    planVeiling
} from "../veiling/api/veilingApi";
import Sidebar from "./components/Sidebar";

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

    // ?? Leverdatums ophalen
    useEffect(() => {
        fetchVeilingDagen(token)
            .then(setLeverdagen)
            .catch(() => setError("Kon leverdatums niet ophalen"));
    }, [token]);

    // ?? Producten ophalen op basis van LEVERDATUM
    useEffect(() => {
        if (!leverdatum) {
            setAvailable([]);
            setSelected([]);
            return;
        }

        fetchAanmeldingenVoorDatum(token, leverdatum)
            .then(items => {
                setAvailable(items);
                setSelected([]);
            })
            .catch(() => setError("Kon producten niet ophalen"));
    }, [leverdatum, token]);

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

        if (!leverdatum || !veildatum || selected.length === 0) {
            setError("Vul alle velden in.");
            return;
        }

        const payload = {
            leverdatum,
            veildatum,
            startTijd,
            aanmeldingIds: selected.map(s => s.id)
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
        <div className="vm-layout">
            <Sidebar
                logout={logout}
                active="planning"
                sidebarOpen={sidebarOpen}
                setSidebarOpen={setSidebarOpen}
            />

            <div className="vm-main">
                <header className="vm-topbar">
                    <button className="vm-hamburger" onClick={() => setSidebarOpen(true)}>
                        ? Menu
                    </button>
                    <div>
                        <h1>Veiling plannen</h1>
                        <p>Kies leverdatum, veildatum en producten.</p>
                    </div>
                </header>

                <main className="vm-main-content">
                    <section className="vm-card vm-card-highlight">
                        {error && <div className="alert alert-danger">{error}</div>}
                        {success && <div className="alert alert-success">{success}</div>}

                        <div className="vm-start-grid">
                            <div className="vm-field">
                                <label>Leverdatum (producten)</label>
                                <select
                                    className="form-select"
                                    value={leverdatum}
                                    onChange={e => setLeverdatum(e.target.value)}
                                >
                                    <option value="">Kies leverdatum</option>
                                    {leverdagen.map(d => (
                                        <option key={d} value={d}>{d}</option>
                                    ))}
                                </select>
                            </div>

                            <div className="vm-field">
                                <label>Veildatum (klok draait)</label>
                                <input
                                    type="date"
                                    className="form-control"
                                    value={veildatum}
                                    min={leverdatum}
                                    onChange={e => setVeildatum(e.target.value)}
                                />
                            </div>

                            <div className="vm-field">
                                <label>Starttijd</label>
                                <input
                                    type="time"
                                    className="form-control"
                                    value={startTijd}
                                    onChange={e => setStartTijd(e.target.value)}
                                />
                            </div>
                        </div>

                        {leverdatum && (
                            <div className="vm-planning-container">
                                <div className="vm-list">
                                    <h5>Beschikbare producten ({available.length})</h5>
                                    {available.map(item => (
                                        <div key={item.id} className="vm-planning-item">
                                            <div>
                                                <b>{item.soort}</b> ({item.hoeveelheid})
                                                <div className="small text-muted">
                                                    Min {item.minimumPrijs.toFixed(2)} EUR
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
                                    <h5>Geselecteerd ({selected.length})</h5>
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
                            <button
                                className="btn btn-primary"
                                onClick={handlePlan}
                                disabled={selected.length === 0}
                            >
                                Toevoegen aan veiling
                            </button>
                        </div>
                    </section>
                </main>
            </div>
        </div>
    );
}
