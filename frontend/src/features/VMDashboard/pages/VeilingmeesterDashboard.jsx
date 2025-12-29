import { useMemo, useState } from "react";
import { useVeilingmeesterDashboard } from "../hooks/useVeilingmeesterDashboard";
import { Sidebar } from "../components/Sidebar";
import { TopBar } from "../components/TopBar";
import { StatsCards } from "../components/StatsCards";
import { CurrentProductPanel } from "../components/CurrentProductPanel";
import { BidsPanel } from "../components/BidsPanel";
import { ControlPanel } from "../components/ControlPanel";
import { QueuePanel } from "../components/QueuePanel";
import { AuditPanel } from "../components/AuditPanel";

function statusOf(dashboard, active) {
    if (dashboard?.status) return dashboard.status;
    if (active?.status) return active.status;
    return "—";
}

export default function VeilingmeesterDashboard() {
    const {
        loading,
        loadingDashboard,
        error,
        activeVeiling,
        selectedVeilingId,
        setSelectedVeilingId,
        currentVeilingId,
        dashboard,
        selectVeilingAndLoad,
        actions,
    } = useVeilingmeesterDashboard();

    const [nav, setNav] = useState("dashboard");

    const hasActive = Boolean(activeVeiling?.id);
    const status = statusOf(dashboard, activeVeiling);

    const current = dashboard?.current ?? null;
    const bids = current?.bids ?? [];
    const queue = dashboard?.queue ?? [];
    const audit = dashboard?.audit ?? [];

    const showPicker = !loading && !hasActive && !dashboard;

    const title = useMemo(() => {
        if (dashboard?.veilingNaam) return dashboard.veilingNaam;
        return "—";
    }, [dashboard]);

    return (
        <div className="vm-shell">
            <Sidebar activeKey={nav} onNavigate={setNav} />

            <main className="vm-main">
                <TopBar
                    veilingId={currentVeilingId}
                    status={status}
                    startUtc={dashboard?.startTijdUtc ?? null}
                    endUtc={dashboard?.eindTijdUtc ?? null}
                    hasActive={hasActive}
                />

                {error && <div className="vm-alert">{error}</div>}

                {loading && (
                    <div className="vm-loading">
                        <div className="vm-loading__title">Laden...</div>
                        <div className="vm-loading__text">Even geduld.</div>
                    </div>
                )}

                {showPicker && (
                    <div className="vm-picker">
                        <div className="vm-picker__card">
                            <div className="vm-picker__title">Geen actieve veiling</div>
                            <div className="vm-picker__text">Kies een veiling-ID om te laden.</div>
                            <div className="vm-picker__row">
                                <input
                                    className="vm-input"
                                    type="number"
                                    min="1"
                                    value={selectedVeilingId}
                                    onChange={(e) => setSelectedVeilingId(Number(e.target.value))}
                                />
                                <button className="vm-btn vm-btn--accent" type="button" onClick={() => selectVeilingAndLoad(selectedVeilingId)}>
                                    Laad dashboard
                                </button>
                            </div>
                        </div>
                    </div>
                )}

                {!loading && dashboard && (
                    <div className="vm-content">
                        <div className="vm-headerBlock">
                            <div className="vm-headerBlock__left">
                                <div className="vm-h1">{title}</div>
                                <div className="vm-muted">
                                    Veiling <span className="vm-mono">#{dashboard.veilingId}</span> • Locatie {String(dashboard.locatie)} • VM{" "}
                                    {dashboard.vmNaam || "—"}
                                </div>
                            </div>
                            <div className="vm-headerBlock__right">
                                <button className="vm-btn" type="button" onClick={actions.refresh} disabled={loadingDashboard}>
                                    Ververs
                                </button>
                            </div>
                        </div>

                        <StatsCards dashboard={dashboard} />

                        <div className="vm-gridMain">
                            <section className="vm-col">
                                {(nav === "dashboard" || nav === "active") && <CurrentProductPanel current={current} />}
                                {(nav === "dashboard" || nav === "active") && <ControlPanel status={dashboard.status} current={current} actions={actions} />}
                                {nav === "audit" && <AuditPanel audit={audit} />}
                                {nav === "settings" && (
                                    <div className="vm-panel">
                                        <div className="vm-panel__header">
                                            <div className="vm-panel__title">Instellingen</div>
                                            <div className="vm-panel__hint">Binnenkort</div>
                                        </div>
                                        <div className="vm-empty">
                                            <div className="vm-empty__title">Nog niet beschikbaar</div>
                                            <div className="vm-empty__text">Dit is frontend-only of later.</div>
                                        </div>
                                    </div>
                                )}
                            </section>

                            <aside className="vm-side">
                                {(nav === "dashboard" || nav === "active") && <BidsPanel bids={bids} />}
                                {(nav === "dashboard" || nav === "active") && <QueuePanel status={dashboard.status} queue={queue} actions={actions} />}
                                {nav === "audit" && <QueuePanel status={dashboard.status} queue={queue} actions={actions} />}
                                {nav === "audit" && <BidsPanel bids={bids} />}
                            </aside>
                        </div>
                    </div>
                )}
            </main>
        </div>
    );
}
