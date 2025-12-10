// src/features/veilingmeesterDashboard/VeilingmeesterDashboard.jsx
import { useState, useEffect, useContext } from "react";
import { AuthContext } from "../auth/AuthContext";

import {
    getActiveVeiling,
    startVeiling,
    pauseVeiling,
    resumeVeiling,
    stopVeiling,
    placeBid,
    fetchPlannedVeilingen,
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

    const [veiling, setVeiling] = useState(null);        // actieve veiling (overzicht)
    const [planned, setPlanned] = useState([]);          // geplande veilingen
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const { lot, queue, bids, audit, loading: liveLoading } = useLiveVeiling(
        token,
        veiling?.id
    );

    const [sidebarOpen, setSidebarOpen] = useState(false);

    useEffect(() => {
        if (!token || role !== "Veilingmeester") return;

        async function loadInit() {
            try {
                setLoading(true);
                setError("");

                const [actief, geplande] = await Promise.all([
                    getActiveVeiling(token).catch(() => null),
                    fetchPlannedVeilingen(token).catch(() => []),
                ]);

                setVeiling(actief);          // kan null zijn
                setPlanned(geplande || []);
            } catch (e) {
                console.error(e);
                setError("Kon veilinggegevens niet laden.");
            } finally {
                setLoading(false);
            }
        }

        loadInit();
    }, [token, role]);

    const isBusy = loading || liveLoading;

    const nextPlanned = planned && planned.length > 0 ? planned[0] : null;

    async function refreshState() {
        try {
            const [actief, geplande] = await Promise.all([
                getActiveVeiling(token).catch(() => null),
                fetchPlannedVeilingen(token).catch(() => []),
            ]);
            setVeiling(actief);
            setPlanned(geplande || []);
        } catch (e) {
            console.error(e);
        }
    }

    async function handleStart() {
        setError("");

        if (!nextPlanned) {
            setError(
                "Er is geen geplande veiling beschikbaar. Plan eerst een veiling via 'Veiling plannen'."
            );
            return;
        }

        // optioneel: check of datum vandaag is
        const todayStr = new Date().toISOString().slice(0, 10);
        if (nextPlanned.veildatum && nextPlanned.veildatum !== todayStr) {
            if (
                !window.confirm(
                    "Deze veiling staat niet op vandaag. Weet je zeker dat je nu wilt starten?"
                )
            ) {
                return;
            }
        }

        try {
            await startVeiling(token, nextPlanned.id);
            await refreshState();
        } catch (err) {
            console.error(err);
            setError("Kon veiling niet starten: " + err.message);
        }
    }

    async function handlePause() {
        try {
            await pauseVeiling(token, veiling.id);
            await refreshState();
        } catch {
            setError("Pauzeren mislukt.");
        }
    }

    async function handleResume() {
        try {
            await resumeVeiling(token, veiling.id);
            await refreshState();
        } catch {
            setError("Hervatten mislukt.");
        }
    }

    async function handleStop() {
        try {
            await stopVeiling(token, veiling.id);
            await refreshState();
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
                                {nextPlanned ? (
                                    <p className="mb-0">
                                        Veiling #{nextPlanned.id} op{" "}
                                        <strong>{nextPlanned.veildatum}</strong> om{" "}
                                        <strong>{nextPlanned.startTijd}</strong> –{" "}
                                        {nextPlanned.aantalProducten} producten
                                    </p>
                                ) : (
                                    <p className="mb-0 text-muted">
                                        Er is nog geen veiling gepland. Ga naar “Veiling plannen”
                                        om een veiling klaar te zetten.
                                    </p>
                                )}
                            </div>

                            {nextPlanned && !veiling && (
                                <button
                                    className="btn btn-primary vm-primary-btn"
                                    onClick={handleStart}
                                    disabled={isBusy}
                                >
                                    Start volgende veiling
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
                                    <div className="vm-card-header">
                                        <h2>Veilinginformatie</h2>
                                    </div>
                                    <VeilingInformatie gegevens={veiling} />
                                </div>

                                <div className="vm-card vm-liveclock-card">
                                    <div className="vm-card-header">
                                        <h2>Live klok</h2>
                                    </div>
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
                                    <div className="vm-card-header">
                                        <h2>Wachtrij</h2>
                                    </div>
                                    <WachtrijLijst wachtrij={queue} />
                                </div>

                                <div className="vm-card">
                                    <div className="vm-card-header vm-card-header-tabs">
                                        <h2>Activiteit</h2>
                                        <span className="vm-tab-pill">
                                            Laatste biedingen & log
                                        </span>
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
                        <p className="vm-empty-hint">
                            Er is momenteel geen actieve veiling. Start de volgende geplande
                            veiling of plan een nieuwe.
                        </p>
                    )}
                </main>
            </div>
        </div>
    );
}
