import { useContext, useEffect, useState } from "react";
import { AuthContext } from "../auth/AuthContext";
import Sidebar from "./components/Sidebar";
import Topbar from "./components/Topbar";
import { fetchArchiefVeilingen } from "../veiling/api/veilingApi";
import "./VeilingmeesterDashboard.css";

export default function Archief() {
    const { token, logout } = useContext(AuthContext);
    const [sidebarOpen, setSidebarOpen] = useState(false);

    const [items, setItems] = useState([]);
    const [error, setError] = useState("");

    useEffect(() => {
        if (!token) return;

        fetchArchiefVeilingen(token)
            .then((data) => setItems(data ?? []))
            .catch((e) => setError(e.message || "Kon archief niet laden"));
    }, [token]);

    return (
        <div className="vm-shell">
            <Sidebar logout={logout} sidebarOpen={sidebarOpen} setSidebarOpen={setSidebarOpen} />

            <div className="vm-content">
                <Topbar title="Archief" veiling={null} onMenuClick={() => setSidebarOpen(true)} />

                <main className="vm-page vm-archief-wrap">
                    {error && <div className="vm-alert vm-alert-danger">{error}</div>}

                    <section className="vm-hero-card">
                        <h2>Afgelopen veilingen</h2>
                        <p className="vm-muted mb-0">
                            Overzicht van afgesloten veilingen inclusief koper, tijdstip en prijs.
                        </p>
                    </section>

                    {items.length === 0 && (
                        <section className="vm-panel">
                            <p className="vm-muted mb-0">Nog geen afgesloten veilingen.</p>
                        </section>
                    )}

                    <section className="vm-archief-list">
                        {items.map((v) => (
                            <article key={v.id} className="vm-panel vm-archief-card">
                                <div className="vm-archief-head">
                                    <div>
                                        <h5 className="vm-archief-title">Veiling #{v.id}</h5>
                                        <div className="vm-muted vm-archief-sub">
                                            Veildatum: {v.veildatum} {" | "} Start: {v.startTijd} {" | "} Einde: {v.eindTijd}
                                        </div>
                                    </div>

                                    <div className="vm-archief-badge">
                                        {v.aantalProducten} producten
                                    </div>
                                </div>

                                <div className="vm-archief-divider" />

                                {!v.transacties?.length ? (
                                    <p className="vm-muted mb-0">Geen transacties gevonden.</p>
                                ) : (
                                    <div className="vm-archief-table">
                                        <div className="table-responsive">
                                            <table className="table table-sm align-middle mb-0">
                                                <thead>
                                                    <tr>
                                                        <th>Product</th>
                                                        <th>Koper</th>
                                                        <th>Aantal</th>
                                                        <th>Prijs</th>
                                                        <th>Tijd</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    {v.transacties.map((t, idx) => (
                                                        <tr key={idx}>
                                                            <td>{t.soort}</td>
                                                            <td>{t.koperNaam}</td>
                                                            <td>{t.aantal}</td>
                                                            <td>{t.prijs?.toFixed(2)} EUR</td>
                                                            <td>{t.tijdstip ? new Date(t.tijdstip).toLocaleString("nl-NL") : "-"}</td>
                                                        </tr>
                                                    ))}
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                )}
                            </article>
                        ))}
                    </section>
                </main>
            </div>
        </div>
    );
}
