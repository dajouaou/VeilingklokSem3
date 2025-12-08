// src/features/VM/pages/VMDashboard.jsx
import { useState } from "react";
import { useParams } from "react-router-dom";
import useVMDashboard from "../hooks/useVMDashboard";
import { formatVeilingStatus, formatDateTime } from "../utils/formatters";
import "../../../styles/dashboard.css";

import CurrentProductCard from "../components/CurrentProductCard";
import QueueList from "../components/QueueList";
import BidList from "../components/BidList";
import ControlPanel from "../components/ControlPanel";
import StatsCard from "../components/StatsCard";
import AuditList from "../components/AuditList";

export default function VMDashboard() {
    const { veilingId } = useParams();
    const parsedId = Number(veilingId) || 1;

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
        veilingNaam,
        locatie,
        vmNaam,
        status,
        startTijdUtc,
        eindTijdUtc,
        totalBidsCurrentProduct,
        totaalBiedingenVeiling,
        productenInQueue,
        verkochteProducten,
        totaalProducten,
    } = useVMDashboard(parsedId);

    const [activeTab, setActiveTab] = useState("bids");

    const safeVeilingNaam =
        veilingNaam || (parsedId ? `Veiling ${parsedId}` : "Veiling");
    const safeLocatie = locatie || "";
    const safeVmNaam = vmNaam || "Veilingmeester";
    const veilingStatusLabel = formatVeilingStatus(status);

    const isInitialLoad =
        loading &&
        !error &&
        !currentProduct &&
        (!queue || queue.length === 0) &&
        (!bids || bids.length === 0) &&
        (!audit || audit.length === 0);

    if (isInitialLoad) {
        return (
            <div className="vm-page vm-page-center">
                <p>Dashboard wordt geladen…</p>
            </div>
        );
    }

    const headerSubtitle = currentProduct
        ? `Live veiling (${veilingStatusLabel})`
        : `Nog geen actief product (${veilingStatusLabel})`;

    const queueCount = productenInQueue ?? (queue ? queue.length : 0);

    const canStart = !currentProduct && queueCount > 0;
    const canNext = !!currentProduct && queueCount > 0;
    const canClose = !!currentProduct;

    return (
        <div className="vm-page">
            <header className="vm-header card">
                <div className="vm-header-left">
                    <span className="vm-logo-dot" />
                    <span className="vm-logo-text">Veilingklok</span>
                </div>

                <div className="vm-header-center">
                    <div className="vm-header-title">
                        {safeVeilingNaam}
                        {safeLocatie ? ` – ${safeLocatie}` : ""}
                    </div>
                    <div className="vm-header-subtitle">{headerSubtitle}</div>
                </div>

                <div className="vm-header-right">
                    <span className="vm-header-user">Ingelogd als: {safeVmNaam}</span>
                    <span className="vm-header-user">
                        Start: {formatDateTime(startTijdUtc)}
                    </span>
                    <span className="vm-header-user">
                        Einde: {formatDateTime(eindTijdUtc)}
                    </span>
                </div>
            </header>

            {loading && !isInitialLoad && (
                <div className="vm-alert vm-alert-info">
                    Dashboard wordt ververst…
                </div>
            )}

            {error && <div className="vm-alert vm-alert-error">{error}</div>}

            <main className="vm-grid-top">
                <section className="card vm-col-control">
                    <h2 className="card-title">Besturing</h2>

                    <ControlPanel
                        start={startVeiling}
                        next={nextProduct}
                        close={closeProduct}
                        disabled={loading}
                        canStart={canStart}
                        canNext={canNext}
                        canClose={canClose}
                    />

                    <p className="vm-status-text">
                        Status veiling: {veilingStatusLabel}.{" "}
                        {currentProduct
                            ? "Er is een actief product."
                            : "Nog geen product gestart."}
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
                    <StatsCard
                        totalBidsCurrentProduct={totalBidsCurrentProduct}
                        totaalBiedingenVeiling={totaalBiedingenVeiling}
                        productenInQueue={queueCount}
                        verkochteProducten={verkochteProducten}
                        totaalProducten={totaalProducten}
                    />
                </section>
            </main>

            <section className="vm-grid-bottom">
                <div className="card vm-bottom-left">
                    <h2 className="card-title">Queue (resterende producten)</h2>

                    {queueCount > 0 && (
                        <p className="vm-status-text">
                            Nog {queueCount} product(en) in deze veiling.
                        </p>
                    )}

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
                            className={
                                activeTab === "bids"
                                    ? "vm-tab vm-tab-active"
                                    : "vm-tab"
                            }
                            onClick={() => setActiveTab("bids")}
                        >
                            Biedingen
                        </button>
                        <button
                            type="button"
                            className={
                                activeTab === "audit"
                                    ? "vm-tab vm-tab-active"
                                    : "vm-tab"
                            }
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
                            <p className="vm-muted">
                                Nog geen logregels voor deze veiling.
                            </p>
                        )}
                    </div>
                </div>
            </section>
        </div>
    );
}
