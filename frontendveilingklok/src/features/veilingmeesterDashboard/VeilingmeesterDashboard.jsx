import { useState, useEffect, useContext, useCallback } from "react";
import { AuthContext } from "../auth/AuthContext";

import {
    getActiveVeiling,
    startVeiling,
    pauseVeiling,
    resumeVeiling,
    stopVeiling,
    fetchVolgendeVeiling,
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
    const { token, role, logout } = useContext(AuthContext);

    const [veiling, setVeiling] = useState(null);     // actieve veiling
    const [volgende, setVolgende] = useState(null);   // eerstvolgende geplande veiling
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [sidebarOpen, setSidebarOpen] = useState(false);

    const { lot, queue, bids, audit, loading: liveLoading } =
        useLiveVeiling(token, veiling?.id);

    const isBusy = loading || liveLoading;

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

    useEffect(() => {
        loadInit();
    }, [loadInit]);

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
            setError(err.message || "Kon veiling niet starten.");
        }
    }
    async function refreshActiveVeiling() {
        const actief = await getActiveVeiling(token).catch(() => null);
        setVeiling(actief);
    }

    async function handlePause() {
        if (!veiling?.id) return;
        try {
            await pauseVeiling(token, veiling.id);

            setVeiling(prev => prev ? { ...prev, isPauze: true, isGepauzeerd: true, status: "Gepauzeerd" } : prev);

            await refreshActiveVeiling();
        } catch {
            setError("Pauzeren mislukt.");
        }
    }


    async function handleResume() {
        if (!veiling?.id) return;
        try {
            await resumeVeiling(token, veiling.id);

            setVeiling(prev => prev ? { ...prev, isPauze: false, isGepauzeerd: false, status: "Gestart" } : prev);

            await refreshActiveVeiling();
        } catch {
            setError("Hervatten mislukt.");
        }
    }


    async function handleStop() {
        if (!veiling?.id) return;
        try {
            await stopVeiling(token, veiling.id);
            setVeiling(null);

            const next = await fetchVolgendeVeiling(token).catch(() => null);
            setVolgende(next);
        } catch {
            setError("Stoppen mislukt.");
        }
    }

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

                    {/* Volgende geplande veiling */}
                    <section className="vm-card vm-card-highlight mb-4">
                        <div className="vm-card-header d-flex justify-content-between align-items-center">
                            <div>
                                <h2>Volgende geplande veiling</h2>
                                {volgende ? (
                                    <p className="mb-0">
                                        Veiling #{volgende.id} op{" "}
                                        <strong>{volgende.veildatum}</strong> om{" "}
                                        <strong>{volgende.startTijd}</strong> –{" "}
                                        {volgende.aantalProducten} producten
                                    </p>
                                ) : (
                                    <p className="mb-0 text-muted">
                                        Geen geplande veiling beschikbaar.
                                    </p>
                                )}
                            </div>

                            {volgende && !veiling && (
                                <button
                                    className="btn btn-primary vm-primary-btn"
                                    onClick={handleStart}
                                    disabled={isBusy}
                                >
                                    Veiling starten
                                </button>
                            )}
                        </div>
                    </section>

                    {/* Metrics */}
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
                            <p className="value">
                                {isBusy ? "Bezig" : veiling ? "Actief" : "Inactief"}
                            </p>
                        </div>
                    </section>

                    {/* Klokbediening */}
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

                    {veiling && !isBusy && (
                        <>
                            <section className="vm-grid-2">
                                <div className="vm-card">
                                    <VeilingInformatie gegevens={veiling} />
                                </div>
                                <div className="vm-card">
                                    <LiveKlok lot={lot} />
                                </div>
                            </section>

                            <section className="vm-grid-2">
                                <WachtrijLijst wachtrij={queue} />
                                <div>
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
