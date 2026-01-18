import { useState, useEffect, useContext, useCallback, useMemo } from "react";
import { AuthContext } from "../auth/AuthContext";

// API-calls voor het ophalen en beheren van veilingen
import {
    getActiveVeiling,
    startVeiling,
    pauseVeiling,
    resumeVeiling,
    stopVeiling,
    fetchVolgendeVeiling,
} from "../veiling/api/veilingApi";

// Hook voor realtime updates via SignalR
import useLiveVeiling from "./hooks/useLiveVeiling";

// UI-componenten
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
    // Auth-gegevens van de ingelogde gebruiker
    const { token, role, logout } = useContext(AuthContext);

    // State voor actieve veiling en eerstvolgende geplande veiling
    const [veiling, setVeiling] = useState(null);
    const [volgende, setVolgende] = useState(null);

    // State voor laden, foutmeldingen en sidebar
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [sidebarOpen, setSidebarOpen] = useState(false);

    // Realtime data van de veiling via SignalR
    const {
        lot,
        wachtrij,
        lastBid,
        audit,
        loading: liveLoading,
    } = useLiveVeiling(token, veiling?.id);

    // Zet het laatste bod om naar een array zodat de lijstcomponent werkt
    const bids = useMemo(() => (lastBid ? [lastBid] : []), [lastBid]);

    // Alleen REST-calls blokkeren de UI, realtime updates niet
    const isBusy = loading;

    // Haalt actieve veiling op of anders de volgende geplande veiling
    const loadInit = useCallback(async () => {
        if (!token || role !== "Veilingmeester") return;

        try {
            setLoading(true);
            setError("");

            const actief = await getActiveVeiling(token).catch(() => null);

            let next = null;
            if (!actief) {
                next = await fetchVolgendeVeiling(token).catch(() => null);
            }

            setVeiling(actief);
            setVolgende(next);
        } catch (e) {
            console.error(e);
            setError("Kon veilinggegevens niet laden.");
        } finally {
            setLoading(false);
        }
    }, [token, role]);

    // Laadt veilinggegevens bij start en elke 5 seconden opnieuw
    useEffect(() => {
        if (!token || role !== "Veilingmeester") return;

        loadInit();
        const interval = setInterval(() => loadInit(), 5000);
        return () => clearInterval(interval);
    }, [token, role, loadInit]);

    // Start de eerstvolgende geplande veiling
    async function handleStart() {
        setError("");

        if (veiling) {
            setError("Er is al een actieve veiling.");
            return;
        }
        if (!volgende) {
            setError("Geen geplande veiling beschikbaar.");
            return;
        }

        try {
            const gestart = await startVeiling(token, volgende.id);
            setVeiling(gestart);
            setVolgende(null);
        } catch (err) {
            setError(err?.message || "Kon veiling niet starten.");
        }
    }

    // Haalt de actieve veiling opnieuw op
    async function refreshActiveVeiling() {
        const actief = await getActiveVeiling(token).catch(() => null);
        setVeiling(actief);
    }

    // Pauzeert de veiling
    async function handlePause() {
        if (!veiling?.id) return;

        try {
            await pauseVeiling(token, veiling.id);

            // UI alvast aanpassen voordat backend terugkomt
            setVeiling((prev) =>
                prev
                    ? { ...prev, isPauze: true, isGepauzeerd: true, status: "Gepauzeerd" }
                    : prev
            );

            await refreshActiveVeiling();
        } catch (e) {
            setError(e?.message || "Pauzeren mislukt.");
        }
    }

    // Hervat een gepauzeerde veiling
    async function handleResume() {
        if (!veiling?.id) return;

        try {
            await resumeVeiling(token, veiling.id);

            setVeiling((prev) =>
                prev
                    ? { ...prev, isPauze: false, isGepauzeerd: false, status: "Gestart" }
                    : prev
            );

            await refreshActiveVeiling();
        } catch (e) {
            setError(e?.message || "Hervatten mislukt.");
        }
    }

    // Stopt de veiling definitief
    async function handleStop() {
        if (!veiling?.id) return;

        try {
            await stopVeiling(token, veiling.id);
            setVeiling(null);

            const next = await fetchVolgendeVeiling(token).catch(() => null);
            setVolgende(next);
        } catch (e) {
            setError(e?.message || "Stoppen mislukt.");
        }
    }

    return (
        <div className="vm-shell">
            <Sidebar
                logout={logout}
                sidebarOpen={sidebarOpen}
                setSidebarOpen={setSidebarOpen}
            />

            <div className="vm-content">
                <Topbar
                    title="Veilingmeester"
                    veiling={veiling}
                    onMenuClick={() => setSidebarOpen(true)}
                />

                <main className="vm-page">
                    {error && <div className="vm-alert vm-alert-danger">{error}</div>}

                    <section className="vm-hero-card">
                        <h2>Overzicht</h2>
                        <p className="vm-muted">
                            Monitor de veiling, bekijk biedingen en beheer de klok.
                            {liveLoading && (
                                <span className="ms-2 badge bg-light text-dark">
                                    Live verbinden…
                                </span>
                            )}
                        </p>

                        <div className="vm-hero-row">
                            <div className="vm-hero-item">
                                <div className="vm-muted">Volgende veiling</div>
                                {volgende ? (
                                    <div className="vm-hero-strong">
                                        #{volgende.id} | {volgende.veildatum} | {volgende.startTijd}
                                    </div>
                                ) : (
                                    <div className="vm-hero-strong">Geen planning</div>
                                )}
                                <div className="vm-muted">
                                    Producten: {volgende?.aantalProducten ?? 0}
                                </div>
                            </div>

                            <div className="vm-hero-item vm-hero-actions">
                                {volgende && !veiling && (
                                    <button
                                        type="button"
                                        className="vm-btn vm-btn-primary"
                                        onClick={handleStart}
                                        disabled={isBusy}
                                    >
                                        Start veiling
                                    </button>
                                )}
                            </div>
                        </div>
                    </section>

                    <section className="vm-metrics">
                        <div className="vm-metric">
                            <div className="vm-muted">In wachtrij</div>
                            <div className="vm-metric-value">{wachtrij?.length ?? 0}</div>
                        </div>

                        <div className="vm-metric">
                            <div className="vm-muted">Biedingen</div>
                            <div className="vm-metric-value">{bids.length}</div>
                        </div>

                        <div className="vm-metric">
                            <div className="vm-muted">Log events</div>
                            <div className="vm-metric-value">{audit?.length ?? 0}</div>
                        </div>

                        <div className="vm-metric">
                            <div className="vm-muted">Status</div>
                            <div className="vm-metric-value">
                                {loading ? "Bezig" : veiling ? "Actief" : "Inactief"}
                            </div>
                        </div>
                    </section>

                    <section className="vm-panel">
                        <header className="vm-panel-header vm-panel-header-split">
                            <div>
                                <h3>Klokbediening</h3>
                                <p className="vm-muted">
                                    Start, pauzeer, hervat of stop de veiling.
                                </p>
                            </div>
                        </header>

                        <VeilingControls
                            veiling={veiling}
                            onStart={handleStart}
                            onPause={handlePause}
                            onResume={handleResume}
                            onStop={handleStop}
                        />
                    </section>

                    {veiling && (
                        <>
                            <section className="vm-grid-2">
                                <VeilingInformatie gegevens={veiling} />
                                <LiveKlok lot={lot} />
                            </section>

                            <section className="vm-grid-2">
                                <WachtrijLijst wachtrij={wachtrij} />
                                <div className="vm-stack">
                                    <BiedingenLijst biedingen={bids} />
                                    <AuditLijst audit={audit} />
                                </div>
                            </section>
                        </>
                    )}
                </main>
            </div>
        </div>
    );
}
