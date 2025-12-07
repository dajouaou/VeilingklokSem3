// src/pages/VMDashboard.jsx

import { useState } from "react";
import useVMDashboard from "../hooks/useVMDashboard";
import "../styles/dashboard.css";

import CurrentProductCard from "../components/CurrentProductCard";
import QueueList from "../components/QueueList";
import BidList from "../components/BidList";
import ControlPanel from "../components/ControlPanel";
import StatsCard from "../components/StatsCard";
import AuditList from "../components/AuditList";

export default function VMDashboard() {
    const {
        loading,
        error,
        currentProduct,
        queue,
        bids,
        audit,
        startVeiling,
        nextProduct,
        closeProduct,
    } = useVMDashboard(1);

    const [activeTab, setActiveTab] = useState("bids");

    const totalBids = bids?.length ?? 0;

    if (loading && !currentProduct && queue.length === 0 && bids.length === 0) {
        return (
            <div className="vm-page vm-page-center">
                <p>Dashboard wordt geladen…</p>
            </div>
        );
    }

    return (
        <div className="vm-page">
            <header className="vm-header card">
                <div className="vm-header-left">
                    <span className="vm-logo-dot" />
                    <span className="vm-logo-text">Veilingklok</span>
                </div>

                <div className="vm-header-center">
                    <div className="vm-header-title">Veiling 1 – Aalsmeer</div>
                    <div className="vm-header-subtitle">
                        {currentProduct ? "Live veiling actief" : "Nog geen actief product"}
                    </div>
                </div>

                <div className="vm-header-right">
                    <span className="vm-header-user">Ingelogd als: Veilingmeester</span>
                </div>
            </header>

            {error && <div className="vm-alert vm-alert-error">{error}</div>}

            <main className="vm-grid-top">
                <section className="card vm-col-control">
                    <h2 className="card-title">Besturing</h2>

                    <ControlPanel
                        start={startVeiling}
                        next={nextProduct}
                        close={closeProduct}
                    />

                    <p className="vm-status-text">
                        Status: {currentProduct ? "Actieve veiling" : "Nog geen product gestart"}
                    </p>
                </section>

                <section className="card vm-col-current">
                    <h2 className="card-title">Actief product</h2>

                    {currentProduct ? (
                        <CurrentProductCard product={currentProduct} />
                    ) : (
                        <p className="vm-muted">
                            Er is nog geen actief product. Start eerst een veiling.
                        </p>
                    )}
                </section>

                <section className="card vm-col-stats">
                    <h2 className="card-title">Statistieken</h2>
                    <StatsCard totalBids={totalBids} />
                </section>
            </main>

            <section className="vm-grid-bottom">
                <div className="card vm-bottom-left">
                    <h2 className="card-title">Queue (resterende producten)</h2>
                    {queue && queue.length > 0 ? (
                        <QueueList queue={queue} />
                    ) : (
                        <p className="vm-muted">
                            Er staan momenteel geen producten in de queue.
                        </p>
                    )}
                </div>

                <div className="card vm-bottom-right">
                    <div className="vm-tabs">
                        <button
                            type="button"
                            className={activeTab === "bids" ? "vm-tab vm-tab-active" : "vm-tab"}
                            onClick={() => setActiveTab("bids")}
                        >
                            Biedingen
                        </button>
                        <button
                            type="button"
                            className={activeTab === "audit" ? "vm-tab vm-tab-active" : "vm-tab"}
                            onClick={() => setActiveTab("audit")}
                        >
                            Logboek
                        </button>
                    </div>

                    <div className="vm-tab-content">
                        {activeTab === "bids" ? (
                            bids && bids.length > 0 ? (
                                <BidList bids={bids} />
                            ) : (
                                <p className="vm-muted">
                                    Nog geen biedingen op het huidige product.
                                </p>
                            )
                        ) : audit && audit.length > 0 ? (
                            <AuditList entries={audit} />
                        ) : (
                            <p className="vm-muted">Nog geen logregels voor deze veiling.</p>
                        )}
                    </div>
                </div>
            </section>
        </div>
    );
}
