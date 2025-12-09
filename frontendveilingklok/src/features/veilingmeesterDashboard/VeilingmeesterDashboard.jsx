import { useState, useEffect, useContext } from "react";
import { AuthContext } from "../auth/AuthContext";

import {
    getActiveVeiling,
    startVeiling,
    pauseVeiling,
    resumeVeiling,
    stopVeiling,
    placeBid,
    fetchVeilingDagen
} from "../veiling/api/veilingApi";

import useLiveVeiling from "./hooks/useLiveVeiling";
import LiveKlok from "./components/LiveKlok";
import WachtrijLijst from "./components/WachtrijLijst";
import VeilingControls from "./components/VeilingControls";
import BiedingenLijst from "./components/BiedingenLijst";
import AuditLijst from "./components/AuditLijst";
import VeilingInformatie from "./components/VeilingInformatie";

import Sidebar from "./components/Sidebar";
import Topbar from "./components/Topbar";

import "./VeilingmeesterDashboard.css";

export default function VeilingmeesterDashboard() {
    const auth = useContext(AuthContext);
    const { token, role } = auth;
    const logout = auth?.logout ?? (() => { });

    const [veiling, setVeiling] = useState(null);
    const [veildagen, setVeildagen] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [gekozenDatum, setGekozenDatum] = useState("");
    const [startTijd, setStartTijd] = useState("09:00");

    const { lot, queue, bids, audit, loading: liveLoading } = useLiveVeiling(token, veiling?.id);

    const [sidebarOpen, setSidebarOpen] = useState(false);

    useEffect(() => {
        if (!token || role !== "Veilingmeester") return;

        async function loadInit() {
            try {
                setLoading(true);
                setError("");

                const [actief, dagen] = await Promise.all([
                    getActiveVeiling(token).catch(() => null),
                    fetchVeilingDagen(token)
                ]);

                setVeiling(actief);
                setVeildagen(dagen);
            } catch {
                setError("Kon veilinggegevens niet laden.");
            } finally {
                setLoading(false);
            }
        }

        loadInit();
    }, [token, role]);

    async function handleStart() {
        if (!gekozenDatum || !startTijd) {
            setError("Kies een veildatum én een starttijd.");
            return;
        }

        try {
            setError("");
            const v = await startVeiling(token, gekozenDatum, startTijd);
            setVeiling(v);
        } catch (err) {
            setError("Kon veiling niet starten: " + err.message);
        }
    }

    async function handlePause() {
        try {
            await pauseVeiling(token, veiling.id);
            setVeiling(await getActiveVeiling(token));
        } catch {
            setError("Pauzeren mislukt.");
        }
    }

    async function handleResume() {
        try {
            await resumeVeiling(token, veiling.id);
            setVeiling(await getActiveVeiling(token));
        } catch {
            setError("Hervatten mislukt.");
        }
    }

    async function handleStop() {
        try {
            await stopVeiling(token, veiling.id);
            setVeiling(null);
        } catch {
            setError("Stoppen mislukt.");
        }
    }

    const isBusy = loading || liveLoading;

    return (
        <div className="vm-layout">

            <Sidebar
                logout={logout}
                active="dashboard"
                sidebarOpen={sidebarOpen}
                setSidebarOpen={setSidebarOpen}
            />

            <div className="vm-main">
                <Topbar
                    title="Veilingmeester Dashboard"
                    veiling={veiling}
                    onMenuClick={() => setSidebarOpen(true)}
                />

                <main className="vm-main-content">
                    {error && <div className="alert alert-danger vm-alert">{error}</div>}

                    {!veiling && (
                        <section className="vm-card vm-card-highlight">
                            <div className="vm-card-header">
                                <div>
                                    <h2>Nieuwe veiling starten</h2>
                                    <p>Kies een geplande veildag en starttijd om de klok te activeren.</p>
                                </div>
                            </div>

                            <div className="vm-start-grid">
                                <div className="vm-field">
                                    <label>Veildatum</label>
                                    <select
                                        className="form-select"
                                        value={gekozenDatum}
                                        onChange={(e) => setGekozenDatum(e.target.value)}
                                    >
                                        <option value="">Selecteer een veildag</option>
                                        {veildagen.map((d) => (
                                            <option key={d} value={d}>{d}</option>
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
                                        onClick={handleStart}
                                        disabled={!gekozenDatum || !startTijd || loading}
                                    >
                                        Start veiling
                                    </button>
                                    <span className="vm-hint">Er kan maar een actieve veiling tegelijk zijn.</span>
                                </div>
                            </div>
                        </section>
                    )}

                    <section className="vm-metrics-grid">
                        <div className="vm-card vm-metric">
                            <p className="label">In wachtrij</p>
                            <p className="value">{queue.length}</p>
                        </div>
                        <div className="vm-card vm-metric">
                            <p className="label">Laatste biedingen</p>
                            <p className="value">{bids.length}</p>
                        </div>
                        <div className="vm-card vm-metric">
                            <p className="label">Log events</p>
                            <p className="value">{audit.length}</p>
                        </div>
                        <div className="vm-card vm-metric">
                            <p className="label">Live status</p>
                            <p className="value">{isBusy ? "Bezig" : veiling ? "Actief" : "Inactief"}</p>
                        </div>
                    </section>

                    <section className="vm-card vm-controls-card">
                        <div className="vm-card-header">
                            <h2>Klokbediening</h2>
                        </div>

                        <VeilingControls
                            veiling={veiling}
                            onStart={handleStart}
                            onPause={handlePause}
                            onResume={handleResume}
                            onStop={handleStop}
                        />
                    </section>

                    {isBusy && (
                        <div className="vm-loading-block">
                            <div className="spinner-border text-primary" />
                            <span>Live gegevens ophalen</span>
                        </div>
                    )}

                    {veiling && !isBusy && (
                        <>
                            <section className="vm-grid-2">
                                <div className="vm-card">
                                    <div className="vm-card-header"><h2>Veilinginformatie</h2></div>
                                    <VeilingInformatie gegevens={veiling} />
                                </div>

                                <div className="vm-card vm-liveclock-card">
                                    <div className="vm-card-header"><h2>Live klok</h2></div>
                                    <LiveKlok lot={lot} />
                                    <button
                                        className="btn btn-outline-primary vm-buy-btn"
                                        onClick={() => placeBid(token, veiling.id)}
                                    >
                                        Koop tegen huidige prijs
                                    </button>
                                </div>
                            </section>

                            <section className="vm-grid-2 vm-bottom-grid">
                                <div className="vm-card">
                                    <div className="vm-card-header"><h2>Wachtrij</h2></div>
                                    <WachtrijLijst wachtrij={queue} />
                                </div>

                                <div className="vm-card">
                                    <div className="vm-card-header vm-card-header-tabs">
                                        <h2>Activiteit</h2>
                                        <span className="vm-tab-pill">Laatste biedingen & log</span>
                                    </div>

                                    <div className="vm-activity-grid">
                                        <div className="vm-activity-column">
                                            <h3>Biedingen</h3>
                                            <BiedingenLijst biedingen={bids} />
                                        </div>
                                        <div className="vm-activity-column">
                                            <h3>Audit log</h3>
                                            <AuditLijst audit={audit} />
                                        </div>
                                    </div>
                                </div>
                            </section>
                        </>
                    )}

                    {!veiling && !loading && (
                        <p className="vm-empty-hint">Er is momenteel geen actieve veiling.</p>
                    )}
                </main>
            </div>
        </div>
    );
}
